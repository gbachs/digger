using EnsureThat;

namespace Digger.Collectors.PerformanceCounters
{
    public class PerformanceCounterConfig
    {
        public string CategoryName { get; private set; }
        public string CounterName { get; private set; }
        public string InstanceName { get; private set; }

        public PerformanceCounterConfig(string categoryName, string counterName, string instanceName = null)
        {
            Ensure.That(categoryName, nameof(categoryName)).IsNotNullOrWhiteSpace();
            Ensure.That(counterName, nameof(counterName)).IsNotNullOrWhiteSpace();

            CategoryName = categoryName;
            CounterName = counterName;
            InstanceName = instanceName;
        }
    }
}