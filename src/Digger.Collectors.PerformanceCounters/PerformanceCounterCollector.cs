using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Digger.Extensions;

namespace Digger.Collectors.PerformanceCounters
{
    public class PerformanceCounterCollector : AsyncPollingCollectorBase
    {
        private PerformanceCounter _counter;

        public PerformanceCounterCollector(PerformanceCounterCollectorConfig config, IDispatcher dispatcher, ILogger logger)
            : base(config.Name, config.CollectInterval, dispatcher, logger)
        {
            var performanceCounterConfig = config.PerformanceCounterConfig;
            _counter = performanceCounterConfig.InstanceName != null
                ? new PerformanceCounter(performanceCounterConfig.CategoryName, performanceCounterConfig.CounterName, performanceCounterConfig.InstanceName, true)
                : new PerformanceCounter(performanceCounterConfig.CategoryName, performanceCounterConfig.CounterName, true);

            _counter.MachineName = config.Host ?? Environment.MachineName;
        }

        protected override Task OnStartingAsync()
        {
            _counter.NextValue();

            return Task.FromResult(0);
        }

        protected override async Task OnCollectAsync(IDispatcher dispatcher)
        {
            var value = _counter.NextValue();
            var point = new MeasurementPoint(Name)
                .AddTag("host", _counter.MachineName)
                .AddField("value", (decimal)value)
                .AddTimeStamp(SysTime.Now());

            var measurement = new PerformanceCounterMeasurement(point);

            await dispatcher.DispatchAsync(measurement).ForAwait();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (IsDisposed || !disposing)
                return;

            if (_counter != null)
            {
                _counter.Dispose();
                _counter = null;
            }
        }
    }
}