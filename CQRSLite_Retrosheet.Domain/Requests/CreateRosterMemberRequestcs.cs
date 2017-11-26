using FluentValidation;
using System;

namespace CQRSLite_Retrosheet.Domain.Requests
{
    public class CreateRosterMemberRequest
    {
        public int Year { get; set; }
        public string TeamCode { get; set; }
        public string PlayerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Bats { get; set; }
        public string Throws { get; set; }
    }

    public class CreateRosterMemberRequestValidator : AbstractValidator<CreateRosterMemberRequest>
    {
        public CreateRosterMemberRequestValidator()
        {
            RuleFor(x => x.Year).InclusiveBetween(1903, DateTime.Now.Year);
            RuleFor(x => x.TeamCode).Must(x => x.Length == 3);
            RuleFor(x => x.PlayerId).Length(8);
            RuleFor(x => x.LastName).Length(1, 50);
            RuleFor(x => x.FirstName).Length(1, 50);
            RuleFor(x => x.Bats).Matches("^[LRB? ]$");
            RuleFor(x => x.Throws).Matches("^[LRB? ]$");
        }
    }
}
