using System.ComponentModel.DataAnnotations;
using IRasRag.Domain.Common;
using IRasRag.Domain.Enums;

namespace IRasRag.Domain.Entities
{
    public class Document : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Content { get; set; }

        [Required]
        public Guid UploadedByUserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileUrl { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; }

        [Required]
        public DocumentRagStatus RagStatus { get; set; } = DocumentRagStatus.Pending;

        // Navigation properties
        public ICollection<Recommendation> Recommendations { get; set; }
        public User UploadedByUser { get; set; }
    }
}
