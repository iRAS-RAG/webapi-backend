using System.Collections.Generic;

namespace IRasRag.Application.DTOs.Metrics
{
    public class BatchHistoryDto
    {
        public Guid BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public List<TimeSeriesPointDto> FeedSeries { get; set; } = new List<TimeSeriesPointDto>();
        public List<TimeSeriesPointDto> MortalitySeries { get; set; } =
            new List<TimeSeriesPointDto>();
        public List<TimeSeriesPointDto> CountSeries { get; set; } = new List<TimeSeriesPointDto>();
        public List<TimeSeriesPointDto> FcrSeries { get; set; } = new List<TimeSeriesPointDto>();
    }
}
