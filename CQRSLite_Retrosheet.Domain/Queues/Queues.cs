using CQRSLite_Retrosheet.Domain.ReadModel;
using CQRSLite_Retrosheet.Domain.Requests;
using System;
using System.Collections.Concurrent;

namespace CQRSLite_Retrosheet.Domain.Queues
{
    public static class Queues
    {
        public static ConcurrentDictionary<string, CreateGameSummaryRequest> cdCreateGameSummaryRequest =
            new ConcurrentDictionary<string, CreateGameSummaryRequest>(Environment.ProcessorCount * 2, 257);

        public static ConcurrentDictionary<string, GameSummaryRM> cdGameSummaryRM =
            new ConcurrentDictionary<string, GameSummaryRM>(Environment.ProcessorCount * 2, 257);

        public static ConcurrentDictionary<string, CreateBaseballPlayRequest> cdCreateBaseballPlayRequest = 
            new ConcurrentDictionary<string, CreateBaseballPlayRequest>(Environment.ProcessorCount * 2, 257);

        public static ConcurrentDictionary<string, BaseballPlayRM> cdBaseballPlayRM = 
            new ConcurrentDictionary<string, BaseballPlayRM>(Environment.ProcessorCount * 2, 257);

        public static ConcurrentDictionary<string, CreateLineupChangeRequest> cdCreateLineupChangeRequest =
            new ConcurrentDictionary<string, CreateLineupChangeRequest>(Environment.ProcessorCount * 2, 257);

        public static ConcurrentDictionary<string, LineupRM> cdLineupRM =
            new ConcurrentDictionary<string, LineupRM>(Environment.ProcessorCount * 2, 257);
    }
}
