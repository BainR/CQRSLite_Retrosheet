using System;

namespace CQRSLite_Retrosheet.Domain.ReadModel
{
    public class GameSummaryRM
    {
        public string RetrosheetGameId { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeam { get; set; }
        public bool UseDH { get; set; }
        public bool HomeTeamBatsFirst { get; set; }
        public string ParkCode { get; set; }
        public string WinningPitcher { get; set; }
        public string LosingPitcher { get; set; }
        public string SavePitcher { get; set; }
        public bool HasValidationErrors { get; set; }
        public DateTime GameDay { get; set; }
        public int? HomeTeamFinalScore { get; set; }
        public int? AwayTeamFinalScore { get; set; }
    }
}
