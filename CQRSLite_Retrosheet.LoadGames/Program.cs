using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CQRSLite_Retrosheet.LoadGames
{
    class Program
    {
        private static HttpClient client = new HttpClient();
        public static IConfigurationRoot Configuration { get; set; }

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            bool loadTeamAndRosterFiles = true;
            if (bool.TryParse(Configuration["LoadTeamAndRosterFiles"], out loadTeamAndRosterFiles) && loadTeamAndRosterFiles)
            {
                await LoadTeamsAsync();
                await LoadRostersAsync();
            }

            await LoadGamesAsync();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task LoadGamesAsync()
        {
            string webServiceBaseURL = Configuration["WebServiceBaseURL"];
            if (!webServiceBaseURL.EndsWith("/"))
            {
                webServiceBaseURL = webServiceBaseURL + "/";
            }

            switch (Configuration["SourceType"])
            {
                case "URL":
                    var httpClient = new HttpClient();
                    Stream urlZipFile = httpClient.GetStreamAsync(Configuration["SourceURL"]).Result;
                    ZipArchive urlArchive = new ZipArchive(urlZipFile);
                    foreach (ZipArchiveEntry entry in urlArchive.Entries.Where(a => a.FullName.Substring(a.FullName.Length - 4, 2) == ".E"))
                    {
                        await ProcessEventFileAsync(entry.FullName, entry.Open(), webServiceBaseURL);
                    }
                    break;
                case "ZipFile":
                    Stream zipFile = File.Open(Configuration["SourceZipFile"], FileMode.Open, FileAccess.Read, FileShare.Read);
                    ZipArchive archive = new ZipArchive(zipFile);
                    foreach (ZipArchiveEntry entry in archive.Entries.Where(a => a.FullName.Substring(a.FullName.Length - 4, 2) == ".E"))
                    {
                        await ProcessEventFileAsync(entry.FullName, entry.Open(), webServiceBaseURL);
                    }
                    break;
                case "Directory":
                    foreach (string filename in Directory.GetFiles(Configuration["SourceDirectory"], "*.E*"))
                    {
                        await ProcessEventFileAsync(filename, File.OpenRead(filename), webServiceBaseURL);
                    }
                    break;
                default:
                    break;
            }
        }

        private static async Task ProcessEventFileAsync(string filename, Stream eventFile, string webServiceBaseURL)
        {
            // used for debugging
            //if (!filename.Contains("1993"))
            //{
            //    return;
            //}

            Console.WriteLine(filename + DateTime.Now.ToString(" MM/dd/yyyy HH:mm:ss.fff"));

            List<List<string>> games = new List<List<string>>();
            using (StreamReader reader = new StreamReader(eventFile))
            {
                string line = reader.ReadLine();
                List<string> plays = new List<string>();

                while (!reader.EndOfStream)
                {
                    if (line.StartsWith("id"))
                    {
                        if (plays.Count > 0)
                        {
                            games.Add(plays);
                        }
                        plays = new List<string>();
                    }
                    plays.Add(line);
                    line = reader.ReadLine();
                }
                plays.Add(line);
                games.Add(plays);
            } // using StreamReader

            foreach (var game in games)
            {
                string retrosheetGameId = "";
                short fileLineNumber = 0;
                short eventNumber = 0;
                short lineupChangeSequence = 0;
                bool startOfData = true;
                int lastPlayIndex = game.Select((value, index) => new { value, index }).Where(pair => pair.value.StartsWith("play") && !pair.value.EndsWith(",NP")).Max(m => m.index);
                int lastLineupChangeIndex = game.Select((value, index) => new { value, index }).Where(pair => pair.value.StartsWith("start") || pair.value.StartsWith("sub")).Max(m => m.index);

                string hometeam = "";
                string visteam = "";
                string usedh = "";
                string site = "";
                string wp = "";
                string lp = "";
                string save = "";

                foreach (var line in game)
                {
                    // don't care comments show opportunity for error checking and for future enhancements
                    fileLineNumber++;

                    if (line.StartsWith("id"))
                    {
                        // new game
                        retrosheetGameId = line.Split(',')[1];
                        fileLineNumber = 0;
                        eventNumber = 0;
                        lineupChangeSequence = 0;
                        startOfData = true;
                    }
                    else if (line.StartsWith("version"))
                    {
                        // version, don't care
                    }
                    else if (line.StartsWith("info"))
                    {
                        // tracking some info values, ignoring others
                        string[] fields = line.Split(',');
                        switch (fields[1])
                        {
                            case "visteam":
                                visteam = fields[2];
                                break;
                            case "hometeam":
                                hometeam = fields[2];
                                break;
                            case "usedh":
                                usedh = fields[2];
                                break;
                            case "site":
                                site = fields[2];
                                break;
                            case "wp":
                                wp = fields[2];
                                break;
                            case "lp":
                                lp = fields[2];
                                break;
                            case "save":
                                save = fields[2];
                                break;
                            default:
                                break;
                        }

                    }
                    else if (line.StartsWith("start"))
                    {
                        // start
                        try
                        {
                            string[] fields = line.Split(','); // assumes no commas in quoted fields, fields[0] is the word "start"
                            string playerId = fields[1];
                            string name = fields[2];
                            int team = int.Parse(fields[3]); // 0 = visiting team, 1 = hometeam
                            int battingOrder = int.Parse(fields[4]); // batting order position
                            int fieldPosition = int.Parse(fields[5]); // position in field
                            bool lastLineupChange = fileLineNumber == lastLineupChangeIndex;
                            lineupChangeSequence++;

                            var body = new
                            {
                                RetrosheetGameId = retrosheetGameId,
                                EventNumber = 1,
                                Sequence = lineupChangeSequence, // PK = RetrosheetGameId, Sequence
                                IsStarter = true,
                                PlayerId = playerId,
                                Name = name,
                                Team = team,
                                BattingOrder = battingOrder,
                                FieldPosition = fieldPosition,
                                LastLineupChange = lastLineupChange
                            };

                            await CallAPIAsync(webServiceBaseURL, "Retrosheet/Write/LineupChange", JsonConvert.SerializeObject(body));
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("Error (Event Starter): " + line + " ~ " + e.Message);
                        }
                    }
                    else if (line.StartsWith("play"))
                    {
                        // play
                        try
                        {
                            if (eventNumber == 1)
                            {
                                var body = new
                                {
                                    RetrosheetGameId = retrosheetGameId,
                                    AwayTeam = visteam,
                                    HomeTeam = hometeam,
                                    UseDH = usedh,
                                    ParkCode = site,
                                    WinningPitcher = wp,
                                    LosingPitcher = lp,
                                    SavePitcher = save
                                };

                                await CallAPIAsync(webServiceBaseURL, "Retrosheet/Write/GameSummary", JsonConvert.SerializeObject(body));
                            }

                            string[] fields = line.Split(','); // assumes no commas in quoted fields, fields[0] is the word "play"
                            int inning = int.Parse(fields[1]);
                            int teamAtBat = int.Parse(fields[2]); // 0 = visitor, 1 = home
                            string batter = fields[3];
                            string countOnBatter = fields[4]; // could be ??, otherwise two digis, balls followed by strikes
                            string pitches = fields[5]; // could be empty
                            string play = fields[6];
                            bool lastPlay = fileLineNumber == lastPlayIndex;

                            if (string.Equals(play, "NP", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // no play, next line in file is a sub line
                            }
                            else
                            {
                                // send play to web service
                                eventNumber++;

                                var body = new
                                {
                                    RetrosheetGameId = retrosheetGameId,
                                    EventNumber = eventNumber,
                                    LineupChangeSequence = lineupChangeSequence,
                                    EventText = play,
                                    Inning = inning,
                                    TeamAtBat = teamAtBat,
                                    Batter = batter,
                                    CountOnBatter = countOnBatter,
                                    Pitches = pitches,
                                    LastPlay = lastPlay
                                };

                                await CallAPIAsync(webServiceBaseURL, "Retrosheet/Write/Event", JsonConvert.SerializeObject(body));
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("Error (Event Play): " + line + " ~ " + e.Message);
                        }
                    }
                    else if (line.StartsWith("sub"))
                    {
                        // sub
                        try
                        {
                            string[] fields = line.Split(','); // assumes no commas in quoted fields, fields[0] is the word "start"
                            string playerId = fields[1];
                            string name = fields[2];
                            int team = int.Parse(fields[3]); // 0 = visiting team, 1 = hometeam
                            int battingOrder = int.Parse(fields[4]); // batting order position
                            int fieldPosition = int.Parse(fields[5]); // position in field
                            bool lastLineupChange = fileLineNumber == lastLineupChangeIndex;
                            lineupChangeSequence++;

                            var body = new
                            {
                                RetrosheetGameId = retrosheetGameId,
                                EventNumber = eventNumber + 1, // matches event number that follows the substitution
                                Sequence = lineupChangeSequence, // PK = RetrosheetGameId, EventNumber, Sequence
                                IsStarter = false,
                                PlayerId = playerId,
                                Name = name,
                                Team = team,
                                BattingOrder = battingOrder,
                                FieldPosition = fieldPosition,
                                LastLineupChange = lastLineupChange
                            };

                            await CallAPIAsync(webServiceBaseURL, "Retrosheet/Write/LineupChange", JsonConvert.SerializeObject(body));
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("Error (Event Substitute): " + line + " ~ " + e.Message);
                        }
                    }
                    else if (line.StartsWith("com"))
                    {
                        // comment, don't care
                    }
                    else if (line.StartsWith("badj"))
                    {
                        // batting hand adjustment, ie switch hitter batting right handed against right handed pitcher, don't care
                    }
                    else if (line.StartsWith("padj"))
                    {
                        // pitching hand adjustment, pitcher pitches with non-typical hand, don't care
                    }
                    else if (line.StartsWith("ladj"))
                    {
                        // team bats out of order, likely to be a problem, future fix?
                    }
                    else if (line.StartsWith("data"))
                    {
                        // data, earned runs for each pitcher, occurs after all play lines and could signal end of game, otherwise don't care
                        if (startOfData)
                        {
                            Console.WriteLine("Completed " + retrosheetGameId + " " + DateTime.Now.ToString(" MM/dd/yyyy HH:mm:ss.fff"));
                            startOfData = false;
                        }
                    }
                    else if (line.Trim() == "")
                    {
                        // blank line, don't care
                    }
                    else
                    {
                        throw new Exception("unknown line type");
                    }
                } // foreach line
            } // foreach game
        } // ProcessEventFile

        private static async Task LoadTeamsAsync()
        {
            string webServiceBaseURL = Configuration["WebServiceBaseURL"];
            if (!webServiceBaseURL.EndsWith("/"))
            {
                webServiceBaseURL = webServiceBaseURL + "/";
            }

            switch (Configuration["SourceType"])
            {
                case "URL":
                    var httpClient = new HttpClient();
                    Stream urlZipFile = httpClient.GetStreamAsync(Configuration["SourceURL"]).Result;
                    ZipArchive urlArchive = new ZipArchive(urlZipFile);
                    foreach (ZipArchiveEntry entry in urlArchive.Entries.Where(a => a.FullName.StartsWith("TEAM")))
                    {
                        await ProcessTeamFileAsync(entry.FullName, entry.Open(), webServiceBaseURL);
                    }
                    break;
                case "ZipFile":
                    Stream zipFile = File.Open(Configuration["SourceZipFile"], FileMode.Open, FileAccess.Read, FileShare.Read);
                    ZipArchive archive = new ZipArchive(zipFile);
                    foreach (ZipArchiveEntry entry in archive.Entries.Where(a => a.FullName.StartsWith("TEAM")))
                    {
                        await ProcessTeamFileAsync(entry.FullName, entry.Open(), webServiceBaseURL);
                    }
                    break;
                case "Directory":
                    foreach (string filename in Directory.GetFiles(Configuration["SourceDirectory"], "TEAM*"))
                    {
                        await ProcessTeamFileAsync(filename, File.OpenRead(filename), webServiceBaseURL);
                    }
                    break;
                default:
                    break;
            }
        }

        private static async Task ProcessTeamFileAsync(string filename, Stream teamFile, string webServiceBaseURL)
        {
            Console.WriteLine(filename + DateTime.Now.ToString(" MM/dd/yyyy HH:mm:ss.fff"));

            HttpClient client = new HttpClient();
            int year;
            if (!int.TryParse(filename.Substring(filename.Length - 4, 4), out year))
            {
                return;
            }
            using (StreamReader reader = new StreamReader(teamFile))
            {
                do
                {
                    string teamData = reader.ReadLine();
                    string[] fields = teamData.Split(',');
                    if (fields.Count() == 4)
                    {
                        try
                        {
                            string teamCode = fields[0];
                            string league = fields[1];
                            string home = fields[2];
                            string name = fields[3];

                            var body = new
                            {
                                Year = year,
                                TeamCode = teamCode,
                                League = league,
                                Home = home,
                                Name = name
                            };

                            await CallAPIAsync(webServiceBaseURL, "Retrosheet/Write/AddTeam", JsonConvert.SerializeObject(body));
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("Error (Team): " + teamData + " ~ " + e.Message);
                        }
                    }
                } while (!reader.EndOfStream);
            }
        }

        private static async Task LoadRostersAsync()
        {
            string webServiceBaseURL = Configuration["WebServiceBaseURL"];
            if (!webServiceBaseURL.EndsWith("/"))
            {
                webServiceBaseURL = webServiceBaseURL + "/";
            }

            switch (Configuration["SourceType"])
            {
                case "URL":
                    var httpClient = new HttpClient();
                    Stream urlZipFile = httpClient.GetStreamAsync(Configuration["SourceURL"]).Result;
                    ZipArchive urlArchive = new ZipArchive(urlZipFile);
                    foreach (ZipArchiveEntry entry in urlArchive.Entries.Where(a => a.FullName.EndsWith(".ROS")))
                    {
                        if (entry.FullName.Substring(entry.FullName.Length - 8, 4).All(char.IsDigit))
                        {
                            // exclude composite roster files such as ALLPOST.ROS
                            await ProcessRosterFileAsync(entry.FullName, entry.Open(), webServiceBaseURL);
                        }
                    }
                    break;
                case "ZipFile":
                    Stream zipFile = File.Open(Configuration["SourceZipFile"], FileMode.Open, FileAccess.Read, FileShare.Read);
                    ZipArchive archive = new ZipArchive(zipFile);
                    foreach (ZipArchiveEntry entry in archive.Entries.Where(a => a.FullName.EndsWith(".ROS")))
                    {
                        if (entry.FullName.Substring(entry.FullName.Length - 8, 4).All(char.IsDigit))
                        {
                            // exclude composite roster files such as ALLPOST.ROS
                            await ProcessRosterFileAsync(entry.FullName, entry.Open(), webServiceBaseURL);
                        }
                    }
                    break;
                case "Directory":
                    foreach (string filename in Directory.GetFiles(Configuration["SourceDirectory"], "*.ROS"))
                    {
                        if (filename.Substring(filename.Length - 8, 4).All(char.IsDigit))
                        {
                            // exclude composite roster files such as ALLPOST.ROS
                            await ProcessRosterFileAsync(filename, File.OpenRead(filename), webServiceBaseURL);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private static async Task ProcessRosterFileAsync(string filename, Stream rosterFile, string webServiceBaseURL)
        {
            if (filename == "AS3315.ROS" || filename == "MULTTEAM.ROS")
            {
                return; // unwanted files in allas.zip
            }

            Console.WriteLine(filename + DateTime.Now.ToString(" MM/dd/yyyy HH:mm:ss.fff"));

            HttpClient client = new HttpClient();
            int year = int.Parse(filename.Substring(filename.Length - 8, 4));
            string teamCode = filename.Substring(filename.Length - 11, 3);
            using (StreamReader reader = new StreamReader(rosterFile))
            {
                do
                {
                    string rosterData = reader.ReadLine();
                    string[] fields = rosterData.Split(',');
                    if (fields.Count() >= 5)
                    {
                        try
                        {
                            string playerId = fields[0].Replace("\"", "");
                            string lastName = fields[1].Replace("\"", "");
                            string firstName = fields[2].Replace("\"", "");
                            string bats = fields[3].Replace("\"", "");
                            string throws = fields[4].Replace("\"", "");

                            var body = new
                            {
                                Year = year,
                                TeamCode = teamCode,
                                PlayerId = playerId,
                                LastName = lastName,
                                FirstName = firstName,
                                Bats = bats,
                                Throws = throws
                            };

                            await CallAPIAsync(webServiceBaseURL, "Retrosheet/Write/AddRosterMember", JsonConvert.SerializeObject(body));
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("Error (Roster): " + rosterData + " ~ " + e.Message);
                        }
                    }
                } while (!reader.EndOfStream);
            }
        }

        public static async Task CallAPIAsync(string webServiceBaseURL, string methodName, string jsonContent)
        {
            try
            {
                StringContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(webServiceBaseURL + methodName, content);
                if (!response.IsSuccessStatusCode)
                {
                    string msg = await response.Content.ReadAsStringAsync();
                    if (!msg.EndsWith(": Previous play had validation errors"))
                    {
                        Console.WriteLine(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inside CALLAPI: " + methodName + " : " + jsonContent + " : " + ex.GetBaseException().Message);
            }
        } // CallAPI
    } // Program
} // CQRSLite_Retrosheet.LoadGames
