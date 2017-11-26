using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class QueueLineupRMCommand : BaseCommand
    {
        public readonly LineupRM BattingOrder;

        public QueueLineupRMCommand(Guid id, LineupRM battingOrder)
        {
            Id = id;
            BattingOrder = battingOrder;
        }
    }
}
