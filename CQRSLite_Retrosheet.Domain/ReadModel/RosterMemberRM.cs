namespace CQRSLite_Retrosheet.Domain.ReadModel
{
    public class RosterMemberRM
    {
        public int Year { get; set; }
        public string TeamCode { get; set; }
        public string PlayerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Bats { get; set; }
        public string Throws { get; set; }
    }
}
