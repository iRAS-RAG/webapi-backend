using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    public class HarvestBatchRequest
    {
        [Required(ErrorMessage = "Ngày thu hoạch là bắt buộc")]
        public DateTime HarvestDate { get; set; }

        // If true, allow harvesting earlier than planned end date
        public bool Force { get; set; } = false;

        // Optional: total actual harvested weight in kg
        public double? ActualHarvestWeightKg { get; set; }
    }
}
