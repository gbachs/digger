using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using Digger.Extensions;

namespace Digger.Collectors.WinServices
{
    public class WinServiceStatusCollector : AsyncPollingCollectorBase
    {
        private const string Query = "select ProcessId, Name, DisplayName, State from Win32_Service";

        private readonly string[] _serviceNames;
        private readonly string _host;
        private readonly bool _includeProcessInfo;
        private ManagementObjectSearcher _searcher;

        public WinServiceStatusCollector(WinServiceStatusCollectorConfig config, IDispatcher dispatcher, ILogger logger)
            : base(config.Name, config.CollectInterval, dispatcher, logger)
        {
            _serviceNames = config.ServiceNames;
            _host = config.Host ?? Environment.MachineName;
            _includeProcessInfo = config.IncludeProcessInfo;

            var options = new ConnectionOptions();
            if (!string.IsNullOrEmpty(config.Username))
            {
                options.Username = config.Username;
                options.Password = config.Password;
            }

            var scope = new ManagementScope($@"\\{_host}\root\cimv2", options);
            scope.Connect();

            _searcher = new ManagementObjectSearcher(scope, new ObjectQuery(Query));
        }

        protected override Task OnStartingAsync()
        {
            return Task.FromResult(0);
        }

        protected override async Task OnCollectAsync(IDispatcher dispatcher)
        {
            if (!_serviceNames.Any())
                return;

            var services = _searcher.Get();
            foreach (var service in services.Cast<ManagementObject>())
            {
                var svcName = (string)service.GetPropertyValue("Name");
                if (!_serviceNames.Contains(svcName))
                    continue;

                var state = service.GetPropertyValue("State") as string;

                var point = new MeasurementPoint(Name)
                    .AddTag("host", _host)
                    .AddTag("svcName", svcName)
                    .AddTag("displayName", (string)service.GetPropertyValue("DisplayName"))
                    .AddField("state", state)
                    .AddField("isRunning", state != null && state.ToLower() == "running")
                    .AddTimeStamp(SysTime.Now());

                if (_includeProcessInfo)
                    AppendProcessInfo(service, point);

                var measurement = new WinServiceStatusMeasurement(point);

                await dispatcher.DispatchAsync(measurement).ForAwait();
            }
        }

        private static void AppendProcessInfo(ManagementBaseObject service, MeasurementPoint point)
        {
            int processId;
            if (!int.TryParse(service.GetPropertyValue("ProcessId").ToString(), out processId))
                return;

            var process = Process.GetProcessById(processId);
            point
                .AddField("workingSetMb", (decimal) process.WorkingSet64/1024/1024)
                .AddField("threadCount", process.Threads.Count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (IsDisposed || !disposing)
                return;

            if (_searcher != null)
            {
                _searcher.Dispose();
                _searcher = null;
            }
        }
    }
}