using System;

namespace CQRSLite_Retrosheet.Domain.Events
{
    public class RosterMemberCreatedEvent : BaseEvent
    {
        public readonly int Year;
        public readonly string TeamCode;
        public readonly string PlayerId;
        public readonly string LastName;
        public readonly string FirstName;
        public readonly string Bats;
        public readonly string Throws;

        public RosterMemberCreatedEvent(Guid id, int year, string teamcode, string playerid, string lastname, string firstname, string bats, string throws)
        {
            Id = id;
            Year = year;
            TeamCode = teamcode;
            PlayerId = playerid;
            LastName = lastname;
            FirstName = firstname;
            Bats = bats;
            Throws = throws;
        }
    }
}
