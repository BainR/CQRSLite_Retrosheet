using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class TeamRepository
    {
        private IRepository<TeamRM> _repo;

        public TeamRepository(IRepository<TeamRM> repo)
        {
            _repo = repo;
        }

        public bool Exists(string teamcode, int year)
        {
            teamcode = teamcode.Replace("'", "");
            string id = teamcode + "_" + year;
            return _repo.Exists(id);
        }

        public TeamRM GetTeam(string teamcode, int year)
        {
            teamcode = teamcode.Replace("'", "");
            string id = teamcode + "_" + year;
            return _repo.GetById(id);
        }

        public List<TeamRM> GetTeamsByTeamCode(string teamCode)
        {
            teamCode = teamCode.Replace("'", "");
            return _repo.GetByPartialKey(teamCode, 1).OrderBy(o => o.Year).ToList();
        }

        public List<TeamRM> GetTeamsByYear(int year)
        {
            return _repo.GetByPartialKey(year.ToString(), 5).OrderBy(o => o.League).ThenBy(o => o.TeamCode).ToList();
        }

        public async Task SaveAsync(TeamRM item)
        {
            await _repo.SaveAsync(item, (x) => x.TeamCode + "_" + x.Year.ToString());
        }
    }
}
