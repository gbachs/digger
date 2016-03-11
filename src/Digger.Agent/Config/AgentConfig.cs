using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Newtonsoft.Json.Linq;

namespace Digger.Agent.Config
{
    public class AgentConfig
    {
        public IEnumerable<JToken> Collectors { get; }
        public string DispatcherType { get; }
        public ProcessorsConfig Processors { get; }

        public AgentConfig(JToken json)
        {
            Ensure.That(json, nameof(json)).IsNotNull();

            Collectors = json.SelectToken("collectors").ToArray();
            DispatcherType = json.SelectToken("dispatcherType").ToObject<string>();
            Processors = json.SelectToken("processors").ToObject<ProcessorsConfig>();
        }
    }
}