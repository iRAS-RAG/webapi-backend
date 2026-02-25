using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class SensorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PinCode { get; set; }
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

    // List Request DTO
    public class SensorListRequest : BasePaginatedListRequest
    {
        public Guid? MasterBoardId { get; set; }
    }

    // SensorLog Response DTO
    public class SensorLogDto
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public double Data { get; set; }
        public bool IsWarning { get; set; }
        public string DataJson { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // SensorLog List Request (chart query)
    public class SensorLogListRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        /// <summary>Interval in minutes for downsampling. If null, returns raw logs.</summary>
        public int? Interval { get; set; }
    }

    // Create SensorLog DTO (manual entry)
    public class CreateSensorLogDto
    {
        [Required(ErrorMessage = "Giá trị dữ liệu là bắt buộc")]
        public double Data { get; set; }

        /// <summary>Custom timestamp for the log entry. Defaults to current UTC time if not provided.</summary>
        public DateTime? Timestamp { get; set; }
    }
}
