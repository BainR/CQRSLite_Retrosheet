using System;

namespace CQRSLite_Retrosheet.Domain.Events
{
    public class TeamCreatedEvent : BaseEvent
    {
        public readonly int Year;
        public readonly string TeamCode;
        public readonly string League;
        public readonly string Home;
        public readonly string Name;

        public TeamCreatedEvent(Guid id, int year, string teamcode, string league, string home, string name)
        {
            Id = id;
            Year = year;
            TeamCode = teamcode;
            League = league;
            Home = home;
            Name = name;
        }
    }
}
