using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class SensorTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MeasureType { get; set; }
        public string UnitOfMeasure { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    // Create DTO
    public class CreateSensorTypeDto
    {
        [Required(ErrorMessage = "Tên loại cảm biến là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên loại cảm biến không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Loại đo là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Loại đo không được vượt quá 50 ký tự")]
        public string MeasureType { get; set; }

        [Required(ErrorMessage = "Đơn vị đo là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Đơn vị đo không được vượt quá 50 ký tự")]
        public string UnitOfMeasure { get; set; }
    }

    // Update DTO
    public class UpdateSensorTypeDto
    {
        [MaxLength(255, ErrorMessage = "Tên loại cảm biến không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        [MaxLength(50, ErrorMessage = "Loại đo không được vượt quá 50 ký tự")]
        public string? MeasureType { get; set; }

        [MaxLength(50, ErrorMessage = "Đơn vị đo không được vượt quá 50 ký tự")]
        public string? UnitOfMeasure { get; set; }
    }
}
