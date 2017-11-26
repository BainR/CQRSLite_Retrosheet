using AutoMapper;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using System;

namespace CQRSLite_Retrosheet.Web.AutoMapperConfig
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<CreateTeamRequest, CreateTeamCommand>()
                .ConstructUsing(x => new CreateTeamCommand(Guid.NewGuid(), x.Year, x.TeamCode, x.League, x.Home, x.Name));

            CreateMap<TeamCreatedEvent, TeamRM>();
        }
    }
}
