using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digger.Agent.Processors
{
    public class ConsoleProcessor : IProcessor
    {
        public Task ProcessAsync(IMeasurement measurement)
        {
            Write(w =>
            {
                foreach (var influxPoint in measurement.Points)
                {
                    w($"name={influxPoint.Measurement}");
                    w(GetTagsString(influxPoint));
                    foreach (var field in influxPoint.ValueTypeFields)
                        w($"{field.Key}={field.Value}");
                }
            });

            return Task.FromResult(0);
        }

        private static string GetTagsString(MeasurementPoint point)
        {
            return point.Tags != null && point.Tags.Any()
                ? string.Join(" ", point.Tags.Select(kv => $"{kv.Key}={kv.Value}"))
                : null;
        }

        private void Write(Action<Action<string>> writer)
        {
            var content = new StringBuilder();

            writer(s =>
            {
                if (s != null)
                    content.Append(s).Append(" ");
            });

            Console.WriteLine(content.ToString().Trim());
        }
    }
}