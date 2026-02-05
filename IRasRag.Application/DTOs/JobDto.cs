using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class JobDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid JobTypeId { get; set; }
        public string JobTypeName { get; set; }
        public Guid? SensorId { get; set; }
        public string? SensorName { get; set; }
        public float? MinValue { get; set; }
        public float? MaxValue { get; set; }
        public bool DefaultState { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? RepeatIntervalMinutes { get; set; }
        public string ExecutionDays { get; set; }
    }

    // Create DTO
    public class CreateJobDto
    {
        [Required(ErrorMessage = "Tên công việc là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên công việc không được vượt quá 255 ký tự")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Id loại công việc là bắt buộc")]
        public Guid JobTypeId { get; set; }

        public Guid? SensorId { get; set; }

        public float? MinValue { get; set; }

        public float? MaxValue { get; set; }

        public bool DefaultState { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Khoảng thời gian lặp lại phải lớn hơn 0")]
        public int? RepeatIntervalMinutes { get; set; }

        [MaxLength(20, ErrorMessage = "Ngày thực thi không được vượt quá 20 ký tự")]
        public string? ExecutionDays { get; set; }
    }

    // Update DTO
    public class UpdateJobDto
    {
        [MaxLength(255, ErrorMessage = "Tên công việc không được vượt quá 255 ký tự")]
        public string? Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        public Guid? JobTypeId { get; set; }

        public Guid? SensorId { get; set; }

        public float? MinValue { get; set; }

        public float? MaxValue { get; set; }

        public bool? DefaultState { get; set; }

        public bool? IsActive { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Khoảng thời gian lặp lại phải lớn hơn 0")]
        public int? RepeatIntervalMinutes { get; set; }

        [MaxLength(20, ErrorMessage = "Ngày thực thi không được vượt quá 20 ký tự")]
        public string? ExecutionDays { get; set; }
    }
}
