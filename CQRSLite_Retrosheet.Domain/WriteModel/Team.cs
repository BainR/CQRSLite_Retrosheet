using CQRSlite.Domain;
using CQRSLite_Retrosheet.Domain.Events;
using System;

namespace CQRSLite_Retrosheet.Domain.WriteModel
{
    public class Team : AggregateRoot
    {
        private int _Year;
        private string _TeamCode;
        private string _League;
        private string _Home;
        private string _Name;

        public Team(Guid id, int year, string teamcode, string league, string home, string name)
        {
            Id = id;
            _Year = year;
            _TeamCode = teamcode;
            _League = league;
            _Home = home;
            _Name = name;

            ApplyChange(new TeamCreatedEvent(id, year, teamcode, league, home, name));
        }
    }
}
