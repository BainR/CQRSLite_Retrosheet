using CQRSLite_Retrosheet.Domain.ReadModel;
using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class QueueBaseballPlayRMCommand : BaseCommand
    {
        public readonly BaseballPlayRM BaseballPlay;

        public QueueBaseballPlayRMCommand(Guid id, BaseballPlayRM baseballPlay)
        {
            Id = id;
            BaseballPlay = baseballPlay;
        }
    }
}
