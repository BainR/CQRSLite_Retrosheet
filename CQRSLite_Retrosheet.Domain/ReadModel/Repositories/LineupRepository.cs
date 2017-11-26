using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class LineupRepository
    {
        IRepository<LineupRM> _repo;

        public LineupRepository(IRepository<LineupRM> repo)
        {
            _repo = repo;
        }

        public async Task SaveAsync(LineupRM lineup)
        {
            await _repo.SaveAsync(lineup, (x) => x.RetrosheetGameId + "_" + x.Sequence.ToString("000"));
        }

        public LineupRM GetLineup(string retrosheetGameId, short sequence)
        {
            return _repo.GetById(retrosheetGameId + "_" + sequence.ToString("000"));
        }

        public List<LineupRM> GetGameLineup(string retrosheetGameId)
        {
            return _repo.GetByPartialKey(retrosheetGameId, 1).OrderBy(o => o.Sequence).ToList();
        }
    }
}
