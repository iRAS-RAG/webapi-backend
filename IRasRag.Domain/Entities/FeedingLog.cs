using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class FeedingLog : BaseEntity
    {
        [Required]
        public Guid FarmingBatchId { get; set; }

        [Required]
        public float Amount { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public FarmingBatch FarmingBatch { get; set; }
    }
}
