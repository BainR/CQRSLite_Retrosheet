using CQRSlite.Domain;
using CQRSLite_Retrosheet.Domain.Events;
using System;

namespace CQRSLite_Retrosheet.Domain.WriteModel
{
    public class RosterMember : AggregateRoot
    {
        private int _Year;
        private string _TeamCode;
        private string _PlayerId;
        private string _LastName;
        private string _FirstName;
        private string _Bats;
        private string _Throws;

        public RosterMember(Guid id, int year, string teamcode, string playerid, string lastname, string firstname, string bats, string throws)
        {
            Id = id;
            _Year = year;
            _TeamCode = teamcode;
            _PlayerId = playerid;
            _LastName = lastname;
            _FirstName = firstname;
            _Bats = bats;
            _Throws = throws;

            ApplyChange(new RosterMemberCreatedEvent(id, year, teamcode, playerid, lastname, firstname, bats, throws));
        }
    }
}
