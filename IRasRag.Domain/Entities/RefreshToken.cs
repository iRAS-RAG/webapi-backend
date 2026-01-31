using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string TokenHash { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        // Navigation properties
        public User User { get; set; }
    }
}
