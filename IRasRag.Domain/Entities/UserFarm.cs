using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class UserFarm : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid FarmId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Farm Farm { get; set; }
    }
}
