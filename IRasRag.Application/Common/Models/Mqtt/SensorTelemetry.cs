namespace IRasRag.Application.Common.Models.Mqtt
{
    public class SensorTelemetry
    {
        public string Mac { get; set; }
        public IReadOnlyList<SensorReading> Readings { get; set; }
    }

    public class SensorReading
    {
        public int Pin { get; set; }
        public double Val { get; set; }
    }
}
