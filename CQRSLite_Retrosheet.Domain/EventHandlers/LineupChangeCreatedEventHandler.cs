using AutoMapper;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using System;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.EventHandlers
{
    public class LineupChangeCreatedEventHandler : IEventHandler<LineupChangeCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly LineupChangeRepository _lineupChangeRepo;
        private readonly LineupRepository _lineupRepo;
        private readonly ICommandSender _commandSender;

        public LineupChangeCreatedEventHandler(IMapper mapper, LineupChangeRepository lineupChangeRepo, LineupRepository lineupRepo, ICommandSender commandSender)
        {
            _mapper = mapper;
            _lineupRepo = lineupRepo;
            _lineupChangeRepo = lineupChangeRepo;
            _commandSender = commandSender;
        }

        public async Task Handle(LineupChangeCreatedEvent message)
        {
            LineupChangeRM lineupChange = _mapper.Map<LineupChangeRM>(message);
            LineupRM lineup = _mapper.Map<LineupRM>(message);

            if (!message.LastLineupChange)
            {
                QueueLineupRMCommand cmd = new QueueLineupRMCommand(Guid.NewGuid(), lineup);
                await _commandSender.Send(cmd);
            }

            await _lineupChangeRepo.SaveAsync(lineupChange);
            await _lineupRepo.SaveAsync(lineup);
        }
    }
}
