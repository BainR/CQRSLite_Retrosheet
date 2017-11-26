using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class LineupChangeRepository
    {
        IRepository<LineupChangeRM> _repo;

        public LineupChangeRepository(IRepository<LineupChangeRM> repo)
        {
            _repo = repo;
        }

        public List<LineupChangeRM> GetGameLineupChanges(string retrosheetGameId)
        {
            retrosheetGameId = retrosheetGameId.Replace("'", "");
            return _repo.GetByPartialKey(retrosheetGameId, 1).OrderBy(o => o.EventNumber).ThenBy(o => o.Sequence).ToList();
        }

        public async Task SaveAsync(LineupChangeRM lineup)
        {
            await _repo.SaveAsync(lineup, (x) => x.RetrosheetGameId + "_" + x.EventNumber.ToString("000") + "_" + x.Sequence.ToString("000"));
        }
    }
}
