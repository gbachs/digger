namespace Digger
{
    public abstract class Measurement<T> : IMeasurement
    {
        public string Type { get; } = typeof(T).Name;
        public MeasurementPoints Points { get; }

        protected Measurement(MeasurementPoint point) : this(new MeasurementPoints().Add(point)) { }

        protected Measurement(MeasurementPoints points)
        {
            Points = points;
        }
    }
}