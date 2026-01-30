using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

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
