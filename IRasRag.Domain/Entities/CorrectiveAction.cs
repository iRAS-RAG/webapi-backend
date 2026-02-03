using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;

namespace IRasRag.Domain.Entities
{
    public class CorrectiveAction : BaseEntity
    {
        [Required]
        public Guid AlertId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ActionTaken { get; set; }

        public string Notes { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public Alert Alert { get; set; }
        public User User { get; set; }
    }
}
