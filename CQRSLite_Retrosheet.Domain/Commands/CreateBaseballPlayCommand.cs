using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class CreateBaseballPlayCommand : BaseCommand
    {
        public readonly string RetrosheetGameId;
        public readonly int EventNumber;
        public readonly int LineupChangeSequence;
        public readonly int Inning;
        public readonly int TeamAtBat; // 0 = visitor, 1 = home
        public readonly string Batter;
        public readonly string CountOnBatter; // could be ??, otherwise two digis, balls followed by strikes, some data has empty string
        public readonly string Pitches; // could be empty
        public readonly string EventText;
        public readonly bool LastPlay;
        public readonly BaseballPlayDetails Details;

        public CreateBaseballPlayCommand(Guid id, string retrosheetGameId, int eventNumber, int lineupChangeSequence, int inning, int teamAtBat, 
            string batter, string countOnBatter, string pitches, string eventText, bool lastPlay, BaseballPlayDetails details)
        {
            Id = id;
            RetrosheetGameId = retrosheetGameId;
            EventNumber = eventNumber;
            LineupChangeSequence = lineupChangeSequence;
            Inning = inning;
            TeamAtBat = teamAtBat;
            Batter = batter;
            CountOnBatter = countOnBatter;
            Pitches = pitches;
            EventText = eventText;
            LastPlay = lastPlay;
            Details = details;
        }
    }
}
