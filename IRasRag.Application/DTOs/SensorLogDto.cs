namespace IRasRag.Application.DTOs
{
    public class SensorLogDto
    {
        public string SensorName { get; set; } = string.Empty;
        public string SensorTypeName { get; set; } = string.Empty;
        public double LastValue { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
