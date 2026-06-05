namespace IRasRag.Application.DTOs.Metrics
{
    public class BatchSummaryDto
    {
        public Guid BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; } = string.Empty;
        public int InitialQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public double TotalFeedKg { get; set; }
        public int TotalDeaths { get; set; }
        public double TotalDeadWeightKg { get; set; }
        public double? Fcr { get; set; }
    }
}
