using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class MortalityLog : BaseEntity
    {
        [Required]
        public Guid BatchId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation properties
        public FarmingBatch Batch { get; set; }
        public User User { get; set; }
    }
}
