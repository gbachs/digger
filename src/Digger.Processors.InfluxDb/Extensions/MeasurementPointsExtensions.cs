using System.Linq;
using MyInfluxDbClient;

namespace Digger.Processors.InfluxDb.Extensions
{
    internal static class MeasurementPointsExtensions
    {
        internal static InfluxPoints ToInfluxPoints(this MeasurementPoints points)
        {
            return new InfluxPoints
            {
                points.Select(p => p.ToInfluxPoint())
            };
        }

        internal static InfluxPoint ToInfluxPoint(this MeasurementPoint point)
        {
            var influxPoint = new InfluxPoint(point.Measurement)
                .AddTags(point.Tags)
                .AddFields(point.ValueTypeFields)
                .AddFields(point.StringFields);

            return influxPoint;
        }
    }
}