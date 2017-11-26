using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.Events
{
    public class BaseballPlayCreatedEvent : BaseEvent
    {
        public readonly String RetrosheetGameId;
        public readonly int EventNumber;
        public readonly int LineupChangeSequence;
        public readonly string EventText;
        public readonly string Batter;
        public readonly string CountOnBatter;
        public readonly string Pitches;
        public readonly BaseballPlayDetails Details;

        public BaseballPlayCreatedEvent(Guid id, string retrosheetGameId, int eventNumber, int lineupChangeSequence, string eventText,
            string batter, string countOnBatter, string pitches, BaseballPlayDetails details)
        {
            Id = id;
            RetrosheetGameId = retrosheetGameId;
            EventNumber = eventNumber;
            LineupChangeSequence = lineupChangeSequence;
            EventText = eventText;
            Batter = batter;
            CountOnBatter = countOnBatter;
            Pitches = pitches;
            Details = details;
        }
    }
}
