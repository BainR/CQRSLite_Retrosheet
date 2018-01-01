using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class CreateGameSummaryCommand : BaseCommand
    {
        public readonly string RetrosheetGameId;
        public readonly string AwayTeam;
        public readonly string HomeTeam;
        public readonly bool UseDH;
        public readonly bool HomeTeamBatsFirst;
        public readonly string ParkCode;
        public readonly string WinningPitcher;
        public readonly string LosingPitcher;
        public readonly string SavePitcher;
        public readonly bool HasValidationErrors;
        public readonly string GameDay;
        public readonly int? HomeTeamFinalScore;
        public readonly int? AwayTeamFinalScore;

        public CreateGameSummaryCommand(Guid id, string retrosheetGameId, string awayTeam, string homeTeam, bool useDH,
            bool homeTeamBatsfirst, string parkCode, string winningPitcher, string losingPitcher, string savePitcher,
            bool hasValidationErrors, string gameDay, int? homeTeamFinalScore, int? awayTeamFinalScore)
        {
            Id = id;
            RetrosheetGameId = retrosheetGameId;
            AwayTeam = awayTeam;
            HomeTeam = homeTeam;
            UseDH = useDH;
            HomeTeamBatsFirst = homeTeamBatsfirst;
            ParkCode = parkCode;
            WinningPitcher = winningPitcher;
            LosingPitcher = losingPitcher;
            SavePitcher = savePitcher;
            HasValidationErrors = hasValidationErrors;
            GameDay = gameDay;
            HomeTeamFinalScore = homeTeamFinalScore;
            AwayTeamFinalScore = awayTeamFinalScore;
        }
    }
}
