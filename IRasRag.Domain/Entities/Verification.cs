using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class Verification : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string CodeHash { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        [Required]
        public bool IsConsumed { get; set; }

        // Navigation properties
        public User User { get; set; }
    }
}
