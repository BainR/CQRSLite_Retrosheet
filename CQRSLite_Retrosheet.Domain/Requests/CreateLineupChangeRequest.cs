using CQRSLite_Retrosheet.Domain.ReadModel;
using FluentValidation;

namespace CQRSLite_Retrosheet.Domain.Requests
{
    public class CreateLineupChangeRequest
    {
        public string RetrosheetGameId { get; set; }
        public short EventNumber { get; set; }
        public short Sequence { get; set; }
        public bool IsStarter { get; set; }
        public string PlayerId { get; set; }
        public string Name { get; set; }
        public byte Team { get; set; }
        public byte BattingOrder { get; set; }
        public byte FieldPosition { get; set; }
        public bool LastLineupChange { get; set; }
        public LineupRM PreviousBattingOrder { get; set; }
    }

    public class CreateLineupChangeRequestValidator : AbstractValidator<CreateLineupChangeRequest>
    {
        public CreateLineupChangeRequestValidator()
        {
            RuleFor(x => x.RetrosheetGameId).NotNull().NotEmpty().Length(12).WithMessage("RetrosheetGameId must have exactly 12 characters.");
            RuleFor(x => x.EventNumber).GreaterThan((short)0).LessThanOrEqualTo((short)255).WithMessage("Event Number must be between 0 and 255.");
            RuleFor(x => x.Sequence).GreaterThan((short)0).LessThanOrEqualTo((short)255).WithMessage("Sequence must be between 0 and 255.");
            RuleFor(x => x.PlayerId).NotNull().NotEmpty().Length(8).WithMessage("RetrosheetGameId must have exactly 8 characters.");
            RuleFor(x => x.Name).NotNull().NotEmpty().Length(1, 101).WithMessage("RetrosheetGameId must have exactly 8 characters.");
            RuleFor(x => x.Team).GreaterThanOrEqualTo((byte)0).LessThanOrEqualTo((byte)1).WithMessage("Team must be between 0 and 1.");
            RuleFor(x => x.BattingOrder).GreaterThanOrEqualTo((byte)0).LessThanOrEqualTo((byte)9).WithMessage("Batting Order Position must be between 0 and 9.");
            RuleFor(x => x.FieldPosition).GreaterThanOrEqualTo((byte)1).LessThanOrEqualTo((byte)12).WithMessage("Field Position must be between 1 and 12.");
            RuleFor(x => x).Must(x => (x.IsStarter && x.EventNumber == 1) || !x.IsStarter).WithName("IsStarter").WithMessage("Starters must have EventNumber = 1.");
        }
    }
}