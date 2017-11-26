using AutoMapper;
using CQRSlite.Events;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.EventHandlers
{
    public class GameSummaryCreatedEventHandler : IEventHandler<GameSummaryCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly GameSummaryRepository _summaryRepo;

        public GameSummaryCreatedEventHandler(IMapper mapper, GameSummaryRepository summaryRepo)
        {
            _mapper = mapper;
            _summaryRepo = summaryRepo;
        }

        public async Task Handle(GameSummaryCreatedEvent message)
        {
            GameSummaryRM summary = _mapper.Map<GameSummaryRM>(message);
            await _summaryRepo.SaveAsync(summary);
        }
    }
}
