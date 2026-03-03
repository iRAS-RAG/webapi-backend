using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class DocumentDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public Guid UploadedByUserId { get; set; }
        public string UploadedByUserEmail { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public Guid UploadedByUserId { get; set; }
        public string UploadedByUserEmail { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    // Create DTO
    public class CreateDocumentDto
    {
        [Required]
        public Stream FileStream { get; set; }

        [Required]
        public string FileTitle { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public long FileSize { get; set; }

        [Required]
        public Guid UploadedByUserId { get; set; }
    }

    // Update DTO
    public class UpdateDocumentDto
    {
        public string? Content { get; set; }
    }

    // List Request DTO
    public class DocumentListRequest : BasePaginatedListRequest { }
}
