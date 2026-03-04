using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    // Không bao gồm Mappings — danh sách thiết bị liên kết được truy cập riêng
    // qua endpoint /job-control-mappings?jobId={id} để tránh over-fetching.
    public class JobDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid JobTypeId { get; set; }
        public string JobTypeName { get; set; } = string.Empty;
        public Guid? SensorId { get; set; }
        public string? SensorName { get; set; }
        public float? MinValue { get; set; }
        public float? MaxValue { get; set; }
        public bool DefaultState { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? RepeatIntervalMinutes { get; set; }
        public string? ExecutionDays { get; set; }
    }

    // Create DTO
    public class CreateJobDto
    {
        [Required(ErrorMessage = "Tên công việc là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên công việc không được vượt quá 255 ký tự")]
        public string Name { get; set; } = string.Empty;

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

        // Danh sách mapping thiết bị điều khiển (tùy chọn, tạo kèm khi tạo job)
        public List<CreateJobMappingItemDto>? Mappings { get; set; }
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

        // Nếu được cung cấp, toàn bộ danh sách mappings hiện tại sẽ bị thay thế bằng danh sách mới.
        // Truyền mảng rỗng [] để xóa hết tất cả mappings.
        // Không truyền field này (null) để giữ nguyên mappings hiện tại.
        public List<CreateJobMappingItemDto>? Mappings { get; set; }
    }

    // List Request DTO
    public class JobListRequest : BasePaginatedListRequest
    {
        public bool? DefaultState { get; set; }
        public bool? IsActive { get; set; }
    }
}
