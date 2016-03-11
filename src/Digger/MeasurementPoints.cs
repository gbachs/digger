using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace Digger
{
    public class MeasurementPoints : IEnumerable<MeasurementPoint>
    {
        private readonly List<MeasurementPoint> _points = new List<MeasurementPoint>();

        public MeasurementPoint this[int index] => _points[index];

        public int Count => _points.Count;

        public bool IsEmpty => !_points.Any();

        public MeasurementPoints Add(MeasurementPoint point)
        {
            _points.Add(point);

            return this;
        }

        public MeasurementPoints Add(IEnumerable<MeasurementPoint> points)
        {
            Ensure.That(points, nameof(points)).IsNotNull();

            _points.AddRange(points);

            return this;
        }

        public void Clear()
        {
            _points.Clear();
        }

        public IEnumerator<MeasurementPoint> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _points.GetEnumerator();
        }
    }
}