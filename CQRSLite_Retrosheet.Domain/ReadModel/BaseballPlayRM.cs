using System;

namespace CQRSLite_Retrosheet.Domain.ReadModel
{
    public class BaseballPlayRM
    {
        public int Inning { get; set; }
        public string TeamAtBat { get; set; }
        public bool IsBottomHalf { get; set; }
        public int BattingOrder { get; set; }
        public string RetrosheetGameId { get; set; }
        public int EventNumber { get; set; }
        public int LineupChangeSequence { get; set; }
        public string EventText { get; set; }
        public string BasicPlay { get; set; }
        public string Modifier { get; set; }
        public string Batter { get; set; }
        public int? Balls { get; set; }
        public int? Strikes { get; set; }
        public string Pitches { get; set; }
        public string Runner1 { get; set; }
        public string Runner2 { get; set; }
        public string Runner3 { get; set; }
        public string RunnerB { get; set; }
        public bool Runner1Out { get; set; }
        public bool Runner2Out { get; set; }
        public bool Runner3Out { get; set; }
        public bool RunnerBOut { get; set; }
        public int OutsOnPlay { get; set; }
        public bool BatterEvent { get; set; }
        public int HitValue { get; set; }
        public int? R1Destination { get; set; }
        public int? R2Destination { get; set; }
        public int? R3Destination { get; set; }
        public int? RBDestination { get; set; }
        public int RunsOnPlay { get; set; }
        public bool AtBat { get; set; }
        public bool SacHit { get; set; }
        public bool SacFly { get; set; }
        public bool WildPitch { get; set; }
        public bool PassedBall { get; set; }
        public bool Bunt { get; set; }
        public bool Foul { get; set; }
        public bool StolenBaseR1 { get; set; }
        public bool StolenBaseR2 { get; set; }
        public bool StolenBaseR3 { get; set; }
        public bool CaughtStealingR1 { get; set; }
        public bool CaughtStealingR2 { get; set; }
        public bool CaughtStealingR3 { get; set; }
        public bool PickOffR1 { get; set; }
        public bool PickOffR2 { get; set; }
        public bool PickOffR3 { get; set; }
        public string PlayOnBatter { get; set; }
        public string PlayOnR1 { get; set; }
        public string PlayOnR2 { get; set; }
        public string PlayOnR3 { get; set; }
        public String EventType { get; set; }
        public bool? RBIB { get; set; }
        public bool? RBI1 { get; set; }
        public bool? RBI2 { get; set; }
        public bool? RBI3 { get; set; }
        public int RBI { get; set; }

        public bool StartOfHalfInning { get; set; }
        public bool EndOfHalfInning { get; set; }
        public bool EndOfGame { get; set; }
        public int VisitorNextBatter { get; set; }
        public int HomeNextBatter { get; set; }

        public int StartOfPlay_Outs { get; set; }
        public int StartOfPlay_VisitorScore { get; set; }
        public int StartOfPlay_HomeScore { get; set; }
        public bool StartOfPlay_RunnerOnFirst { get; set; }
        public byte? StartOfPlay_Runner1BO { get; set; }
        public bool StartOfPlay_RunnerOnSecond { get; set; }
        public byte? StartOfPlay_Runner2BO { get; set; }
        public bool StartOfPlay_RunnerOnThird { get; set; }
        public byte? StartOfPlay_Runner3BO { get; set; }
        public int EndOfPlay_Outs { get; set; }
        public int EndOfPlay_VisitorScore { get; set; }
        public int EndOfPlay_HomeScore { get; set; }
        public bool EndOfPlay_RunnerOnFirst { get; set; }
        public byte? EndOfPlay_Runner1BO { get; set; }
        public bool EndOfPlay_RunnerOnSecond { get; set; }
        public byte? EndOfPlay_Runner2BO { get; set; }
        public bool EndOfPlay_RunnerOnThird { get; set; }
        public byte? EndOfPlay_Runner3BO { get; set; }
    }
}
