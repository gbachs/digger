namespace Digger
{
    public interface IMeasurement
    {
        string Type { get; }
        MeasurementPoints Points { get; }
    }
}