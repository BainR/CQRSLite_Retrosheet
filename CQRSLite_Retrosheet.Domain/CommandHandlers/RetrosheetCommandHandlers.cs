using AutoMapper;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.WriteModel;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.CommandHandlers
{
    public class RetrosheetCommandHandlers :
        ICommandHandler<CreateGameSummaryCommand>,
        ICommandHandler<CreateBaseballPlayCommand>,
        ICommandHandler<CreateLineupChangeCommand>,
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
                command.HasValidationErrors, command.GameDay, command.HomeTeamFinalScore, command.AwayTeamFinalScore);
            await _session.Add(gameSummary);
            await _session.Commit();
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

        public async Task Handle(CreateLineupChangeCommand command)
        {
            LineupChange lineup = new LineupChange(command.Id, command.RetrosheetGameId, command.EventNumber, command.Sequence, command.IsStarter,
                command.PlayerId, command.Name, command.Team, command.BattingOrder, command.FieldPosition, command.LastLineupChange, command.PreviousBattingOrder);
            await _session.Add(lineup);
            await _session.Commit();
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
