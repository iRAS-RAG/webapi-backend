using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class JobTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // Create DTO
    public class CreateJobTypeDto
    {
        [Required(ErrorMessage = "Tên loại công việc là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên loại công việc không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        public string Description { get; set; }
    }

    // Update DTO
    public class UpdateJobTypeDto
    {
        [MaxLength(255, ErrorMessage = "Tên loại công việc không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        public string? Description { get; set; }
    }
}
