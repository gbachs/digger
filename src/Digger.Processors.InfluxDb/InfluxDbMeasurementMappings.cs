using System.Collections.Generic;
using EnsureThat;

namespace Digger.Processors.InfluxDb
{
    public class InfluxDbMeasurementMappings
    {
        private readonly Dictionary<string, InfluxDbTargetInfo> _registrations = new Dictionary<string, InfluxDbTargetInfo>();

        public InfluxDbTargetInfo DefaultDbTargetInfo { get; }

        public bool IsEmpty => _registrations.Count == 0;

        public InfluxDbMeasurementMappings(InfluxDbTargetInfo defaultDbTargetInfo)
        {
            Ensure.That(defaultDbTargetInfo, nameof(defaultDbTargetInfo)).IsNotNull();

            DefaultDbTargetInfo = defaultDbTargetInfo;
        }

        public void Register<T>(InfluxDbTargetInfo dbTargetInfo) where T : IMeasurement
        {
            Ensure.That(dbTargetInfo, nameof(dbTargetInfo)).IsNotNull();

            _registrations.Add(typeof(T).Name, dbTargetInfo);
        }

        public InfluxDbTargetInfo GetDbTargetInfoOrDefault(IMeasurement measurement)
        {
            Ensure.That(measurement, nameof(measurement)).IsNotNull();

            return GetDbTargetInfoOrDefault(measurement.Type);
        }

        private InfluxDbTargetInfo GetDbTargetInfoOrDefault(string type)
        {
            InfluxDbTargetInfo r;

            return _registrations.TryGetValue(type, out r) ? r : DefaultDbTargetInfo;
        }
    }
}