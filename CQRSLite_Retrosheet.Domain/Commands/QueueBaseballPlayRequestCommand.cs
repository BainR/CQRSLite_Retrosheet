using CQRSLite_Retrosheet.Domain.Requests;
using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class QueueBaseballPlayRequestCommand : BaseCommand
    {
        public readonly CreateBaseballPlayRequest Request;

        public QueueBaseballPlayRequestCommand(Guid id, CreateBaseballPlayRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
