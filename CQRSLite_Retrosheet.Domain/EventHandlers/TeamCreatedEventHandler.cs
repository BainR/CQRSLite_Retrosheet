using AutoMapper;
using CQRSlite.Events;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.EventHandlers
{
    public class TeamCreatedEventHandler : IEventHandler<TeamCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly TeamRepository _teamRepo;
        public TeamCreatedEventHandler(IMapper mapper, TeamRepository teamRepo)
        {
            _mapper = mapper;
            _teamRepo = teamRepo;
        }

        public async Task Handle(TeamCreatedEvent message)
        {
            TeamRM team = _mapper.Map<TeamRM>(message);
            await _teamRepo.SaveAsync(team);
        }
    }
}
