using CQRSlite.Domain;
using CQRSLite_Retrosheet.Domain.Events;
using System;

namespace CQRSLite_Retrosheet.Domain.WriteModel
{
    public class GameSummary : AggregateRoot
    {
        private string _RetrosheetGameId;
        private string _AwayTeam;
        private string _HomeTeam;
        private bool _UseDH;
        private string _ParkCode;
        private string _WinningPitcher;
        private string _LosingPitcher;
        private string _SavePitcher;
        private bool _HasValidationErrors;
        private string _GameDay;
        private int? _HomeTeamFinalScore;
        private int? _AwayTeamFinalScore;
        private bool _HomeTeamBatsFirst;

        private GameSummary() { }

        public GameSummary(Guid id, string retrosheetGameId, string awayTeam, string homeTeam, bool useDH, string parkCode,
            string winningPitcher, string losingPitcher, string savePitcher, bool hasValidationErrors, string gameDay,
            int? homeTeamFinalScore, int? awayTeamFinalScore, bool homeTeamBatsFirst)
        {
            Id = id;
            _RetrosheetGameId = retrosheetGameId;
            _AwayTeam = awayTeam;
            _HomeTeam = homeTeam;
            _UseDH = useDH;
            _ParkCode = parkCode;
            _WinningPitcher = winningPitcher;
            _LosingPitcher = losingPitcher;
            _SavePitcher = savePitcher;
            _HasValidationErrors = hasValidationErrors;
            _GameDay = gameDay;
            _HomeTeamFinalScore = homeTeamFinalScore;
            _AwayTeamFinalScore = awayTeamFinalScore;
            _HomeTeamBatsFirst = homeTeamBatsFirst;

            ApplyChange(new GameSummaryCreatedEvent(id, retrosheetGameId, awayTeam, homeTeam, useDH, parkCode, 
                winningPitcher, losingPitcher, savePitcher, hasValidationErrors,
                gameDay, homeTeamFinalScore, awayTeamFinalScore, homeTeamBatsFirst));
        }
    }
}
