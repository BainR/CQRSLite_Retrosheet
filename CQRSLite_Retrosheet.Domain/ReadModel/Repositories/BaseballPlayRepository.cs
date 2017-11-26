using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class BaseballPlayRepository
    {
        IRepository<BaseballPlayRM> _repo;

        public BaseballPlayRepository(IRepository<BaseballPlayRM> repo)
        {
            _repo = repo;
        }

        public bool Exists(string gameid, int eventnumber)
        {
            gameid = gameid.Replace("'", "");
            string id = gameid + "_" + eventnumber.ToString("000");
            return _repo.Exists(id);
        }

        public BaseballPlayRM GetPlay(string gameid, int eventnumber)
        {
            gameid = gameid.Replace("'", "");
            string id = gameid + "_" + eventnumber.ToString("000");
            return _repo.GetById(id);
        }

        public List<BaseballPlayRM> GetGame(string gameid)
        {
            gameid = gameid.Replace("'", "");
            return _repo.GetByPartialKey(gameid, 1).OrderBy(o => o.RetrosheetGameId).ThenBy(o => o.EventNumber).ToList();
        }

        public async Task SaveAsync(BaseballPlayRM item)
        {
            await _repo.SaveAsync(item, (x) => x.RetrosheetGameId + "_" + x.EventNumber.ToString("000"));
        }
    }
}
