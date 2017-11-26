using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.Events
{
    public class LineupChangeCreatedEvent : BaseEvent
    {
        public readonly string RetrosheetGameId;
        public readonly short EventNumber;
        public readonly short Sequence;
        public bool IsStarter;
        public readonly string PlayerId;
        public readonly string Name;
        public readonly byte Team;
        public readonly byte BattingOrder;
        public readonly byte FieldPosition;
        public readonly bool LastLineupChange;
        public readonly LineupRM PreviousBattingOrder;

        public LineupChangeCreatedEvent(Guid id, string retrosheetGameId, short eventNumber, short sequence, bool isStarter, string playerId,
            string name, byte team, byte battingOrder, byte fieldPosition, bool lastLineupChange, LineupRM previousBattingOrder)
        {
            Id = id;
            RetrosheetGameId = retrosheetGameId;
            EventNumber = eventNumber;
            Sequence = sequence;
            IsStarter = isStarter;
            PlayerId = playerId;
            Name = name;
            Team = team;
            BattingOrder = battingOrder;
            FieldPosition = fieldPosition;
            LastLineupChange = lastLineupChange;
            PreviousBattingOrder = previousBattingOrder;
        }
    }
}
