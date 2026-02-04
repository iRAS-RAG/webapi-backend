using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class SensorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PinCode { get; set; }
        public Guid SensorTypeId { get; set; }
        public string SensorTypeName { get; set; }
        public Guid MasterBoardId { get; set; }
        public string MasterBoardName { get; set; }
    }

    // Create DTO
    public class CreateSensorDto
    {
        [Required(ErrorMessage = "Tên cảm biến là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên cảm biến không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mã chân là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Mã chân phải là số không âm")]
        public int PinCode { get; set; }

        [Required(ErrorMessage = "Id loại cảm biến là bắt buộc")]
        public Guid SensorTypeId { get; set; }

        [Required(ErrorMessage = "Id bảng mạch là bắt buộc")]
        public Guid MasterBoardId { get; set; }
    }

    // Update DTO
    public class UpdateSensorDto
    {
        [MaxLength(255, ErrorMessage = "Tên cảm biến không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mã chân phải là số không âm")]
        public int? PinCode { get; set; }

        public Guid? SensorTypeId { get; set; }

        public Guid? MasterBoardId { get; set; }
    }
}
