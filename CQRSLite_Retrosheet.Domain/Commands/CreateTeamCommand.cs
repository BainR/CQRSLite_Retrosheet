using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class CreateTeamCommand : BaseCommand
    {
        public readonly int Year;
        public readonly string TeamCode;
        public readonly string League;
        public readonly string Home;
        public readonly string Name;

        public CreateTeamCommand(Guid id, int year, string teamCode, string league, string home, string name)
        {
            Id = id;
            Year = year;
            TeamCode = teamCode;
            League = league;
            Home = home;
            Name = name;
        }
    }
}
