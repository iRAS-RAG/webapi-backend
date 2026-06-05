using System.Collections.Generic;

namespace IRasRag.Application.DTOs.Metrics
{
    public class TimeSeriesPointDto
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

    public class TimeSeriesSeriesDto
    {
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public List<TimeSeriesPointDto> Points { get; set; } = new List<TimeSeriesPointDto>();
    }

    public class TimeSeriesResponseDto
    {
        public string Metric { get; set; } = string.Empty;
        public List<TimeSeriesSeriesDto> Series { get; set; } = new List<TimeSeriesSeriesDto>();
    }
}
