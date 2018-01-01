using AutoMapper;
using CQRSLite_Retrosheet.Domain.Commands;
using CQRSLite_Retrosheet.Domain.Events;
using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using System;
using System.Text.RegularExpressions;

namespace CQRSLite_Retrosheet.Web.AutoMapperConfig
{
    public class BaseballPlayProfile : Profile
    {
        public BaseballPlayProfile()
        {
            CreateMap<CreateBaseballPlayRequest, CreateBaseballPlayCommand>()
                .ConstructUsing(x => new CreateBaseballPlayCommand(Guid.NewGuid(), x.RetrosheetGameId, x.EventNumber, x.LineupChangeSequence,
                x.Inning, x.TeamAtBat, x.Batter, x.CountOnBatter, x.Pitches, x.EventText, x.LastPlay, x.Details));

            CreateMap<BaseballPlayCreatedEvent, BaseballPlayRM>()
                .ConvertUsing(x => CopyDetails(x));
        }

        private BaseballPlayRM CopyDetails(BaseballPlayCreatedEvent source)
        {
            BaseballPlayRM play = new BaseballPlayRM();

            play.Inning = source.Details.Inning;
            play.TeamAtBat = source.Details.TeamAtBat;
            play.IsBottomHalf = source.Details.IsBottomHalf;
            play.BattingOrder = source.Details.BattingOrder;
            play.RetrosheetGameId = source.RetrosheetGameId;
            play.EventNumber = source.EventNumber;
            play.LineupChangeSequence = source.LineupChangeSequence;
            play.EventText = source.EventText;
            play.BasicPlay = source.Details.BasicPlay;
            play.Modifier = source.Details.Modifier;
            play.Batter = source.Batter;
            // CountOnBatter should be ?? if it is not an actual count.  Source data commonly does not conform to this convention, but I don't want this to be a fatal error. 
            play.Balls = Regex.IsMatch(source.CountOnBatter, "^[0-9]{2}$") ? int.Parse(source.CountOnBatter[0].ToString()) : (int?)null;
            play.Strikes = Regex.IsMatch(source.CountOnBatter, "^[0-9]{2}$") ? int.Parse(source.CountOnBatter[1].ToString()) : (int?)null;
            play.Pitches = source.Pitches;
            play.Runner1 = source.Details.Runner1;
            play.Runner2 = source.Details.Runner2;
            play.Runner3 = source.Details.Runner3;
            play.RunnerB = source.Details.RunnerB;
            play.Runner1Out = source.Details.Runner1Out;
            play.Runner2Out = source.Details.Runner2Out;
            play.Runner3Out = source.Details.Runner3Out;
            play.RunnerBOut = source.Details.RunnerBOut;
            play.OutsOnPlay = source.Details.OutsOnPlay;
            play.BatterEvent = source.Details.BatterEvent;
            play.HitValue = source.Details.HitValue;
            play.R1Destination = source.Details.R1Destination;
            play.R2Destination = source.Details.R2Destination;
            play.R3Destination = source.Details.R3Destination;
            play.RBDestination = source.Details.RBDestination;
            play.RunsOnPlay = source.Details.RunsOnPlay;
            play.AtBat = source.Details.AtBat;
            play.SacHit = source.Details.SacHit;
            play.SacFly = source.Details.SacFly;
            play.WildPitch = source.Details.WildPitch;
            play.PassedBall = source.Details.PassedBall;
            play.Bunt = source.Details.Bunt;
            play.Foul = source.Details.Foul;
            play.StolenBaseR1 = source.Details.StolenBaseR1;
            play.StolenBaseR2 = source.Details.StolenBaseR2;
            play.StolenBaseR3 = source.Details.StolenBaseR3;
            play.CaughtStealingR1 = source.Details.CaughtStealingR1;
            play.CaughtStealingR2 = source.Details.CaughtStealingR2;
            play.CaughtStealingR3 = source.Details.CaughtStealingR3;
            play.PickOffR1 = source.Details.PickOffR1;
            play.PickOffR2 = source.Details.PickOffR2;
            play.PickOffR3 = source.Details.PickOffR3;
            play.PlayOnBatter = source.Details.PlayOnBatter;
            play.PlayOnR1 = source.Details.PlayOnR1;
            play.PlayOnR2 = source.Details.PlayOnR2;
            play.PlayOnR3 = source.Details.PlayOnR3;
            play.EventType = source.Details.EventType;
            play.RBIB = source.Details.RBIB;
            play.RBI1 = source.Details.RBI1;
            play.RBI2 = source.Details.RBI2;
            play.RBI3 = source.Details.RBI3;
            play.RBI = source.Details.RBI;

            play.StartOfHalfInning = source.Details.StartOfHalfInning;
            play.EndOfHalfInning = source.Details.EndOfHalfInning;
            play.EndOfGame = source.Details.EndOfGame;
            play.VisitorNextBatter = source.Details.VisitorNextBatter;
            play.HomeNextBatter = source.Details.HomeNextBatter;
            play.StartOfPlay_Outs = source.Details.StartOfPlay.Outs;
            play.StartOfPlay_VisitorScore = source.Details.StartOfPlay.VisitorScore;
            play.StartOfPlay_HomeScore = source.Details.StartOfPlay.HomeScore;
            play.StartOfPlay_RunnerOnFirst = source.Details.StartOfPlay.RunnerOnFirst;
            play.StartOfPlay_Runner1BO = source.Details.StartOfPlay.Runner1BO;
            play.StartOfPlay_RunnerOnSecond = source.Details.StartOfPlay.RunnerOnSecond;
            play.StartOfPlay_Runner2BO = source.Details.StartOfPlay.Runner2BO;
            play.StartOfPlay_RunnerOnThird = source.Details.StartOfPlay.RunnerOnThird;
            play.StartOfPlay_Runner3BO = source.Details.StartOfPlay.Runner3BO;
            play.EndOfPlay_Outs = source.Details.EndOfPlay.Outs;
            play.EndOfPlay_VisitorScore = source.Details.EndOfPlay.VisitorScore;
            play.EndOfPlay_HomeScore = source.Details.EndOfPlay.HomeScore;
            play.EndOfPlay_RunnerOnFirst = source.Details.EndOfPlay.RunnerOnFirst;
            play.EndOfPlay_Runner1BO = source.Details.EndOfPlay.Runner1BO;
            play.EndOfPlay_RunnerOnSecond = source.Details.EndOfPlay.RunnerOnSecond;
            play.EndOfPlay_Runner2BO = source.Details.EndOfPlay.Runner2BO;
            play.EndOfPlay_RunnerOnThird = source.Details.EndOfPlay.RunnerOnThird;
            play.EndOfPlay_Runner3BO = source.Details.EndOfPlay.Runner3BO;

            return play;
        }
    }
}
