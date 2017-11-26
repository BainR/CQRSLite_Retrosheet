using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.ReadModel.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CQRSLite_Retrosheet.Web.Controllers
{
    [Route("Retrosheet/Read")]
    public class QueryController : Controller
    {
        private GameSummaryRepository summaryRepo;
        private BaseballPlayRepository baseballPlayRepo;
        private TeamRepository teamRepo;
        private PlayerRepository playerRepo;
        private RosterMemberRepository rosterMemberRepo;
        private LineupRepository lineupRepo;
        private LineupChangeRepository lineupChangeRepo;


        public QueryController(BaseballPlayRepository baseballPlayRepo, TeamRepository teamRepo, PlayerRepository playerRepo, RosterMemberRepository rosterMemberRepo,
            GameSummaryRepository summaryRepo, LineupRepository lineupRepo, LineupChangeRepository lineupChangeRepo)
        {
            this.baseballPlayRepo = baseballPlayRepo;
            this.teamRepo = teamRepo;
            this.playerRepo = playerRepo;
            this.rosterMemberRepo = rosterMemberRepo;
            this.summaryRepo = summaryRepo;
            this.lineupRepo = lineupRepo;
            this.lineupChangeRepo = lineupChangeRepo;
        }

        #region Game Summary

        // http://localhost:57285/Retrosheet/Read/GameSummary/MIN199110270
        [HttpGet("GameSummary/{gameid}")]
        public IActionResult GetGameSummary(string gameid)
        {
            var summary = summaryRepo.GetGame(gameid);
            if (summary == null || string.IsNullOrWhiteSpace(summary.RetrosheetGameId))
            {
                gameid = gameid.Replace("'", "");
                return BadRequest("No Baseball Game with ID " + gameid + " was found.");
            }
            return Ok(summary);
        }

        // http://localhost:57285/Retrosheet/Read/GamesBySeason/1991
        [HttpGet("GamesBySeason/{year}")]
        public IActionResult GetGameSummary(int year)
        {
            var games = summaryRepo.GetGamesBySeason(year);
            if (games.Count == 0)
            {
                return BadRequest("No Games in " + year.ToString() + " were found.");
            }
            return Ok(games);
        }

        #endregion

        #region Baseball Play

        // http://localhost:57285/Retrosheet/Read/Event/MIN199110270/80
        [HttpGet("Event/{gameid}/{eventnumber}")]
        public IActionResult GetBaseballPlay(string gameid, int eventnumber)
        {
            var baseballPlay = baseballPlayRepo.GetPlay(gameid, eventnumber);
            if (baseballPlay == null || string.IsNullOrWhiteSpace(baseballPlay.RetrosheetGameId))
            {
                gameid = gameid.Replace("'", "");
                string id = gameid + "_" + eventnumber.ToString("000");
                return BadRequest("No Baseball Play with ID " + id + " was found.");
            }
            return Ok(baseballPlay);
        }

        // http://localhost:57285/Retrosheet/Read/Game/MIN199110270
        [HttpGet("Game/{gameid}")]
        public IActionResult GetBaseballGame(string gameid)
        {
            var games = baseballPlayRepo.GetGame(gameid);
            if (games == null || games.Count == 0)
            {
                gameid = gameid.Replace("'", "");
                return BadRequest("No Baseball Game with ID " + gameid + " was found.");
            }
            return Ok(games);
        }

        // http://localhost:57285/Retrosheet/Read/Event/Players/StartOfPlay/MIN199110270/80
        [HttpGet("Event/Players/StartOfPlay/{gameid}/{eventnumber}")]
        public IActionResult GetBaseballPlayPlayersStartOfPlay(string gameid, int eventnumber)
        {
            var play = baseballPlayRepo.GetPlay(gameid, eventnumber);
            var lineup = lineupRepo.GetGameLineup(gameid);
            if (play == null || string.IsNullOrWhiteSpace(play.RetrosheetGameId) || lineup == null || lineup.Count == 0)
            {
                gameid = gameid.Replace("'", "");
                string id = gameid + "_" + eventnumber.ToString("000");
                return BadRequest("No Baseball Play with ID " + id + " was found.");
            }

            var players = PLayersOnField.StartOfPlay(play, lineup);

            return Ok(players);
        }

        // http://localhost:57285/Retrosheet/Read/Event/Players/EndOfPlay/MIN199110270/80
        [HttpGet("Event/Players/EndOfPlay/{gameid}/{eventnumber}")]
        public IActionResult GetBaseballPlayPlayersEndOfPlay(string gameid, int eventnumber)
        {
            var play = baseballPlayRepo.GetPlay(gameid, eventnumber);
            var lineup = lineupRepo.GetGameLineup(gameid);
            if (play == null || string.IsNullOrWhiteSpace(play.RetrosheetGameId) || lineup == null || lineup.Count == 0)
            {
                gameid = gameid.Replace("'", "");
                string id = gameid + "_" + eventnumber.ToString("000");
                return BadRequest("No Baseball Play with ID " + id + " was found.");
            }

            var players = PLayersOnField.EndOfPlay(play, lineup);

            return Ok(players);
        }

        #endregion

        #region Team

        // http://localhost:57285/Retrosheet/Read/Team/MIN/1991
        [HttpGet("Team/{teamcode}/{year}")]
        public IActionResult GetTeam(string teamcode, int year)
        {
            var team = teamRepo.GetTeam(teamcode, year);
            if (team == null || string.IsNullOrWhiteSpace(team.TeamCode))
            {
                teamcode = teamcode.Replace("'", "");
                return BadRequest("Team " + teamcode + " was not found in " + year.ToString());
            }
            return Ok(team);
        }

        // http://localhost:57285/Retrosheet/Read/TeamsByYear/1991
        [HttpGet("TeamsByYear/{year}")]
        public IActionResult GetTeamsByYear(int year)
        {
            var teams = teamRepo.GetTeamsByYear(year);
            if (teams == null || teams.Count == 0)
            {
                return BadRequest("Year " + year + " was not found.");
            }
            return Ok(teams);
        }

        // http://localhost:57285/Retrosheet/Read/TeamsByTeamCode/MIN
        [HttpGet("TeamsByTeamCode/{team}")]
        public IActionResult GetTeamsByTeamCode(string team)
        {
            var teams = teamRepo.GetTeamsByTeamCode(team);
            if (teams == null || teams.Count == 0)
            {
                team = team.Replace("'", "");
                return BadRequest("Team " + team + " was not found.");
            }
            return Ok(teams);
        }

        #endregion

        #region Player

        // http://localhost:57285/Retrosheet/Read/Player/puckk001
        [HttpGet("Player/{id}")]
        public IActionResult GetPlayer(string id)
        {
            var player = playerRepo.GetPlayer(id);
            if (player == null || string.IsNullOrWhiteSpace(player.PlayerId))
            {
                id = id.Replace("'", "");
                return BadRequest("Player " + id + " was not found.");
            }
            return Ok(player);
        }

        // http://localhost:57285/Retrosheet/Read/Players
        [HttpGet("Players")]
        public IActionResult GetPlayers()
        {
            var players = playerRepo.GetAll();
            return Ok(players);
        }

        #endregion

        #region Roster

        // http://localhost:57285/Retrosheet/Read/RosterMember/MIN/1991/puckk001
        [HttpGet("RosterMember/{teamcode}/{year}/{playerid}")]
        public IActionResult GetRosterMember(string teamcode, int year, string playerid)
        {
            var rosterMember = rosterMemberRepo.GetRosterMember(teamcode, year, playerid);
            if (rosterMember == null || string.IsNullOrWhiteSpace(rosterMember.PlayerId))
            {
                teamcode = teamcode.Replace("'", "");
                playerid = playerid.Replace("'", "");
                return BadRequest("Player " + playerid + " was not found on team " + teamcode + " in " + year.ToString());
            }
            return Ok(rosterMember);
        }

        // http://localhost:57285/Retrosheet/Read/Roster/MIN/1991
        [HttpGet("Roster/{teamcode}/{year}")]
        public IActionResult GetRoster(string teamcode, int year)
        {
            var roster = rosterMemberRepo.GetRoster(teamcode, year);
            if (roster == null || roster.Count == 0)
            {
                teamcode = teamcode.Replace("'", "");
                return BadRequest("Team " + teamcode + " was not found in " + year.ToString());
            }
            return Ok(roster);
        }

        #endregion

        #region Lineup

        // http://localhost:57285/Retrosheet/Read/Lineup/MIN199110270/20
        [HttpGet("Lineup/{gameid}/{sequence}")]
        public IActionResult GetLineup(string gameid, short sequence)
        {
            var lineup = lineupRepo.GetLineup(gameid, sequence);
            if (lineup == null || string.IsNullOrWhiteSpace(lineup.RetrosheetGameId))
            {
                gameid = gameid.Replace("'", "");
                return BadRequest("No Lineup for game " + gameid + " with sequence " + sequence.ToString() + " was found.");
            }
            return Ok(lineup);
        }

        // http://localhost:57285/Retrosheet/Read/Lineup/MIN199110270
        [HttpGet("Lineup/{gameid}")]
        public IActionResult GetGameLineup(string gameid)
        {
            var lineup = lineupRepo.GetGameLineup(gameid);
            if (lineup == null || lineup.Count == 0)
            {
                gameid = gameid.Replace("'", "");
                return BadRequest("No Lineup for game " + gameid + " was found.");
            }
            return Ok(lineup);
        }

        #endregion

        #region Lineup Change

        // http://localhost:57285/Retrosheet/Read/LineupChange/MIN199110270
        [HttpGet("LineupChange/{gameid}")]
        public IActionResult GetGameLineupChanges(string gameid)
        {
            var lineupChanges = lineupChangeRepo.GetGameLineupChanges(gameid);
            if (lineupChanges == null || lineupChanges.Count == 0)
            {
                gameid = gameid.Replace("'", "");
                return BadRequest("No Lineup Changes for game " + gameid + " were found.");
            }
            return Ok(lineupChanges);
        }

        #endregion
    }

    public class PLayersOnField
    {
        public string RetrosheetGameId { get; set; }
        public int EventNumber { get; set; }
        public bool IsStartOfPlay { get; set; }
        public int inning { get; set; }
        public int outs { get; set; }
        public string TeamAtBat { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public string Pitcher { get; set; }
        public string Catcher { get; set; }
        public string FirstBaseman { get; set; }
        public string SecondBaseman { get; set; }
        public string ThirdBaseman { get; set; }
        public string ShortStop { get; set; }
        public string LeftFielder { get; set; }
        public string CenterFielder { get; set; }
        public string RightFielder { get; set; }
        public string Batter { get; set; }
        public string RunnerOnFirst { get; set; }
        public string RunnerOnSecond { get; set; }
        public string RunnerOnThird { get; set; }

        public static PLayersOnField StartOfPlay(BaseballPlayRM play, List<LineupRM> lineup)
        {
            if (play == null || lineup == null || lineup.Count == 0 || play.RetrosheetGameId != lineup[0].RetrosheetGameId)
            {
                throw new System.Exception("Invalid play or lineup");
            }

            PLayersOnField players = new PLayersOnField();

            players.RetrosheetGameId = play.RetrosheetGameId;
            players.EventNumber = play.EventNumber;
            players.IsStartOfPlay = true;
            players.inning = play.Inning;
            players.outs = play.StartOfPlay_Outs;
            players.TeamAtBat = play.TeamAtBat;
            players.HomeTeamScore = play.StartOfPlay_HomeScore;
            players.AwayTeamScore = play.StartOfPlay_VisitorScore;
            players.Pitcher = FindDefender(1, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.Catcher = FindDefender(2, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.FirstBaseman = FindDefender(3, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.SecondBaseman = FindDefender(4, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.ThirdBaseman = FindDefender(5, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.ShortStop = FindDefender(6, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.LeftFielder = FindDefender(7, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.CenterFielder = FindDefender(8, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RightFielder = FindDefender(9, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.Batter = FindPlayerByBattingOrder(play.BattingOrder, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RunnerOnFirst = FindPlayerByBattingOrder(play.StartOfPlay_Runner1BO, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RunnerOnSecond = FindPlayerByBattingOrder(play.StartOfPlay_Runner2BO, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RunnerOnThird = FindPlayerByBattingOrder(play.StartOfPlay_Runner3BO, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);

            return players;
        }

        public static PLayersOnField EndOfPlay(BaseballPlayRM play, List<LineupRM> lineup)
        {
            if (play == null || lineup == null || lineup.Count == 0 || play.RetrosheetGameId != lineup[0].RetrosheetGameId)
            {
                throw new System.Exception("Invalid play or lineup");
            }

            PLayersOnField players = new PLayersOnField();

            players.RetrosheetGameId = play.RetrosheetGameId;
            players.EventNumber = play.EventNumber;
            players.IsStartOfPlay = false;
            players.inning = play.Inning;
            players.outs = play.EndOfPlay_Outs;
            players.TeamAtBat = play.TeamAtBat;
            players.HomeTeamScore = play.EndOfPlay_HomeScore;
            players.AwayTeamScore = play.EndOfPlay_VisitorScore;
            players.Pitcher = FindDefender(1, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.Catcher = FindDefender(2, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.FirstBaseman = FindDefender(3, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.SecondBaseman = FindDefender(4, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.ThirdBaseman = FindDefender(5, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.ShortStop = FindDefender(6, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.LeftFielder = FindDefender(7, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.CenterFielder = FindDefender(8, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RightFielder = FindDefender(9, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.Batter = play.BatterEvent ? "" : FindPlayerByBattingOrder(play.BattingOrder, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RunnerOnFirst = FindPlayerByBattingOrder(play.EndOfPlay_Runner1BO, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RunnerOnSecond = FindPlayerByBattingOrder(play.EndOfPlay_Runner2BO, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);
            players.RunnerOnThird = FindPlayerByBattingOrder(play.EndOfPlay_Runner3BO, play.TeamAtBat, lineup[play.LineupChangeSequence - 1]);

            return players;
        }

        private static string FindDefender(int position, string teamAtBat, LineupRM lineup)
        {
            if (teamAtBat == "H" && lineup.Away_BO0_FieldPosition == position)
            {
                return lineup.Away_BO0_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO1_FieldPosition == position)
            {
                return lineup.Away_BO1_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO2_FieldPosition == position)
            {
                return lineup.Away_BO2_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO3_FieldPosition == position)
            {
                return lineup.Away_BO3_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO4_FieldPosition == position)
            {
                return lineup.Away_BO4_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO5_FieldPosition == position)
            {
                return lineup.Away_BO5_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO6_FieldPosition == position)
            {
                return lineup.Away_BO6_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO7_FieldPosition == position)
            {
                return lineup.Away_BO7_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO8_FieldPosition == position)
            {
                return lineup.Away_BO8_PlayerId;
            }
            else if (teamAtBat == "H" && lineup.Away_BO9_FieldPosition == position)
            {
                return lineup.Away_BO9_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO0_FieldPosition == position)
            {
                return lineup.Home_BO0_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO1_FieldPosition == position)
            {
                return lineup.Home_BO1_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO2_FieldPosition == position)
            {
                return lineup.Home_BO2_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO3_FieldPosition == position)
            {
                return lineup.Home_BO3_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO4_FieldPosition == position)
            {
                return lineup.Home_BO4_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO5_FieldPosition == position)
            {
                return lineup.Home_BO5_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO6_FieldPosition == position)
            {
                return lineup.Home_BO6_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO7_FieldPosition == position)
            {
                return lineup.Home_BO7_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO8_FieldPosition == position)
            {
                return lineup.Home_BO8_PlayerId;
            }
            else if (teamAtBat == "V" && lineup.Home_BO9_FieldPosition == position)
            {
                return lineup.Home_BO9_PlayerId;
            }
            else
            {
                return "";
            }
        }

        private static string FindPlayerByBattingOrder(int? battingOrder, string teamAtBat, LineupRM lineup)
        {
            if (!battingOrder.HasValue)
            {
                return "";
            }
            else if (teamAtBat == "H" && battingOrder == 1)
            {
                return lineup.Home_BO1_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 2)
            {
                return lineup.Home_BO2_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 3)
            {
                return lineup.Home_BO3_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 4)
            {
                return lineup.Home_BO4_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 5)
            {
                return lineup.Home_BO5_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 6)
            {
                return lineup.Home_BO6_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 7)
            {
                return lineup.Home_BO7_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 8)
            {
                return lineup.Home_BO8_PlayerId;
            }
            else if (teamAtBat == "H" && battingOrder == 9)
            {
                return lineup.Home_BO9_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 1)
            {
                return lineup.Away_BO1_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 2)
            {
                return lineup.Away_BO2_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 3)
            {
                return lineup.Away_BO3_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 4)
            {
                return lineup.Away_BO4_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 5)
            {
                return lineup.Away_BO5_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 6)
            {
                return lineup.Away_BO6_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 7)
            {
                return lineup.Away_BO7_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 8)
            {
                return lineup.Away_BO8_PlayerId;
            }
            else if (teamAtBat == "V" && battingOrder == 9)
            {
                return lineup.Away_BO9_PlayerId;
            }
            else
            {
                return "";
            }
        }
    }
}
