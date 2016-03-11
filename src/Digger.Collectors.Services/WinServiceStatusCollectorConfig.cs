using System;
using System.Collections.Generic;

namespace Digger.Collectors.WinServices
{
    public class WinServiceStatusCollectorConfig
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public TimeSpan CollectInterval { get; set; }
        public bool IncludeProcessInfo { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] ServiceNames { get; set; }
    }
}