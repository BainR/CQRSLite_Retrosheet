using AutoMapper;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using System;

namespace CQRSLite_Retrosheet.Web.AutoMapperConfig
{
    public class LineupChangeProfile : Profile
    {
        public LineupChangeProfile()
        {
            CreateMap<CreateLineupChangeRequest, CreateLineupChangeCommand>()
                .ConstructUsing(x => new CreateLineupChangeCommand(Guid.NewGuid(), x.RetrosheetGameId, x.EventNumber, x.Sequence,
                x.IsStarter, x.PlayerId, x.Name, x.Team, x.BattingOrder, x.FieldPosition, x.LastLineupChange, x.PreviousBattingOrder));

            CreateMap<LineupChangeCreatedEvent, LineupChangeRM>();

            CreateMap<LineupChangeCreatedEvent, LineupRM>().ConvertUsing(x => AdjustBattingOrder(x));
        }

        private LineupRM AdjustBattingOrder(LineupChangeCreatedEvent source)
        {
            LineupRM bo = source.PreviousBattingOrder;

            if (bo == null)
            {
                bo = new LineupRM();
            }

            bo.EventNumber = source.EventNumber;
            bo.Sequence = source.Sequence;
            bo.RetrosheetGameId = source.RetrosheetGameId;

            if (source.BattingOrder == 0 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO0_PlayerId == source.PlayerId && bo.Home_BO0_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO0_PlayerId = source.PlayerId;
                bo.Home_BO0_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 1 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO1_PlayerId == source.PlayerId && bo.Home_BO1_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO1_PlayerId = source.PlayerId;
                bo.Home_BO1_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 2 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO2_PlayerId == source.PlayerId && bo.Home_BO2_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO2_PlayerId = source.PlayerId;
                bo.Home_BO2_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 3 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO3_PlayerId == source.PlayerId && bo.Home_BO3_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO3_PlayerId = source.PlayerId;
                bo.Home_BO3_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 4 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO4_PlayerId == source.PlayerId && bo.Home_BO4_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO4_PlayerId = source.PlayerId;
                bo.Home_BO4_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 5 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO5_PlayerId == source.PlayerId && bo.Home_BO5_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO5_PlayerId = source.PlayerId;
                bo.Home_BO5_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 6 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO6_PlayerId == source.PlayerId && bo.Home_BO6_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO6_PlayerId = source.PlayerId;
                bo.Home_BO6_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 7 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO7_PlayerId == source.PlayerId && bo.Home_BO7_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO7_PlayerId = source.PlayerId;
                bo.Home_BO7_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 8 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO8_PlayerId == source.PlayerId && bo.Home_BO8_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO8_PlayerId = source.PlayerId;
                bo.Home_BO8_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 9 && source.Team == 1)
            {
                if (source.IsStarter == false && bo.Home_BO9_PlayerId == source.PlayerId && bo.Home_BO9_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Home_BO0_PlayerId = null;
                    bo.Home_BO0_FieldPosition = 0;
                }
                bo.Home_BO9_PlayerId = source.PlayerId;
                bo.Home_BO9_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 0 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO0_PlayerId == source.PlayerId && bo.Away_BO0_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO0_PlayerId = source.PlayerId;
                bo.Away_BO0_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 1 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO1_PlayerId == source.PlayerId && bo.Away_BO1_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO1_PlayerId = source.PlayerId;
                bo.Away_BO1_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 2 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO2_PlayerId == source.PlayerId && bo.Away_BO2_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO2_PlayerId = source.PlayerId;
                bo.Away_BO2_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 3 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO3_PlayerId == source.PlayerId && bo.Away_BO3_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO3_PlayerId = source.PlayerId;
                bo.Away_BO3_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 4 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO4_PlayerId == source.PlayerId && bo.Away_BO4_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO4_PlayerId = source.PlayerId;
                bo.Away_BO4_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 5 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO5_PlayerId == source.PlayerId && bo.Away_BO5_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO5_PlayerId = source.PlayerId;
                bo.Away_BO5_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 6 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO6_PlayerId == source.PlayerId && bo.Away_BO6_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO6_PlayerId = source.PlayerId;
                bo.Away_BO6_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 7 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO7_PlayerId == source.PlayerId && bo.Away_BO7_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO7_PlayerId = source.PlayerId;
                bo.Away_BO7_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 8 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO8_PlayerId == source.PlayerId && bo.Away_BO8_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO8_PlayerId = source.PlayerId;
                bo.Away_BO8_FieldPosition = source.FieldPosition;
            }
            else if (source.BattingOrder == 9 && source.Team == 0)
            {
                if (source.IsStarter == false && bo.Away_BO9_PlayerId == source.PlayerId && bo.Away_BO9_FieldPosition == 10 && source.FieldPosition != 10)
                {
                    bo.Away_BO0_PlayerId = null;
                    bo.Away_BO0_FieldPosition = 0;
                }
                bo.Away_BO9_PlayerId = source.PlayerId;
                bo.Away_BO9_FieldPosition = source.FieldPosition;
            }

            return bo;
        }
    }
}
