using CQRSLite_Retrosheet.Domain.Requests;
using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class QueueGameSummaryRequestCommand : BaseCommand
    {
        public readonly CreateGameSummaryRequest Request;

        public QueueGameSummaryRequestCommand(Guid id, CreateGameSummaryRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
