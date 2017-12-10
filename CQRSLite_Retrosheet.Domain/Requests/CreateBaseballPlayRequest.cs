using CQRSLite_Retrosheet.Domain.ReadModel;
using FluentValidation;
using System.Linq;

namespace CQRSLite_Retrosheet.Domain.Requests
{
    public class CreateBaseballPlayRequest
    {
        public BaseballPlayDetails Details { get; set; }

        public string RetrosheetGameId { get; set; }
        public int EventNumber { get; set; }
        public int LineupChangeSequence { get; set; }
        public int Inning { get; set; }
        public int TeamAtBat { get; set; } // 0 = visitor, 1 = home
        public string Batter { get; set; }
        public string CountOnBatter { get; set; } // could be ??, otherwise two digis, balls followed by strikes
        public string Pitches { get; set; } // could be empty
        public string EventText { get; set; }
        public bool LastPlay { get; set; }
    }

    public class CreateBaseballPlayRequestValidator : AbstractValidator<CreateBaseballPlayRequest>
    {
        public CreateBaseballPlayRequestValidator()
        {
            // TODO - use WithName in place of Details

            RuleFor(x => x.RetrosheetGameId).NotNull().NotEmpty().Length(12).WithMessage("RetrosheetGameId must have exactly 12 characters.");
            RuleFor(x => x.EventNumber).NotNull().LessThan(1000).GreaterThan(0).WithMessage("EventNumber must be between 1 and 999 inclusive.");
            //RuleFor(x => MakeId(x)).Must(x => !baseballPlayRepo.Exists(x)).WithName("BaseballPlay").WithMessage("A BaseballPlay with this ID already exists.");
            RuleFor(x => x.EventText).NotNull().NotEmpty().Length(1, 100).WithMessage("EventText must have between 1 and 100 characters inclusive.");
            //RuleFor(x => x.Details).Must(x => CheckEventNumber(x)).WithMessage("Invalid event number.");
            RuleFor(x => x.Details).Must(x => x.OutsOnPlay <= 3).WithMessage("No more than 3 outs per half inning.");

            RuleFor(x => x.Details).Must(x => !(x.HitValue > 0 && x.Foul)).WithMessage("Foul balls can't be hits.").WithSeverity(Severity.Warning); // Fixed, but fixes lost?
            RuleFor(x => x.Details).Must(x => CheckRunnerExists(x)).WithMessage("Base runner not on base at start of play.");
            RuleFor(x => x.Details).Must(x => CheckOneRunnerPerBag(x)).WithMessage("Max 1 runner per bag at end of play.");
            RuleFor(x => x.Details).Must(x => CheckPassedRunners(x)).WithMessage("Runners can't pass other runners.");
            RuleFor(x => x.Details).Must(x => x.EventText != "Unknown event").WithMessage("Unable to parse event text.");

            RuleFor(x => x).Must(x => CheckInning(x)).WithName("Inning").WithMessage("Wrong Inning");
            RuleFor(x => x).Must(x => CheckTeamAtBat(x)).WithName("TeamAtBat").WithMessage("Wrong TeamAtBat");
            RuleFor(x => x.CountOnBatter).Must(CheckCountOnBatter).WithMessage("Invalid count on batter.").WithSeverity(Severity.Warning); // This is violated too often to track for a non-fatal error.

            RuleFor(x => x).Must(x => (x.Details.EndOfGame && x.LastPlay) || !x.LastPlay).WithName("LastPlay").WithMessage("Unexpected end of game.");

            // Future
            // Verify batter matches lineup
            // Verify Count matches pitches
        }

        private string MakeId(CreateBaseballPlayRequest baseballPlay)
        {
            return baseballPlay.RetrosheetGameId + "_" + baseballPlay.EventNumber.ToString("000");
        }

        private bool CheckRunnerExists(BaseballPlayDetails Details)
        {
            // runners at end of play must have been runners at start of play
            if (Details.R1Destination.HasValue && !Details.StartOfPlay.RunnerOnFirst)
                return false;

            if (Details.R2Destination.HasValue && !Details.StartOfPlay.RunnerOnSecond)
                return false;

            if (Details.R3Destination.HasValue && !Details.StartOfPlay.RunnerOnThird)
                return false;

            return true;
        }

        private bool CheckOneRunnerPerBag(BaseballPlayDetails Details)
        {
            // max of 1 runner per bag at end of play, except for runners left on base at end of half inning
            if (!Details.EndOfHalfInning && ((Details.RBDestination == 1 ? 1 : 0) + (Details.R1Destination == 1 ? 1 : 0) + (Details.R2Destination == 1 ? 1 : 0) + (Details.R3Destination == 1 ? 1 : 0) > 1))
                return false;

            if (!Details.EndOfHalfInning && ((Details.RBDestination == 2 ? 1 : 0) + (Details.R1Destination == 2 ? 1 : 0) + (Details.R2Destination == 2 ? 1 : 0) + (Details.R3Destination == 2 ? 1 : 0) > 1))
                return false;

            if (!Details.EndOfHalfInning && ((Details.RBDestination == 3 ? 1 : 0) + (Details.R1Destination == 3 ? 1 : 0) + (Details.R2Destination == 3 ? 1 : 0) + (Details.R3Destination == 3 ? 1 : 0) > 1))
                return false;

            return true;
        }

        private bool CheckPassedRunners(BaseballPlayDetails Details)
        {
            // runners can't pass other runners
            if ((Details.RBDestination.HasValue ? (Details.RBDestination > 4 ? 4 : Details.RBDestination) : 0) > (Details.R1Destination.HasValue && Details.R1Destination > 0 ? Details.R1Destination : 6))
                return false;

            if ((Details.RBDestination.HasValue ? (Details.RBDestination > 4 ? 4 : Details.RBDestination) : 0) > (Details.R2Destination.HasValue && Details.R2Destination > 0 ? Details.R2Destination : 6))
                return false;

            if ((Details.RBDestination.HasValue ? (Details.RBDestination > 4 ? 4 : Details.RBDestination) : 0) > (Details.R3Destination.HasValue && Details.R3Destination > 0 ? Details.R3Destination : 6))
                return false;

            if ((Details.R1Destination.HasValue ? (Details.R1Destination > 4 ? 4 : Details.R1Destination) : 0) > (Details.R2Destination.HasValue && Details.R2Destination > 0 ? Details.R2Destination : 6))
                return false;

            if ((Details.R1Destination.HasValue ? (Details.R1Destination > 4 ? 4 : Details.R1Destination) : 0) > (Details.R3Destination.HasValue && Details.R3Destination > 0 ? Details.R3Destination : 6))
                return false;

            if ((Details.R2Destination.HasValue ? (Details.R2Destination > 4 ? 4 : Details.R2Destination) : 0) > (Details.R3Destination.HasValue && Details.R3Destination > 0 ? Details.R3Destination : 6))
                return false;

            return true;
        }

        private bool CheckInning(CreateBaseballPlayRequest request)
        {
            return request.Inning == request.Details.Inning;
        }

        private bool CheckTeamAtBat(CreateBaseballPlayRequest request)
        {
            return (request.TeamAtBat == 0 ? "V" : request.TeamAtBat == 1 ? "H" : "") == request.Details.TeamAtBat;
        }

        private bool CheckCountOnBatter(string countOnBatter)
        {
            return countOnBatter.Length == 2 && (countOnBatter == "??" ||
                ((new char[] { '0', '1', '2', '3' }).Contains(countOnBatter[0])
                && (new char[] { '0', '1', '2' }).Contains(countOnBatter[1])));
        }
    }
}