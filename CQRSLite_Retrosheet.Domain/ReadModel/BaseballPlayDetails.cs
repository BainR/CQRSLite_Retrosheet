using System.Text.RegularExpressions;

namespace CQRSLite_Retrosheet.Domain.ReadModel
{
    public class BaseballPlayDetails
    {
        #region Properties
        public int Inning { get; set; }
        public string TeamAtBat { get; set; }
        public int BattingOrder { get; set; }
        public string RetrosheetGameId { get; set; }
        public int EventNumber { get; set; }
        public string EventText { get; set; }
        public string BasicPlay { get; set; }
        public string Modifier { get; set; }
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
        public string EventType { get; set; }
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
        public GameSituation StartOfPlay { get; set; }
        public GameSituation EndOfPlay { get; set; }

        #endregion

        public BaseballPlayDetails(BaseballPlayRM previousEvent, string RetrosheetGameId, int EventNumber, string EventText, bool LastPlay)
        {
            setDetails(previousEvent, RetrosheetGameId, EventNumber, EventText, LastPlay);
        }

        private void setDetails(BaseballPlayRM previousEvent, string RetrosheetGameId, int EventNumber, string EventText, bool LastPlay)
        {
            // TODO - add logic for games in which the home team bats first.  Validation rules will need changes too.  See SEA200709261

            this.RetrosheetGameId = RetrosheetGameId;
            this.EventNumber = EventNumber;
            this.EventText = EventText;

            StartOfPlay = new GameSituation();
            EndOfPlay = new GameSituation();

            if (previousEvent == null)
            {
                Inning = 1;
                StartOfPlay.Outs = 0;
                StartOfPlay.HomeScore = 0;
                StartOfPlay.VisitorScore = 0;
                StartOfPlay.RunnerOnFirst = false;
                StartOfPlay.Runner1BO = null;
                StartOfPlay.RunnerOnSecond = false;
                StartOfPlay.Runner2BO = null;
                StartOfPlay.RunnerOnThird = false;
                StartOfPlay.Runner3BO = null;
                StartOfHalfInning = true;
                TeamAtBat = "V";
                BattingOrder = 1;
                VisitorNextBatter = 2;
                HomeNextBatter = 1;
            }
            else if (previousEvent.EndOfHalfInning)
            {
                Inning = previousEvent.TeamAtBat == "H" ? previousEvent.Inning + 1 : previousEvent.Inning;
                StartOfPlay.Outs = 0;
                StartOfPlay.HomeScore = previousEvent.EndOfPlay_HomeScore;
                StartOfPlay.VisitorScore = previousEvent.EndOfPlay_VisitorScore;
                StartOfPlay.RunnerOnFirst = false;
                StartOfPlay.Runner1BO = null;
                StartOfPlay.RunnerOnSecond = false;
                StartOfPlay.Runner2BO = null;
                StartOfPlay.RunnerOnThird = false;
                StartOfPlay.Runner3BO = null;
                StartOfHalfInning = true;
                TeamAtBat = previousEvent.TeamAtBat == "V" ? "H" : "V";
                BattingOrder = previousEvent.TeamAtBat == "V" ? previousEvent.HomeNextBatter : previousEvent.VisitorNextBatter;
                VisitorNextBatter = previousEvent.TeamAtBat == "V" ? previousEvent.VisitorNextBatter : previousEvent.VisitorNextBatter + 1;
                VisitorNextBatter = VisitorNextBatter == 10 ? 1 : VisitorNextBatter;
                HomeNextBatter = previousEvent.TeamAtBat == "H" ? previousEvent.HomeNextBatter : previousEvent.HomeNextBatter + 1;
                HomeNextBatter = HomeNextBatter == 10 ? 1 : HomeNextBatter;
            }
            else
            {
                Inning = previousEvent.Inning;
                StartOfPlay.Outs = previousEvent.EndOfPlay_Outs; ;
                StartOfPlay.HomeScore = previousEvent.EndOfPlay_HomeScore;
                StartOfPlay.VisitorScore = previousEvent.EndOfPlay_VisitorScore;
                StartOfPlay.RunnerOnFirst = previousEvent.EndOfPlay_RunnerOnFirst;
                StartOfPlay.Runner1BO = previousEvent.EndOfPlay_Runner1BO;
                StartOfPlay.RunnerOnSecond = previousEvent.EndOfPlay_RunnerOnSecond;
                StartOfPlay.Runner2BO = previousEvent.EndOfPlay_Runner2BO;
                StartOfPlay.RunnerOnThird = previousEvent.EndOfPlay_RunnerOnThird;
                StartOfPlay.Runner3BO = previousEvent.EndOfPlay_Runner3BO;
                StartOfHalfInning = false;
                TeamAtBat = previousEvent.TeamAtBat;
                BattingOrder = previousEvent.TeamAtBat == "V" ? previousEvent.VisitorNextBatter : previousEvent.HomeNextBatter;
                VisitorNextBatter = previousEvent.TeamAtBat == "V" ? previousEvent.VisitorNextBatter + 1 : previousEvent.VisitorNextBatter;
                VisitorNextBatter = VisitorNextBatter == 10 ? 1 : VisitorNextBatter;
                HomeNextBatter = previousEvent.TeamAtBat == "H" ? previousEvent.HomeNextBatter + 1 : previousEvent.HomeNextBatter;
                HomeNextBatter = HomeNextBatter == 10 ? 1 : HomeNextBatter;
            }

            string input = EventText;
            string et = input.ToString();
            string bp = Regex.Match(et, @"(.*?)(?=\.|/|$)").Value;
            string mod = Regex.Match(et, @"(?<=/)(.*?)(?=\.|;|$)").Value;
            string r1 = Regex.Match(et, @"(?<=[\.;])1(.*?)(?=;|$)").Value;
            string r2 = Regex.Match(et, @"(?<=[\.;])2(.*?)(?=;|$)").Value;
            string r3 = Regex.Match(et, @"(?<=[\.;])3(.*?)(?=;|$)").Value;
            string rb = Regex.Match(et, @"(?<=[\.;])B(.*?)(?=;|$)").Value;

            bp = bp.Replace("#", "").Replace("!", "");
            r1 = r1.Replace("#", "").Replace("!", "");
            r2 = r2.Replace("#", "").Replace("!", "");
            r3 = r3.Replace("#", "").Replace("!", "");
            rb = rb.Replace("#", "").Replace("!", "");

            bool iwalk = Regex.IsMatch(bp, @"^I");
            bool walk = iwalk || Regex.IsMatch(bp, @"^W(?!P)");
            bool strikeOut = Regex.IsMatch(bp, @"^K");
            bool forceOut = Regex.IsMatch(mod, @"^FO");
            bool fieldersChoice = Regex.IsMatch(bp, @"^FC");
            bool hp = Regex.IsMatch(bp, @"^HP");
            bool s = Regex.IsMatch(bp, @"^S(?!B)");
            bool d = Regex.IsMatch(bp, @"^D(?!I)");
            bool t = Regex.IsMatch(bp, @"^T");
            bool hr = Regex.IsMatch(bp, @"^H(?!P)");
            bool sacHit = Regex.IsMatch(mod, @"^SH") || Regex.IsMatch(mod, @"\/SH");
            bool sacFly = Regex.IsMatch(mod, @"^SF") || Regex.IsMatch(mod, @"\/SF");
            bool bunt = sacHit || Regex.IsMatch(mod, @"(?<!COU|P)B(?!R|INT|OOT|S)");
            bool foul = Regex.IsMatch(et, @"7LDF|7LF|7LSF|5DF|5F|25F|2F|23F|3F|3DF|9LSF|9LF|9DF|FL|FLE|5SF|2RF|3SF|2LF|LMF|LDF|LF");
            bool wildPitch = Regex.IsMatch(et, @"WP");
            bool passedBall = Regex.IsMatch(et, @"PB");
            bool fielderInterference = Regex.IsMatch(bp, @"^C(?!S)");
            bool ballInPlay = Regex.IsMatch(bp, @"^(\d|E)");

            bool batterEvent = walk || hp || s || d || t || hr || ballInPlay
                || fielderInterference || forceOut || fieldersChoice || strikeOut;
            int hitValue = s ? 1 : d ? 2 : t ? 3 : hr ? 4 : 0;

            string bpr1 = "";
            if (Regex.IsMatch(bp, @"^\d") && Regex.IsMatch(bp, @"\(1\)"))
            {
                bpr1 = bp.Substring(0, Regex.Match(bp, @"\(1\)").Index);
                if (Regex.IsMatch(bpr1, @"\(.\)"))
                {
                    int i = Regex.Match(bpr1, @"(\(.\))(?!.*\(.\))").Index - 1;
                    bpr1 = (bpr1.Substring(i, 1) == bpr1.Substring(i + 4, 1) ? "" : bpr1.Substring(i, 1)) + bpr1.Substring(i + 4);
                }
                r1 = r1 == "" ? "1X2(" + bpr1 + ")" : r1;
            }
            else if (Regex.IsMatch(bp, @"POCS2|PO1|CS2|SB2"))
            {
                bpr1 = bp.Substring(Regex.Match(bp, @"POCS2|PO1|CS2|SB2").Index).Split(';')[0];
            }

            string bpr2 = "";
            if (Regex.IsMatch(bp, @"^\d") && Regex.IsMatch(bp, @"\(2\)"))
            {
                bpr2 = bp.Substring(0, Regex.Match(bp, @"\(2\)").Index);
                if (Regex.IsMatch(bpr2, @"\(.\)"))
                {
                    int i = Regex.Match(bpr2, @"(\(.\))(?!.*\(.\))").Index - 1;
                    bpr2 = (bpr2.Substring(i, 1) == bpr2.Substring(i + 4, 1) ? "" : bpr2.Substring(i, 1)) + bpr2.Substring(i + 4);
                }
                r2 = r2 == "" ? "2X3(" + bpr2 + ")" : r2;
            }
            else if (Regex.IsMatch(bp, @"POCS3|PO2|CS3|SB3"))
            {
                bpr2 = bp.Substring(Regex.Match(bp, @"POCS3|PO2|CS3|SB3").Index).Split(';')[0];
            }

            string bpr3 = "";
            if (Regex.IsMatch(bp, @"^\d") && Regex.IsMatch(bp, @"\(3\)"))
            {
                bpr3 = bp.Substring(0, Regex.Match(bp, @"\(3\)").Index);
                if (Regex.IsMatch(bpr3, @"\(.\)"))
                {
                    int i = Regex.Match(bpr3, @"(\(.\))(?!.*\(.\))").Index - 1;
                    bpr3 = (bpr3.Substring(i, 1) == bpr3.Substring(i + 4, 1) ? "" : bpr3.Substring(i, 1)) + bpr3.Substring(i + 4);
                }
                r3 = r3 == "" ? "3XH(" + bpr3 + ")" : r3;
            }
            else if (Regex.IsMatch(bp, @"POCSH|PO3|CSH|SBH"))
            {
                bpr3 = bp.Substring(Regex.Match(bp, @"POCSH|PO3|CSH|SBH").Index).Split(';')[0];
            }

            string bprb = "";
            if (Regex.IsMatch(bp, @"^\d") && (Regex.IsMatch(bp, @"\(B\)") || !Regex.IsMatch(bp, @"\(\d\)$")))
            {
                bprb = bp;
                if (Regex.IsMatch(bprb, @"\(B\)"))
                {
                    bprb = bprb.Substring(0, Regex.Match(bprb, @"\(B\)").Index);
                }
                if (Regex.IsMatch(bprb, @"\(.\)"))
                {
                    int i = Regex.Match(bprb, @"(\(.\))(?!.*\(.\))").Index - 1;
                    bprb = (bprb.Substring(i, 1) == bprb.Substring(i + 4, 1) ? "" : bprb.Substring(i, 1)) + bprb.Substring(i + 4);
                }
                if (string.IsNullOrEmpty(rb))
                {
                    rb = "BX1(" + bprb + ")";
                }
            }

            if (string.IsNullOrEmpty(rb) || rb.Length == 3)
            {
                if (string.IsNullOrEmpty(rb) && Regex.IsMatch(bp, @"^(\d|I|W(?!P)|S(?!B)|HP|C(?!S)|FC|E)"))
                {
                    rb = "B-1";
                }
                else if (string.IsNullOrEmpty(rb) && Regex.IsMatch(bp, @"^D(?!I)"))
                {
                    rb = "B-2";
                }
                else if (string.IsNullOrEmpty(rb) && Regex.IsMatch(bp, @"^T"))
                {
                    rb = "B-3";
                }
                else if (string.IsNullOrEmpty(rb) && Regex.IsMatch(bp, @"^H"))
                {
                    rb = "B-4";
                }
                else if (string.IsNullOrEmpty(rb) && Regex.IsMatch(bp, @"^K"))
                {
                    rb = Regex.Match(bp, @"(?<=K)[^+]+(?=[+])?").Value;
                    rb = rb == "" ? "BX1(2)" : "BX1(" + rb + ")";
                }
                else if (Regex.IsMatch(bp, @"^\d+E\d$") && rb.Length == 3)
                {
                    rb = rb.Replace("-", "X") + "(" + bp + ")";
                }
            }

            bool pickOffR1 = false;
            bool pickOffR2 = false;
            bool pickOffR3 = false;
            bool caughtStealingR1 = false;
            bool caughtStealingR2 = false;
            bool caughtStealingR3 = false;
            bool stolenBaseR1 = false;
            bool stolenBaseR2 = false;
            bool stolenBaseR3 = false;

            if (Regex.IsMatch(bp, @"(PO|CS|SB)"))
            {
                string rp = bp;
                if (Regex.IsMatch(bp, @"\+"))
                {
                    rp = bp.Split('+')[1];
                }
                foreach (string p in rp.Split(';'))
                {
                    if (Regex.IsMatch(p, @"^POCS2"))
                    {
                        pickOffR1 = true;
                        caughtStealingR1 = true;
                        r1 = (r1 == "" ? "1X2" : r1) + (r1.Length <= 3 ? p.Substring(5) : "");
                    }
                    else if (Regex.IsMatch(p, @"^POCS3"))
                    {
                        pickOffR2 = true;
                        caughtStealingR2 = true;
                        r2 = (r2 == "" ? "2X3" : r2) + (r2.Length <= 3 ? p.Substring(5) : "");
                    }
                    else if (Regex.IsMatch(p, @"^POCSH"))
                    {
                        pickOffR3 = true;
                        caughtStealingR3 = true;
                        r3 = (r3 == "" ? "3XH" : r3) + (r3.Length <= 3 ? p.Substring(5) : "");
                    }
                    else if (Regex.IsMatch(p, @"^PO1"))
                    {
                        pickOffR1 = true;
                        r1 = (r1 == "" ? (Regex.IsMatch(p, @"E") ? "1X1" : "1X2") : r1.Substring(0, 3)) + (p.Substring(3) + (r1.Length > 3 ? r1.Substring(3) : ""));
                    }
                    else if (Regex.IsMatch(p, @"^PO2"))
                    {
                        pickOffR2 = true;
                        r2 = (r2 == "" ? (Regex.IsMatch(p, @"E") ? "2X2" : "2X3") : r2) + (r2.Length <= 3 ? p.Substring(3) : "");
                    }
                    else if (Regex.IsMatch(p, @"^PO3"))
                    {
                        pickOffR3 = true;
                        r3 = (r3 == "" ? (Regex.IsMatch(p, @"E") ? "3X3" : "3XH") : r3) + (r3.Length <= 3 ? p.Substring(3) : "");
                    }
                    else if (Regex.IsMatch(p, @"^CS2"))
                    {
                        caughtStealingR1 = true;
                        r1 = (r1 == "" ? "1X2" : r1.Substring(0, 3)) + (p.Substring(3) + (r1.Length > 3 ? r1.Substring(3) : ""));
                    }
                    else if (Regex.IsMatch(p, @"^CS3"))
                    {
                        caughtStealingR2 = true;
                        r2 = (r2 == "" ? "2X3" : r2) + (r2.Length <= 3 ? p.Substring(3) : "");
                    }
                    else if (Regex.IsMatch(p, @"^CSH"))
                    {
                        caughtStealingR3 = true;
                        r3 = (r3 == "" ? "3XH" : r3) + (r3.Length <= 3 ? p.Substring(3) : "");
                    }
                    else if (Regex.IsMatch(p, @"^SB2"))
                    {
                        stolenBaseR1 = true;
                        r1 = r1 == "" ? "1-2" : r1;
                    }
                    else if (Regex.IsMatch(p, @"^SB3"))
                    {
                        stolenBaseR2 = true;
                        r2 = r2 == "" ? "2-3" : r2;
                    }
                    else if (Regex.IsMatch(p, @"^SBH"))
                    {
                        stolenBaseR3 = true;
                        r3 = (r3 == "" ? "3-H" : r3) + (p.Length > 3 ? p.Substring(3) : "");
                    }
                }
            }

            bool rbOut = Regex.IsMatch(rb, @"\(\d[^E]*?\)") || (strikeOut && !Regex.IsMatch(rb, @"B"));
            int? rbd = rbOut ? 0 : Regex.IsMatch(rb, @"\(TUR\)") ? 6 : Regex.IsMatch(rb, @"\(UR\)") ? 5 : rb.Length >= 3 ? int.Parse(rb.Replace("H", "4").Substring(2, 1)) : 0;
            bool r1Out = Regex.IsMatch(r1, @"\(\d[^E]*?\)");
            int? r1d = r1Out ? 0 : Regex.IsMatch(r1, @"\(TUR\)") ? 6 : Regex.IsMatch(r1, @"\(UR\)") ? 5 : r1.Length >= 3 ? int.Parse(r1.Replace("H", "4").Substring(2, 1)) : (int?)null;
            bool r2Out = Regex.IsMatch(r2, @"\(\d[^E]*?\)");
            int? r2d = r2Out ? 0 : Regex.IsMatch(r2, @"\(TUR\)") ? 6 : Regex.IsMatch(r2, @"\(UR\)") ? 5 : r2.Length >= 3 ? int.Parse(r2.Replace("H", "4").Substring(2, 1)) : (int?)null;
            bool r3Out = Regex.IsMatch(r3, @"\(\d[^E]*?\)");
            int? r3d = r3Out ? 0 : Regex.IsMatch(r3, @"\(TUR\)") ? 6 : Regex.IsMatch(r3, @"\(UR\)") ? 5 : r3.Length >= 3 ? int.Parse(r3.Replace("H", "4").Substring(2, 1)) : (int?)null;

            bool? rbiB = hitValue == 4 ? true : false;
            bool? rbi3 = !(r3d.HasValue && r3d.Value >= 4) ? false :
                Regex.IsMatch(r3, @"\(NR\)|\(NORBI\)") ? false :
                Regex.IsMatch(r3, @"\(RBI\)") ? true :
                Regex.IsMatch(r3, @"E") ? false :
                (hitValue > 0 || sacHit || sacFly || fieldersChoice) ? true :
                Regex.IsMatch(mod, @"GDP") ? false :
                (walk || hp || fielderInterference) && r1d.HasValue && r2d.HasValue ? true :
                Regex.IsMatch(bp, @"^[E123456789]") ? (bool?)null : false;
            bool? rbi2 = !(r2d.HasValue && r2d.Value >= 4) ? false :
                Regex.IsMatch(r2, @"\(NR\)|\(NORBI\)") ? false :
                Regex.IsMatch(r2, @"\(RBI\)") ? true :
                Regex.IsMatch(r2, @"E") ? false :
                (hitValue > 0 || sacHit || sacFly || fieldersChoice) && !Regex.IsMatch(bp, @"E") ? true :
                Regex.IsMatch(mod, @"GDP") ? false :
                Regex.IsMatch(bp, @"^[123456789]") ? (bool?)null : false;
            bool? rbi1 = !(r1d.HasValue && r1d.Value >= 4) ? false :
                Regex.IsMatch(r1, @"\(NR\)|\(NORBI\)") ? false :
                Regex.IsMatch(r1, @"\(RBI\)") ? true :
                Regex.IsMatch(r1, @"E") ? false :
                (hitValue > 0 || sacHit || sacFly || fieldersChoice) && !Regex.IsMatch(bp, @"E") ? true :
                Regex.IsMatch(mod, @"GDP") ? false :
                Regex.IsMatch(bp, @"^[123456789]") ? (bool?)null : false;

            string eventType;
            if (s)
                eventType = "Single";
            else if (d)
                eventType = "Double";
            else if (t)
                eventType = "Triple";
            else if (hr)
                eventType = "Home run";
            else if (iwalk)
                eventType = "Intentional walk";
            else if (walk)
                eventType = "Walk";
            else if (strikeOut)
                eventType = "Strikeout";
            else if (fieldersChoice)
                eventType = "Fielders choice";
            else if (Regex.IsMatch(bp, @"^E"))
                eventType = "Error";
            else if (Regex.IsMatch(bp, @"^BK"))
                eventType = "Balk";
            else if (Regex.IsMatch(bp, @"^DI"))
                eventType = "Defensive indifference";
            else if (Regex.IsMatch(bp, @"^OA"))
                eventType = "Other advance";
            else if (Regex.IsMatch(bp, @"^\d"))
                eventType = "Generic out";
            else if (stolenBaseR1 || stolenBaseR2 || stolenBaseR3)
                eventType = "Stolen base";
            else if (pickOffR1 || pickOffR2 || pickOffR3)
                eventType = "Pick off";
            else if (caughtStealingR1 || caughtStealingR2 || caughtStealingR3)
                eventType = "Caught stealing";
            else if (wildPitch)
                eventType = "Wild pitch";
            else if (passedBall)
                eventType = "Passed ball";
            else if (hp)
                eventType = "Hit by pitch";
            else if (fielderInterference)
                eventType = "Interference";
            else if (Regex.IsMatch(bp, @"^FLE"))
                eventType = "Foul error";
            else
                eventType = "Unknown event";

            BasicPlay = bp;
            Modifier = mod;
            Runner1 = r1;
            Runner2 = r2;
            Runner3 = r3;
            RunnerB = rb;
            Runner1Out = r1Out;
            Runner2Out = r2Out;
            Runner3Out = r3Out;
            RunnerBOut = rbOut;
            OutsOnPlay = (rbOut ? 1 : 0) + (r1Out ? 1 : 0) + (r2Out ? 1 : 0) + (r3Out ? 1 : 0);
            BatterEvent = batterEvent;
            HitValue = hitValue;
            R1Destination = r1d.HasValue ? r1d : StartOfPlay.RunnerOnFirst && string.IsNullOrEmpty(r1) ? 1 : r1d;
            R2Destination = r2d.HasValue ? r2d : StartOfPlay.RunnerOnSecond && string.IsNullOrEmpty(r2) ? 2 : r2d;
            R3Destination = r3d.HasValue ? r3d : StartOfPlay.RunnerOnThird && string.IsNullOrEmpty(r3) ? 3 : r3d;
            RBDestination = rbd;
            RunsOnPlay = (rbd >= 4 ? 1 : 0) + (r1d >= 4 ? 1 : 0) + (r2d >= 4 ? 1 : 0) + (r3d >= 4 ? 1 : 0);
            AtBat = batterEvent && !(walk || hp || fielderInterference || sacHit || sacFly);
            SacHit = sacHit;
            SacFly = sacFly;
            WildPitch = wildPitch;
            PassedBall = passedBall;
            Bunt = bunt;
            Foul = foul;
            StolenBaseR1 = stolenBaseR1;
            StolenBaseR2 = stolenBaseR2;
            StolenBaseR3 = stolenBaseR3;
            CaughtStealingR1 = caughtStealingR1;
            CaughtStealingR2 = caughtStealingR2;
            CaughtStealingR3 = caughtStealingR3;
            PickOffR1 = pickOffR1;
            PickOffR2 = pickOffR2;
            PickOffR3 = pickOffR3;
            PlayOnBatter = Regex.IsMatch(bprb, @"E") ? bprb : Regex.IsMatch(rb, @"^BX") ? (rbOut ? Regex.Match(rb, @"(?<=\()[\d]+(?=[\)\/X-])").Value :
                Regex.Match(rb, @"\(\d([^)\/]*)").Value.Substring(1)) : "";
            PlayOnR1 = r1Out ? Regex.Match(r1, @"(?<=\()[\d]+(?=[\)\/X-])").Value :
                bpr1 != "" && !Regex.IsMatch(bpr1, @"^SB") ? (Regex.IsMatch(bpr1, @"\([\dE]([^)\/]*)") ? Regex.Match(bpr1, @"\([\dE]([^)\/]*)").Value.Substring(1) : "") :
                Regex.IsMatch(r1, @"\([\d]([^)\/]*)") ? Regex.Match(r1, @"\([\d]([^)\/]*)").Value.Substring(1) : "";
            PlayOnR2 = r2Out ? Regex.Match(r2, @"(?<=\()[\d]+(?=[\)\/X-])").Value :
                bpr2 != "" && !Regex.IsMatch(bpr2, @"^SB") ? (Regex.IsMatch(bpr2, @"\([\dE]([^)\/]*)") ? Regex.Match(bpr2, @"\([\dE]([^)\/]*)").Value.Substring(1) : "") :
                Regex.IsMatch(r2, @"\([\d]([^)\/]*)") ? Regex.Match(r2, @"\([\d]([^)\/]*)").Value.Substring(1) : "";
            PlayOnR3 = r3Out ? Regex.Match(r3, @"(?<=\()[\d]+(?=[\)\/X-])").Value :
                bpr3 != "" && !Regex.IsMatch(bpr3, @"^SB") ? (Regex.IsMatch(bpr3, @"\([\dE]([^)\/]*)") ? Regex.Match(bpr3, @"\([\dE]([^)\/]*)").Value.Substring(1) : "") :
                Regex.IsMatch(r3, @"\([\d]([^)\/]*)") ? Regex.Match(r3, @"\([\d]([^)\/]*)").Value.Substring(1) : "";
            EventType = eventType;
            RBIB = rbiB;
            RBI1 = rbi1.HasValue ? rbi1 : StartOfPlay.Outs < 2 ? true : false;
            RBI2 = rbi2.HasValue ? rbi2 : StartOfPlay.Outs < 2 ? true : false;
            RBI3 = rbi3.HasValue ? rbi3 : StartOfPlay.Outs < 2 ? true : false;
            RBI = (RBIB.Value ? 1 : 0) + (RBI1.Value ? 1 : 0) + (RBI2.Value ? 1 : 0) + (RBI3.Value ? 1 : 0);

            EndOfPlay.Outs = StartOfPlay.Outs + OutsOnPlay;
            EndOfPlay.VisitorScore = StartOfPlay.VisitorScore + (TeamAtBat == "V" ? RunsOnPlay : 0);
            EndOfPlay.HomeScore = StartOfPlay.HomeScore + (TeamAtBat == "H" ? RunsOnPlay : 0);
            EndOfHalfInning = EndOfPlay.Outs == 3 || LastPlay;
            EndOfGame = (EndOfHalfInning && TeamAtBat == "V" && Inning == 9 && EndOfPlay.HomeScore > EndOfPlay.VisitorScore)
                || (TeamAtBat == "H" && Inning >= 9 && EndOfPlay.HomeScore > EndOfPlay.VisitorScore)
                || (EndOfHalfInning && Inning >= 9 && TeamAtBat == "H" && EndOfPlay.HomeScore < EndOfPlay.VisitorScore)
                || LastPlay;
            EndOfPlay.RunnerOnFirst = (rbd == 1 || R1Destination == 1 || r2d == 1 || r3d == 1);
            EndOfPlay.Runner1BO = rbd == 1 ? (byte)BattingOrder : R1Destination == 1 ? StartOfPlay.Runner1BO : r2d == 1 ? StartOfPlay.Runner2BO : r3d == 1 ? StartOfPlay.Runner3BO : null;
            EndOfPlay.RunnerOnSecond = (rbd == 2 || r1d == 2 || R2Destination == 2 || r3d == 2);
            EndOfPlay.Runner2BO = rbd == 2 ? (byte)BattingOrder : r1d == 2 ? StartOfPlay.Runner1BO : R2Destination == 2 ? StartOfPlay.Runner2BO : r3d == 2 ? StartOfPlay.Runner3BO : null;
            EndOfPlay.RunnerOnThird = (rbd == 3 || r1d == 3 || r2d == 3 || R3Destination == 3);
            EndOfPlay.Runner3BO = rbd == 3 ? (byte)BattingOrder : r1d == 3 ? StartOfPlay.Runner1BO : r2d == 3 ? StartOfPlay.Runner2BO : R3Destination == 3 ? StartOfPlay.Runner3BO : null;

            EndOfHalfInning = EndOfGame ? true : EndOfHalfInning;
            if (TeamAtBat == "V" && !batterEvent)
            {
                VisitorNextBatter = BattingOrder;
            }
            else if (TeamAtBat == "H" && !batterEvent)
            {
                HomeNextBatter = BattingOrder;
            }

        }

        public class GameSituation
        {
            public int Outs { get; set; }
            public int VisitorScore { get; set; }
            public int HomeScore { get; set; }
            public bool RunnerOnFirst { get; set; }
            public byte? Runner1BO { get; set; }
            public bool RunnerOnSecond { get; set; }
            public byte? Runner2BO { get; set; }
            public bool RunnerOnThird { get; set; }
            public byte? Runner3BO { get; set; }
        }
    }
}
