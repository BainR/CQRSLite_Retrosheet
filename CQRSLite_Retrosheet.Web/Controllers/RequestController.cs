using AutoMapper;
using CQRSlite.Commands;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Web.Controllers
{
    [Route("Retrosheet/Write")]
    public class RequestController : Controller
    {
        private IMapper _mapper;
        private ICommandSender _commandSender;

        public RequestController(IMapper mapper, ICommandSender commandSender)
        {
            _mapper = mapper;
            _commandSender = commandSender;
        }

        [HttpPost("GameSummary")]
        public async Task<IActionResult> AddGameSummary([FromBody] CreateGameSummaryRequest request)
        {
            QueueGameSummaryRequestCommand cmd = new QueueGameSummaryRequestCommand(Guid.NewGuid(), request);
            await _commandSender.Send(cmd);

            return Ok();
        }

        [HttpPost("LineupChange")]
        public async Task<IActionResult> AddStarter([FromBody] CreateLineupChangeRequest request)
        {
            var command = new QueueLineupChangeRequestCommand(Guid.NewGuid(), request);
            await _commandSender.Send(command);

            return Ok();
        }

        [HttpPost("Event")]
        public async Task<IActionResult> AddEvent([FromBody] CreateBaseballPlayRequest request)
        {
            QueueBaseballPlayRequestCommand cmd = new QueueBaseballPlayRequestCommand(Guid.NewGuid(), request);
            await _commandSender.Send(cmd);

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
