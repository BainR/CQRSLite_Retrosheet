using CQRSlite.Domain;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.WriteModel
{
    public class BaseballPlay : AggregateRoot
    {
        private String _RetrosheetGameId;
        private int _EventNumber;
        private int _LineupChangeSequence;
        private string _EventText;
        private string _Batter;
        private string _CountOnBatter;
        private string _Pitches;
        private BaseballPlayDetails _Details;

        private BaseballPlay() { }

        public BaseballPlay(Guid id, string retrosheetGameId, int eventNumber, int lineupChangeSequence, string eventText,
            string batter, string countOnBatter, string pitches, BaseballPlayDetails details)
        {
            Id = id;
            _RetrosheetGameId = retrosheetGameId;
            _EventNumber = eventNumber;
            _LineupChangeSequence = lineupChangeSequence;
            _EventText = eventText;
            _Batter = batter;
            _CountOnBatter = countOnBatter;
            _Pitches = pitches;
            _Details = details;

            ApplyChange(new BaseballPlayCreatedEvent(id, retrosheetGameId, eventNumber, lineupChangeSequence, eventText, batter, countOnBatter, pitches, details));
        }
    }
}
