using AutoMapper;
using CQRSlite.Events;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CQRSLite_RS_Core.Domain.EventHandlers
{
    public class RosterMemberCreatedEventHandler : IEventHandler<RosterMemberCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly RosterMemberRepository _rosterMemberRepo;
        private readonly PlayerRepository _playerRepo;
        private readonly ILogger _logger;

        public RosterMemberCreatedEventHandler(IMapper mapper, RosterMemberRepository rosterMemberRepo, PlayerRepository playerRepo, ILoggerFactory loggerFactory)
        {
            _mapper = mapper;
            _rosterMemberRepo = rosterMemberRepo;
            _playerRepo = playerRepo;
            _logger = loggerFactory.CreateLogger("RosterMember");
        }

        public async Task Handle(RosterMemberCreatedEvent message)
        {
            RosterMemberRM rosterMember = _mapper.Map<RosterMemberRM>(message);
            if (!_rosterMemberRepo.Exists(rosterMember.TeamCode, rosterMember.Year, rosterMember.PlayerId))
            {
                await _rosterMemberRepo.SaveAsync(rosterMember);
            }
            else
            {
                _logger.LogWarning("Duplicate Roster Member " + rosterMember.TeamCode + "_" + rosterMember.Year.ToString() + "_" + rosterMember.PlayerId);
            }

            PlayerRM player = new PlayerRM()
            {
                PlayerId = rosterMember.PlayerId,
                LastName = rosterMember.LastName,
                FirstName = rosterMember.FirstName,
                Bats = rosterMember.Bats,
                Throws = rosterMember.Throws
            };

            if (!_playerRepo.Exists(player.PlayerId))
            {
                await _playerRepo.SaveAsync(player);
            }
        }
    }
}
