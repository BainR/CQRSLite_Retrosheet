using FluentValidation;
using System;

namespace CQRSLite_Retrosheet.Domain.Requests
{
    public class CreateTeamRequest
    {
        public int Year { get; set; }
        public string TeamCode { get; set; }
        public string League { get; set; }
        public string Home { get; set; }
        public string Name { get; set; }
    }

    public class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
    {
        public CreateTeamRequestValidator()
        {
            RuleFor(x => x.Year).InclusiveBetween(1903, DateTime.Now.Year);
            RuleFor(x => x.TeamCode).Must(x => x.Length == 3);
            RuleFor(x => x.League).Matches("^[AN]$");
            RuleFor(x => x.Home).Length(1, 20);
            RuleFor(x => x.Name).Length(1, 20);
        }
    }
}
