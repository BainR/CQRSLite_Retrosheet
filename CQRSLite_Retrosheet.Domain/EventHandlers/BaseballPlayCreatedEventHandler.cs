using AutoMapper;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.EventHandlers
{
    public class BaseballPlayCreatedEventHandler : IEventHandler<BaseballPlayCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly BaseballPlayRepository _baseballPlayRepo;
        private ICommandSender _commandSender;
        private ILogger baseballGameCompletedLogger;

        public BaseballPlayCreatedEventHandler(IMapper mapper, BaseballPlayRepository baseballPlayRepo, ICommandSender commandSender, ILoggerFactory loggerFactory)
        {
            _mapper = mapper;
            _baseballPlayRepo = baseballPlayRepo;
            _commandSender = commandSender;
            baseballGameCompletedLogger = loggerFactory.CreateLogger("BaseballGameCompleted");
        }

        public async Task Handle(BaseballPlayCreatedEvent message)
        {
            BaseballPlayRM baseballPlay = _mapper.Map<BaseballPlayRM>(message);

            if (!message.Details.EndOfGame)
            {
                QueueBaseballPlayRMCommand cmd = new QueueBaseballPlayRMCommand(Guid.NewGuid(), baseballPlay);
                await _commandSender.Send(cmd);
            }
            else
            {
                baseballGameCompletedLogger.LogTrace(message.RetrosheetGameId);
                bool homeTeamBatsFirst = (message.Details.TeamAtBat == "V" && message.Details.IsBottomHalf == true) || (message.Details.TeamAtBat == "H" && message.Details.IsBottomHalf == false);
                QueueGameCompletedCommand cmd = new QueueGameCompletedCommand(Guid.NewGuid(), baseballPlay.RetrosheetGameId, baseballPlay.EndOfPlay_HomeScore, baseballPlay.EndOfPlay_VisitorScore, homeTeamBatsFirst);
                await _commandSender.Send(cmd);
            }

            await _baseballPlayRepo.SaveAsync(baseballPlay);
        }
    }
}
