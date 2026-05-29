using System.Collections.Generic;

namespace IRasRag.Application.DTOs.Metrics
{
    public class FarmSummaryDto
    {
        public Guid FarmId { get; set; }
        public int TotalInitialQuantity { get; set; }
        public int TotalCurrentQuantity { get; set; }
        public double TotalFeedKg { get; set; }
        public int TotalDeathsCount { get; set; }
        public double TotalDeadWeightKg { get; set; }
        public int TotalHarvestedBatches { get; set; }
        public double TotalHarvestWeightKg { get; set; }
        public double? Fcr { get; set; }

        public List<BatchSummaryDto> Batches { get; set; } = new List<BatchSummaryDto>();
    }
}
