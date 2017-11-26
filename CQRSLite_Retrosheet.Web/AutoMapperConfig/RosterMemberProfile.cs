using AutoMapper;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using System;

namespace CQRSLite_RS_Core.Web2.Commands.AutoMapperConfig
{
    public class RosterMemberProfile : Profile
    {
        public RosterMemberProfile()
        {
            CreateMap<CreateRosterMemberRequest, CreateRosterMemberCommand>()
                .ConstructUsing(x => new CreateRosterMemberCommand(Guid.NewGuid(), x.Year, x.TeamCode, x.PlayerId, x.LastName, x.FirstName, x.Bats, x.Throws));

            CreateMap<RosterMemberCreatedEvent, RosterMemberRM>();
        }
    }
}
