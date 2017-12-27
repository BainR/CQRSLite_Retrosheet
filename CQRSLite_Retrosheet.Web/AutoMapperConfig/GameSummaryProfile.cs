using AutoMapper;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using System;
using System.Globalization;

namespace CQRSLite_Retrosheet.Web.AutoMapperConfig
{
    public class GameSummaryProfile : Profile
    {
        public GameSummaryProfile()
        {
            CreateMap<CreateGameSummaryRequest, CreateGameSummaryCommand>()
                .ConstructUsing(x => new CreateGameSummaryCommand(Guid.NewGuid(), x.RetrosheetGameId, x.AwayTeam, x.HomeTeam, bool.Parse(x.UseDH), x.HomeTeamBatsFirst, 
                    x.ParkCode, x.WinningPitcher, x.LosingPitcher, x.SavePitcher, (bool)x.HasValidationErrors, x.GameDay, x.HomeTeamFinalScore, x.AwayTeamFinalScore));

            CreateMap<GameSummaryCreatedEvent, GameSummaryRM>()
                .ForMember(dest => dest.GameDay, opt => opt.MapFrom(src => DateTime.ParseExact(src.GameDay, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)));
        }
    }
}
