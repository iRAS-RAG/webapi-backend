using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class CameraDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Guid FarmId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateCameraDto
    {
        [Required(ErrorMessage = "Tên camera là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên camera không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "URL camera là bắt buộc")]
        [MaxLength(255, ErrorMessage = "URL camera không được vượt quá 255 ký tự")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Mã trang trại là bắt buộc")]
        public Guid FarmId { get; set; }
    }

    // Update DTO
    public class UpdateCameraDto
    {
        [MaxLength(255, ErrorMessage = "Tên camera không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        [MaxLength(255, ErrorMessage = "URL camera không được vượt quá 255 ký tự")]
        public string? Url { get; set; }

        public Guid? FarmId { get; set; }
    }
}
