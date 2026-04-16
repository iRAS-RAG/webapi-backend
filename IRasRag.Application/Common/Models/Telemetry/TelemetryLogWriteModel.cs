namespace IRasRag.Application.Common.Models.Telemetry
{
    public class TelemetryLogWriteModel
    {
        public Guid SensorId { get; set; }
        public double Data { get; set; }
        public bool IsWarning { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RequeueCount { get; set; } = 0;
    }
}