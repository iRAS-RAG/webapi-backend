using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class CorrectiveActionDto
    {
        public Guid Id { get; set; }
        public Guid AlertId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string ActionTaken { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    // Create DTO
    public class CreateCorrectiveActionDto
    {
        [Required(ErrorMessage = "Id cảnh báo là bắt buộc")]
        public Guid AlertId { get; set; }

        [Required(ErrorMessage = "Id người dùng là bắt buộc")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Hành động khắc phục là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Hành động khắc phục không được vượt quá 255 ký tự")]
        public string ActionTaken { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }

    // Update DTO
    public class UpdateCorrectiveActionDto
    {
        public Guid? AlertId { get; set; }

        public Guid? UserId { get; set; }

        [MaxLength(255, ErrorMessage = "Hành động khắc phục không được vượt quá 255 ký tự")]
        public string? ActionTaken { get; set; }

        public string? Notes { get; set; }
    }
}
