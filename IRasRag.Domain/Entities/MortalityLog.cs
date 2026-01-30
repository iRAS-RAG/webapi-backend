using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class MortalityLog : BaseEntity
    {
        [Required]
        public Guid BatchId { get; set; }

        [Required]
        public float Quantity { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation properties
        public FarmingBatch Batch { get; set; }
    }
}
