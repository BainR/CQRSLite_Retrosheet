using AutoMapper;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using CQRSLite_Retrosheet.Domain.WriteModel;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.CommandHandlers
{
    public class RetrosheetCommandHandlers :
        ICommandHandler<CreateGameSummaryCommand>,
        ICommandHandler<QueueGameSummaryRequestCommand>,
        ICommandHandler<QueueGameCompletedCommand>,
        ICommandHandler<CreateBaseballPlayCommand>,
        ICommandHandler<QueueBaseballPlayRequestCommand>,
        ICommandHandler<QueueBaseballPlayRMCommand>,
        ICommandHandler<CreateLineupChangeCommand>,
        ICommandHandler<QueueLineupChangeRequestCommand>,
        ICommandHandler<QueueLineupRMCommand>,
        ICommandHandler<CreateTeamCommand>,
        ICommandHandler<CreateRosterMemberCommand>
    {
        private readonly ISession _session;
        private ICommandSender _commandSender;
        private IMapper _mapper;
        private ILogger baseballPlayValidationLogger;
        private ILogger baseballGameStartedLogger;
        private object summaryLock = new object();
        private object baseballPlayLock = new object();
        private object lineupChangeLock = new object();

        public RetrosheetCommandHandlers(ISession session, ICommandSender commandSender, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _session = session;
            _commandSender = commandSender;
            _mapper = mapper;
            baseballPlayValidationLogger = loggerFactory.CreateLogger("BaseballPlayValidation");
            baseballGameStartedLogger = loggerFactory.CreateLogger("BaseballGameStarted");
        }

        public async Task Handle(CreateGameSummaryCommand command)
        {
            GameSummary gameSummary = new GameSummary(command.Id, command.RetrosheetGameId, command.AwayTeam, command.HomeTeam, 
                command.UseDH, command.ParkCode, command.WinningPitcher, command.LosingPitcher, command.SavePitcher, 
                command.HasValidationErrors, command.GameDay, command.HomeTeamFinalScore, command.AwayTeamFinalScore, command.HomeTeamBatsFirst);
            await _session.Add(gameSummary);
            await _session.Commit();
        }

        public async Task Handle(QueueGameSummaryRequestCommand command)
        {
            GameSummaryRM summary = null;

            lock (summaryLock)
            {
                if (!Queues.Queues.cdGameSummaryRM.TryRemove(command.Request.RetrosheetGameId, out summary))
                {
                    Queues.Queues.cdCreateGameSummaryRequest.GetOrAdd(command.Request.RetrosheetGameId, command.Request);
                    return;
                }
            }

            command.Request.HasValidationErrors = false;
            command.Request.GameDay = summary.RetrosheetGameId.Substring(3, 8);
            command.Request.HomeTeamFinalScore = summary.HomeTeamFinalScore;
            command.Request.AwayTeamFinalScore = summary.AwayTeamFinalScore;
            command.Request.HomeTeamBatsFirst = summary.HomeTeamBatsFirst;
            var cmd = _mapper.Map<CreateGameSummaryCommand>(command.Request);
            await _commandSender.Send(cmd);
        }

        public async Task Handle(QueueGameCompletedCommand command)
        {
            CreateGameSummaryRequest summaryRequest = null;

            lock (summaryLock)
            {
                if (!Queues.Queues.cdCreateGameSummaryRequest.TryRemove(command.RetrosheetGameId, out summaryRequest))
                {
                    Queues.Queues.cdGameSummaryRM.GetOrAdd(command.RetrosheetGameId, new GameSummaryRM()
                    {
                        RetrosheetGameId = command.RetrosheetGameId,
                        HomeTeamFinalScore = command.HomeTeamFinalScore,
                        AwayTeamFinalScore = command.AwayTeamFinalScore
                    });
                    return;
                }
            }

            summaryRequest.AwayTeamFinalScore = command.AwayTeamFinalScore;
            summaryRequest.HomeTeamFinalScore = command.HomeTeamFinalScore;
            summaryRequest.GameDay = command.RetrosheetGameId.Substring(3, 8);
            summaryRequest.HomeTeamBatsFirst = command.HomeTeamBatsFirst;
            summaryRequest.HasValidationErrors = false;
            var cmd = _mapper.Map<CreateGameSummaryCommand>(summaryRequest);
            await _commandSender.Send(cmd);
        }

        public async Task Handle(CreateBaseballPlayCommand command)
        {
            if (command.EventNumber == 1)
            {
                baseballGameStartedLogger.LogTrace(command.RetrosheetGameId);
            }

            BaseballPlay baseballPlay = new BaseballPlay(command.Id, command.RetrosheetGameId, command.EventNumber, command.LineupChangeSequence,
                command.EventText, command.Batter, command.CountOnBatter, command.Pitches, command.Details);
            await _session.Add(baseballPlay);
            await _session.Commit();
        }

        public async Task Handle(QueueBaseballPlayRequestCommand command)
        {
            BaseballPlayRM previousPlay = null;

            lock (baseballPlayLock)
            {
                if (command.Request.EventNumber > 1 && !Queues.Queues.cdBaseballPlayRM.TryRemove(command.Request.RetrosheetGameId + (command.Request.EventNumber - 1).ToString("000"), out previousPlay))
                {
                    Queues.Queues.cdCreateBaseballPlayRequest.GetOrAdd(command.Request.RetrosheetGameId + command.Request.EventNumber.ToString("000"), command.Request);
                    return;
                }
            }

            BaseballPlayDetails details = new BaseballPlayDetails(previousPlay, command.Request.RetrosheetGameId, command.Request.EventNumber, 
                command.Request.TeamAtBat.ToString(), command.Request.EventText, command.Request.LastPlay);
            command.Request.Details = details;

            CreateBaseballPlayRequestValidator validator = new CreateBaseballPlayRequestValidator();
            var validationResults = validator.Validate(command.Request);

            foreach(var warning in validationResults.Errors.Where(e => e.Severity == FluentValidation.Severity.Warning))
            {
                baseballPlayValidationLogger.LogWarning(command.Request.RetrosheetGameId + " " + command.Request.EventNumber.ToString("000")
                    + " " + command.Request.EventText + " : " + warning.ErrorMessage);
            }

            if (!validationResults.Errors.Any(e => e.Severity == FluentValidation.Severity.Error))
            {
                var cmd = _mapper.Map<CreateBaseballPlayCommand>(command.Request);
                await _commandSender.Send(cmd);
            }
            else
            {
                foreach (var validationError in validationResults.Errors.Where(e => e.Severity == FluentValidation.Severity.Error))
                {
                    baseballPlayValidationLogger.LogError(command.Request.RetrosheetGameId + " " + command.Request.EventNumber.ToString("000")
                         + " " + command.Request.EventText + " : " + validationError.ErrorMessage);
                }

                Queues.Queues.cdCreateGameSummaryRequest.TryRemove(command.Request.RetrosheetGameId, out CreateGameSummaryRequest gsRequest);
                gsRequest.HasValidationErrors = true;
                gsRequest.GameDay = command.Request.RetrosheetGameId.Substring(3, 8);
                var cmd = _mapper.Map<CreateGameSummaryCommand>(gsRequest);
                await _commandSender.Send(cmd);
            }
        }

        public async Task Handle(QueueBaseballPlayRMCommand command)
        {
            CreateBaseballPlayRequest nextRequest = null;

            lock (baseballPlayLock)
            {
                if (!Queues.Queues.cdCreateBaseballPlayRequest.TryRemove(command.BaseballPlay.RetrosheetGameId + (command.BaseballPlay.EventNumber + 1).ToString("000"), out nextRequest))
                {
                    Queues.Queues.cdBaseballPlayRM.GetOrAdd(command.BaseballPlay.RetrosheetGameId + command.BaseballPlay.EventNumber.ToString("000"), command.BaseballPlay);
                    return;
                }
            }

            var details = new BaseballPlayDetails(command.BaseballPlay, nextRequest.RetrosheetGameId, nextRequest.EventNumber, 
                nextRequest.TeamAtBat.ToString(), nextRequest.EventText, nextRequest.LastPlay);
            nextRequest.Details = details;

            CreateBaseballPlayRequestValidator validator = new CreateBaseballPlayRequestValidator();
            var validationResults = validator.Validate(nextRequest);


            foreach (var warning in validationResults.Errors.Where(e => e.Severity == FluentValidation.Severity.Warning))
            {
                baseballPlayValidationLogger.LogWarning(nextRequest.RetrosheetGameId + " " + nextRequest.EventNumber.ToString("000")
                    + " " + nextRequest.EventText + " : " + warning.ErrorMessage);
            }

            if (!validationResults.Errors.Any(e => e.Severity == FluentValidation.Severity.Error))
            {
                var cmd = _mapper.Map<CreateBaseballPlayCommand>(nextRequest);
                await _commandSender.Send(cmd);
            }
            else
            {
                foreach (var validationError in validationResults.Errors.Where(e => e.Severity == FluentValidation.Severity.Error))
                {
                    baseballPlayValidationLogger.LogError(nextRequest.RetrosheetGameId + " " + nextRequest.EventNumber.ToString("000")
                         + " " + nextRequest.EventText + " : " + validationError.ErrorMessage);
                }

                Queues.Queues.cdCreateGameSummaryRequest.TryRemove(nextRequest.RetrosheetGameId, out CreateGameSummaryRequest gsRequest);
                gsRequest.HasValidationErrors = true;
                gsRequest.GameDay = nextRequest.RetrosheetGameId.Substring(3, 8);
                var cmd = _mapper.Map<CreateGameSummaryCommand>(gsRequest);
                await _commandSender.Send(cmd);
            }
        }

        public async Task Handle(CreateLineupChangeCommand command)
        {
            LineupChange lineup = new LineupChange(command.Id, command.RetrosheetGameId, command.EventNumber, command.Sequence, command.IsStarter,
                command.PlayerId, command.Name, command.Team, command.BattingOrder, command.FieldPosition, command.LastLineupChange, command.PreviousBattingOrder);
            await _session.Add(lineup);
            await _session.Commit();
        }

        public async Task Handle(QueueLineupChangeRequestCommand command)
        {
            LineupRM previousLineup = null;

            lock (lineupChangeLock)
            {
                if (command.Request.Sequence > 1 && !Queues.Queues.cdLineupRM.TryRemove(command.Request.RetrosheetGameId + (command.Request.Sequence - 1).ToString("000"), out previousLineup))
                {
                    Queues.Queues.cdCreateLineupChangeRequest.GetOrAdd(command.Request.RetrosheetGameId + command.Request.Sequence.ToString("000"), command.Request);
                    return;
                }
            }

            command.Request.PreviousBattingOrder = previousLineup;
            var cmd = _mapper.Map<CreateLineupChangeCommand>(command.Request);
            await _commandSender.Send(cmd);
        }

        public async Task Handle(QueueLineupRMCommand command)
        {
            CreateLineupChangeRequest nextRequest = null;

            lock (lineupChangeLock)
            {
                if (!Queues.Queues.cdCreateLineupChangeRequest.TryRemove(command.BattingOrder.RetrosheetGameId + (command.BattingOrder.Sequence + 1).ToString("000"), out nextRequest))
                {
                    Queues.Queues.cdLineupRM.GetOrAdd(command.BattingOrder.RetrosheetGameId + command.BattingOrder.Sequence.ToString("000"), command.BattingOrder);
                    return;
                }
            }

            nextRequest.PreviousBattingOrder = command.BattingOrder;
            var cmd = _mapper.Map<CreateLineupChangeCommand>(nextRequest);
            await _commandSender.Send(cmd);
        }

        public async Task Handle(CreateTeamCommand command)
        {
            Team team = new Team(command.Id, command.Year, command.TeamCode, command.League, command.Home, command.Name);
            await _session.Add(team);
            await _session.Commit();
        }

        public async Task Handle(CreateRosterMemberCommand command)
        {
            RosterMember rosterMember = new RosterMember(command.Id, command.Year, command.TeamCode, command.PlayerId, command.LastName, command.FirstName, command.Bats, command.Throws);
            await _session.Add(rosterMember);
            await _session.Commit();
        }
    }
}
