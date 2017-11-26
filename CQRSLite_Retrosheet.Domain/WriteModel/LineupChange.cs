using CQRSlite.Domain;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.WriteModel
{
    public class LineupChange : AggregateRoot
    {
        private string _RetrosheetGameId;
        private short _EventNumber;
        private short _Sequence;
        private bool _IsStarter;
        private string _PlayerId;
        private string _Name;
        private byte _Team;
        private byte _BattingOrder;
        private byte _FieldPosition;
        private bool _LastLineupChange;
        private LineupRM _PreviousBattingOrder;

        private LineupChange() { }

        public LineupChange(Guid id, string retrosheetGameId, short eventNumber, short sequence, bool isStarter, string playerId,
            string name, byte team, byte battingOrder, byte fieldPosition, bool lastLineupChange, LineupRM previousBattingOrder)
        {
            Id = id;
            _RetrosheetGameId = retrosheetGameId;
            _EventNumber = eventNumber;
            _Sequence = sequence;
            _IsStarter = isStarter;
            _PlayerId = playerId;
            _Name = name;
            _Team = team;
            _BattingOrder = battingOrder;
            _FieldPosition = fieldPosition;
            _LastLineupChange = lastLineupChange;
            _PreviousBattingOrder = previousBattingOrder;

            ApplyChange(new LineupChangeCreatedEvent(id, retrosheetGameId, eventNumber, sequence, isStarter, playerId,
            name, team, battingOrder, fieldPosition, lastLineupChange, previousBattingOrder));
        }
    }
}
