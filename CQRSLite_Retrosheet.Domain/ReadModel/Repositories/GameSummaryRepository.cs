using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class GameSummaryRepository
    {
        private IRepository<GameSummaryRM> _repo;

        public GameSummaryRepository(IRepository<GameSummaryRM> repo)
        {
            _repo = repo;
        }

        public bool Exists(string retrosheetGameId)
        {
            retrosheetGameId = retrosheetGameId.Replace("'", "");
            return _repo.Exists(retrosheetGameId);
        }

        public GameSummaryRM GetGame(string retrosheetGameId)
        {
            retrosheetGameId = retrosheetGameId.Replace("'", "");
            return _repo.GetById(retrosheetGameId);
        }

        public List<GameSummaryRM> GetGamesBySeason(int year)
        {
            if (year.ToString().Length != 4)
            {
                return new List<GameSummaryRM>();
            }

            return _repo.GetByPartialKey(year.ToString(), 4).OrderBy(o => o.GameDay).ThenBy(o => o.RetrosheetGameId).ToList();
        }

        public async Task SaveAsync(GameSummaryRM item)
        {
            await _repo.SaveAsync(item, (x) => x.RetrosheetGameId);
        }
    }
}
