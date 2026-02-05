using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid UploadedByUserId { get; set; }
        public string UploadedByUserEmail { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    // Create DTO
    public class CreateDocumentDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Id người tải lên là bắt buộc")]
        public Guid UploadedByUserId { get; set; }
    }

    // Update DTO
    public class UpdateDocumentDto
    {
        [MaxLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string? Title { get; set; }

        public string? Content { get; set; }
    }
}
