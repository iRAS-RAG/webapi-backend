using IRasRag.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace IRasRag.Domain.Entities
{
    public class Document : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Guid UploadedByUserId { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; }

        // Navigation properties
        public ICollection<Recommendation> Recommendations { get; set; }
        public User UploadedByUser { get; set; }
    }
}
