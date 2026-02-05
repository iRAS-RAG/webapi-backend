using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class ControlDeviceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PinCode { get; set; }
        public bool State { get; set; }
        public string CommandOn { get; set; }
        public string CommandOff { get; set; }
        public Guid MasterBoardId { get; set; }
        public string MasterBoardName { get; set; }
        public Guid ControlDeviceTypeId { get; set; }
        public string ControlDeviceTypeName { get; set; }
    }

    // Create DTO
    public class CreateControlDeviceDto
    {
        [Required(ErrorMessage = "Tên thiết bị điều khiển là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên thiết bị điều khiển không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mã chân là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Mã chân phải là số không âm")]
        public int PinCode { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool State { get; set; }

        [Required(ErrorMessage = "Lệnh bật là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Lệnh bật không được vượt quá 50 ký tự")]
        public string CommandOn { get; set; }

        [Required(ErrorMessage = "Lệnh tắt là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Lệnh tắt không được vượt quá 50 ký tự")]
        public string CommandOff { get; set; }

        [Required(ErrorMessage = "Id bảng mạch là bắt buộc")]
        public Guid MasterBoardId { get; set; }

        [Required(ErrorMessage = "Id loại thiết bị điều khiển là bắt buộc")]
        public Guid ControlDeviceTypeId { get; set; }
    }

    // Update DTO
    public class UpdateControlDeviceDto
    {
        [MaxLength(255, ErrorMessage = "Tên thiết bị điều khiển không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mã chân phải là số không âm")]
        public int? PinCode { get; set; }

        public bool? State { get; set; }

        [MaxLength(50, ErrorMessage = "Lệnh bật không được vượt quá 50 ký tự")]
        public string? CommandOn { get; set; }

        [MaxLength(50, ErrorMessage = "Lệnh tắt không được vượt quá 50 ký tự")]
        public string? CommandOff { get; set; }

        public Guid? MasterBoardId { get; set; }

        public Guid? ControlDeviceTypeId { get; set; }
    }
}
