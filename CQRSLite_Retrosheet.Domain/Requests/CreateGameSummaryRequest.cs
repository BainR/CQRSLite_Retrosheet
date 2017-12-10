using FluentValidation;
using System;
using System.Globalization;

namespace CQRSLite_Retrosheet.Domain.Requests
{
    public class CreateGameSummaryRequest
    {
        public string RetrosheetGameId { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeam { get; set; }
        public string UseDH { get; set; } // true or false
        public string ParkCode { get; set; }
        public string WinningPitcher { get; set; }
        public string LosingPitcher { get; set; }
        public string SavePitcher { get; set; }
        public bool? HasValidationErrors { get; set; }
        public string GameDay { get; set; }
        public int? HomeTeamFinalScore { get; set; }
        public int? AwayTeamFinalScore { get; set; }
    }

    public class CreateGameSummaryRequestValidator : AbstractValidator<CreateGameSummaryRequest>
    {
        public CreateGameSummaryRequestValidator()
        {
            RuleFor(x => x.RetrosheetGameId).NotNull().NotEmpty().Length(12).WithMessage("RetrosheetGameId must have exactly 12 characters.");
            RuleFor(x => x.RetrosheetGameId).Must(r => DateTime.TryParseExact(r.Substring(3, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _)).WithMessage("RetrosheetGameId must contain a valid date.");
            RuleFor(x => x.AwayTeam).NotNull().NotEmpty().Length(3).WithMessage("Away team code must have exactly 3 characters.");
            RuleFor(x => x.HomeTeam).NotNull().NotEmpty().Length(3).WithMessage("Home team code must have exactly 3 characters.");
            RuleFor(x => x.UseDH).NotNull().NotEmpty().Must(dh => bool.TryParse(dh, out var _)).WithMessage("UseDH must be true or false");
            RuleFor(x => x.ParkCode).Must(x => string.IsNullOrEmpty(x) || x.Length == 5).WithMessage("Park code must have exactly 5 characters.");
            RuleFor(x => x.WinningPitcher).Must(p => string.IsNullOrEmpty(p) || p.Length == 8).WithMessage("Winning pitcher, if given, must have exactly 8 characters.");
            RuleFor(x => x.LosingPitcher).Must(p => string.IsNullOrEmpty(p) || p.Length == 8).WithMessage("Losing pitcher, if given, must have exactly 8 characters.");
            RuleFor(x => x.SavePitcher).Must(p => string.IsNullOrEmpty(p) || p.Length == 8).WithMessage("Save pitcher, if given, must have exactly 8 characters.");
            RuleFor(x => x.HasValidationErrors).Null();
            RuleFor(x => x.GameDay).Null();
            RuleFor(x => x.HomeTeamFinalScore).Null();
            RuleFor(x => x.AwayTeamFinalScore).Null();
        }
    }
}
