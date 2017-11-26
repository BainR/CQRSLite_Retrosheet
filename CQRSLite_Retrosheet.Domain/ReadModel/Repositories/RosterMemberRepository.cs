using CQRSLite_Retrosheet.Domain.ReadModel.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.Domain.ReadModel.Repositories
{
    public class RosterMemberRepository
    {
        private IRepository<RosterMemberRM> _repo;

        public RosterMemberRepository(IRepository<RosterMemberRM> repo)
        {
            _repo = repo;
        }

        public bool Exists(string teamcode, int year, string playerid)
        {
            teamcode = teamcode.Replace("'", "");
            playerid = playerid.Replace("'", "");
            string id = teamcode + "_" + year.ToString() + "_" + playerid;

            return _repo.Exists(id);
        }

        public RosterMemberRM GetRosterMember(string teamcode, int year, string playerid)
        {
            teamcode = teamcode.Replace("'", "");
            playerid = playerid.Replace("'", "");
            string id = teamcode + "_" + year.ToString() + "_" + playerid;

            return _repo.GetById(id);
        }

        public List<RosterMemberRM> GetRoster(string teamcode, int year)
        {
            teamcode = teamcode.Replace("'", "");
            string id = teamcode + "_" + year.ToString();

            return _repo.GetByPartialKey(id, 1).OrderBy(o => o.Year).ToList();
        }

        public async Task SaveAsync(RosterMemberRM item)
        {
            await _repo.SaveAsync(item, (x) => x.TeamCode + "_" + x.Year.ToString() + "_" + x.PlayerId);
        }
    }
}
