using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class Camera : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Url { get; set; }

        [Required]
        public Guid FarmId { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Farm Farm { get; set; }
    }
}
