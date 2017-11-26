using AutoMapper;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using Microsoft.Extensions.Logging;
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
            Dictionaries.Dictionaries.cdBaseballPlayRM.GetOrAdd(message.RetrosheetGameId + message.EventNumber.ToString("000"), baseballPlay);

            if (message.Details.EndOfGame)
            {
                baseballGameCompletedLogger.LogTrace(message.RetrosheetGameId);
            }

            await _baseballPlayRepo.SaveAsync(baseballPlay);
        }
    }
}
