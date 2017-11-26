using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class PlayerRepository
    {
        private IRepository<PlayerRM> _repo;

        public PlayerRepository(IRepository<PlayerRM> repo)
        {
            _repo = repo;
        }

        public bool Exists(string id)
        {
            return _repo.Exists(id);
        }

        public PlayerRM GetPlayer(string id)
        {
            id = id.Replace("'", "");
            return _repo.GetById(id);
        }

        public List<PlayerRM> GetAll()
        {
            return _repo.GetByPartialKey("", 1).OrderBy(o => o.PlayerId).ToList();
        }

        public async Task SaveAsync(PlayerRM item)
        {
            await _repo.SaveAsync(item, (x) => x.PlayerId);
        }
    }
}
