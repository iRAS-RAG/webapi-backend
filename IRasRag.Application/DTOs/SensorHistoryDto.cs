namespace IRasRag.Application.DTOs
{
    public class SensorHistoryPointDto
    {
        public DateTimeOffset RecordedAt { get; set; }
        public double? Value { get; set; }
    }
}
