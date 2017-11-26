using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using System;

namespace CQRSLite_Retrosheet.Domain.Commands
{
    public class QueueLineupChangeRequestCommand : BaseCommand
    {
        public readonly CreateLineupChangeRequest Request;

        public QueueLineupChangeRequestCommand(Guid id, CreateLineupChangeRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}