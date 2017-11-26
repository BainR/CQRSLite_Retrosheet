using CQRSLite_Retrosheet.Domain.ReadModel;
using System;
using System.Collections.Concurrent;

namespace CQRSLite_Retrosheet.Domain.Dictionaries
{
    public static class Dictionaries
    {
        public static ConcurrentDictionary<string, BaseballPlayRM> cdBaseballPlayRM = 
            new ConcurrentDictionary<string, BaseballPlayRM>(Environment.ProcessorCount * 2, 257);

        public static ConcurrentDictionary<string, LineupRM> cdLineupRM =
            new ConcurrentDictionary<string, LineupRM>(Environment.ProcessorCount * 2, 257);
    }
}
