using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class CreateRosterMemberCommand : BaseCommand
    {
        public readonly int Year;
        public readonly string TeamCode;
        public readonly string PlayerId;
        public readonly string LastName;
        public readonly string FirstName;
        public readonly string Bats;
        public readonly string Throws;

        public CreateRosterMemberCommand(Guid id, int year, string teamCode, string playerid, string lastName, string firstName, string bats, string throws)
        {
            Id = id;
            Year = year;
            TeamCode = teamCode;
            PlayerId = playerid;
            LastName = lastName;
            FirstName = firstName;
            Bats = bats;
            Throws = throws;
        }
    }
}
