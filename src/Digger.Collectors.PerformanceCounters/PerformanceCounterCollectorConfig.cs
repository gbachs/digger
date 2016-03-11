using System;
using System.Collections.Generic;

namespace Digger.Collectors.PerformanceCounters
{
    public class PerformanceCounterCollectorConfig
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public TimeSpan CollectInterval { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public PerformanceCounterConfig PerformanceCounterConfig { get; set; }
    }
}