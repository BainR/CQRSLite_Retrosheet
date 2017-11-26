using System;

namespace CQRSLite_Retrosheet.Domain.Events
{
    public class GameSummaryCreatedEvent : BaseEvent
    {
        public readonly string RetrosheetGameId;
        public readonly string AwayTeam;
        public readonly string HomeTeam;
        public readonly bool UseDH;
        public readonly string ParkCode;
        public readonly string WinningPitcher;
        public readonly string LosingPitcher;
        public readonly string SavePitcher;
        public readonly bool HasValidationErrors;
        public readonly string GameDay;
        public readonly int? HomeTeamFinalScore;
        public readonly int? AwayTeamFinalScore;

        public GameSummaryCreatedEvent(Guid id, string retrosheetGameId, string awayTeam, string homeTeam, bool useDH, string parkCode,
            string winningPitcher, string losingPitcher, string savePitcher, bool hasValidationErrors, string gameday,
            int? homeTeamFinalScore, int? awayTeamFinalScore)
        {
            Id = id;
            RetrosheetGameId = retrosheetGameId;
            AwayTeam = awayTeam;
            HomeTeam = homeTeam;
            UseDH = useDH;
            ParkCode = parkCode;
            WinningPitcher = winningPitcher;
            LosingPitcher = losingPitcher;
            SavePitcher = savePitcher;
            HasValidationErrors = hasValidationErrors;
            GameDay = gameday;
            HomeTeamFinalScore = homeTeamFinalScore;
            AwayTeamFinalScore = awayTeamFinalScore;
        }
    }
}
