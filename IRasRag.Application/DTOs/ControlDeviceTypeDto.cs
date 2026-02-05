using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class ControlDeviceTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateControlDeviceTypeDto
    {
        [Required(ErrorMessage = "Tên loại thiết bị điều khiển là bắt buộc")]
        [MaxLength(
            255,
            ErrorMessage = "Tên loại thiết bị điều khiển không được vượt quá 255 ký tự"
        )]
        public string Name { get; set; }

        public string? Description { get; set; }
    }

    // Update DTO
    public class UpdateControlDeviceTypeDto
    {
        [MaxLength(
            255,
            ErrorMessage = "Tên loại thiết bị điều khiển không được vượt quá 255 ký tự"
        )]
        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
