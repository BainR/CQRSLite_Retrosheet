using AutoMapper;
using CQRSlite.Commands;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Dictionaries;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Web.Controllers
{
    [Route("Retrosheet/Write")]
    public class RequestController : Controller
    {
        private IMapper _mapper;
        private ICommandSender _commandSender;
        private ILogger baseballPlayValidationLogger;

        public RequestController(IMapper mapper, ICommandSender commandSender, ILoggerFactory loggerFactory)
        {
            _mapper = mapper;
            _commandSender = commandSender;
            baseballPlayValidationLogger = loggerFactory.CreateLogger("BaseballPlayValidation");
        }

        [HttpPost("GameSummary")]
        public async Task<IActionResult> AddGameSummary([FromBody] CreateGameSummaryRequest request)
        {
            BaseballPlayRM lastPlayOfGame = null;
            var lastPlayKey = Dictionaries.cdBaseballPlayRM.Keys.FirstOrDefault(k => k.StartsWith(request.RetrosheetGameId));
            if (!string.IsNullOrEmpty(lastPlayKey))
            {
                Dictionaries.cdBaseballPlayRM.TryRemove(lastPlayKey, out lastPlayOfGame);
            }

            if (lastPlayOfGame == null || !lastPlayOfGame.EndOfGame)
            {
                request.HasValidationErrors = true;
            }
            else
            { 
                request.HasValidationErrors = false;
                request.HomeTeamFinalScore = lastPlayOfGame.EndOfPlay_HomeScore;
                request.AwayTeamFinalScore = lastPlayOfGame.EndOfPlay_VisitorScore;
            }
            request.GameDay = request.RetrosheetGameId.Substring(3, 8);
            var cmd = _mapper.Map<CreateGameSummaryCommand>(request);
            await _commandSender.Send(cmd);

            return Ok();
        }

        [HttpPost("LineupChange")]
        public async Task<IActionResult> AddStarterOrSub([FromBody] CreateLineupChangeRequest request)
        {
            LineupRM previousLineup = null;

            if (request.Sequence > 1)
            {
                Dictionaries.cdLineupRM.TryRemove(request.RetrosheetGameId + (request.Sequence - 1).ToString("000"), out previousLineup);
            }

            request.PreviousBattingOrder = previousLineup;
            var cmd = _mapper.Map<CreateLineupChangeCommand>(request);
            await _commandSender.Send(cmd);

            return Ok();
        }

        [HttpPost("Event")]
        public async Task<IActionResult> AddEvent([FromBody] CreateBaseballPlayRequest request)
        {
            BaseballPlayRM previousPlay = null;

            if (request.EventNumber > 1 && !Dictionaries.cdBaseballPlayRM.TryRemove(request.RetrosheetGameId + (request.EventNumber - 1).ToString("000"), out previousPlay))
            {
                return StatusCode(500, "Previous play data not available on server.");
            }

            BaseballPlayDetails details = new BaseballPlayDetails(previousPlay, request.RetrosheetGameId, request.EventNumber, request.TeamAtBat.ToString(), request.EventText, request.LastPlay);
            request.Details = details;

            CreateBaseballPlayRequestValidator validator = new CreateBaseballPlayRequestValidator();
            var validationResults = validator.Validate(request);

            foreach (var warning in validationResults.Errors.Where(e => e.Severity == FluentValidation.Severity.Warning))
            {
                baseballPlayValidationLogger.LogWarning(request.RetrosheetGameId + " " + request.EventNumber.ToString("000")
                    + " " + request.EventText + " : " + warning.ErrorMessage);
            }

            if (!validationResults.Errors.Any(e => e.Severity == FluentValidation.Severity.Error))
            {
                var cmd = _mapper.Map<CreateBaseballPlayCommand>(request);
                await _commandSender.Send(cmd);
            }
            else
            {
                foreach (var validationError in validationResults.Errors.Where(e => e.Severity == FluentValidation.Severity.Error))
                {
                    baseballPlayValidationLogger.LogError(request.RetrosheetGameId + " " + request.EventNumber.ToString("000")
                         + " " + request.EventText + " : " + validationError.ErrorMessage);
                }
                return BadRequest(request.RetrosheetGameId + " " + request.EventNumber.ToString("000") + " " + request.EventText + " has validation errors");
            }

            return Ok();
        }

        [HttpPost("AddTeam")]
        public async Task<IActionResult> AddTeam([FromBody] CreateTeamRequest request)
        {
            var cmd = _mapper.Map<CreateTeamCommand>(request);
            await _commandSender.Send(cmd);

            return Ok();
        }

        [HttpPost("AddRosterMember")]
        public async Task<IActionResult> AddRosterMember([FromBody] CreateRosterMemberRequest request)
        {
            var cmd = _mapper.Map<CreateRosterMemberCommand>(request);
            await _commandSender.Send(cmd);

            return Ok();
        }
    }
}
