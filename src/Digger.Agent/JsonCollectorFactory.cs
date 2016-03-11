using System.Collections.Generic;
using System.Linq;
using Digger.Collectors.PerformanceCounters;
using Digger.Collectors.WebHooks;
using Digger.Collectors.WinServices;
using EnsureThat;
using Newtonsoft.Json.Linq;

namespace Digger.Agent
{
    public class JsonCollectorFactory : ICollectorFactory
    {
        private readonly IEnumerable<JToken> _collectorsJson;
        private readonly IDispatcher _dispatcher;
        private readonly LoggerResolver _loggerResolver;

        public JsonCollectorFactory(IEnumerable<JToken> collectorsJson, IDispatcher dispatcher, LoggerResolver loggerResolver)
        {
            Ensure.That(collectorsJson, nameof(collectorsJson)).IsNotNull();
            Ensure.That(dispatcher, nameof(dispatcher)).IsNotNull();

            _collectorsJson = collectorsJson;
            _dispatcher = dispatcher;
            _loggerResolver = loggerResolver;
        }

        public IEnumerable<ICollector> Create()
        {
            var collectors = new List<ICollector>();
            collectors.AddRange(CreatePerformanceCounterCollectors(_collectorsJson));
            collectors.AddRange(CreateWinServiceStatusCollectors(_collectorsJson));
            collectors.AddRange(CreateWebHooksCollectors(_collectorsJson));

            return collectors;
        }

        private IEnumerable<PerformanceCounterCollector> CreatePerformanceCounterCollectors(IEnumerable<JToken> collectors)
        {
            return collectors
                .Where(collector => collector["type"].ToObject<string>() == nameof(PerformanceCounterCollector))
                .Select(c => c.ToObject<PerformanceCounterCollectorConfig>())
                .Select(collectorConfig => new PerformanceCounterCollector(
                    collectorConfig,
                    _dispatcher,
                    _loggerResolver(typeof(PerformanceCounterCollector))));
        }

        private IEnumerable<WinServiceStatusCollector> CreateWinServiceStatusCollectors(IEnumerable<JToken> collectors)
        {
            return collectors
                .Where(collector => collector["type"].ToObject<string>() == nameof(WinServiceStatusCollector))
                .Select(c => c.ToObject<WinServiceStatusCollectorConfig>())
                .Select(collectorConfig => new WinServiceStatusCollector(
                    collectorConfig,
                    _dispatcher,
                    _loggerResolver(typeof(WinServiceStatusCollector))));
        }

        private IEnumerable<WebHooksCollector> CreateWebHooksCollectors(IEnumerable<JToken> collectors)
        {
            return collectors
                .Where(collector => collector["type"].ToObject<string>() == nameof(WebHooksCollector))
                .Select(c => c.ToObject<WebHooksCollectorConfig>())
                .Select(collectorConfig => new WebHooksCollector(
                    collectorConfig.BaseAddress,
                    AgentRuntime.Resolve));
        }
    }
}