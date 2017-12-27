using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class QueueGameCompletedCommand : BaseCommand
    {
        public readonly string RetrosheetGameId;
        public int HomeTeamFinalScore;
        public int AwayTeamFinalScore;
        public bool HomeTeamBatsFirst;

        public QueueGameCompletedCommand(Guid id, string retrosheetGameid, int homeTeamFinalScore, int awayTeamFinalScore, bool homeTeamBatsFirst)
        {
            Id = id;
            RetrosheetGameId = retrosheetGameid;
            HomeTeamFinalScore = homeTeamFinalScore;
            AwayTeamFinalScore = awayTeamFinalScore;
            HomeTeamBatsFirst = homeTeamBatsFirst;
        }
    }
}