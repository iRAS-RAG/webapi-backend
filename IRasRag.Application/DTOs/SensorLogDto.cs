using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // Response DTO
    public class SensorLogDto
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public string SensorName { get; set; } = string.Empty;
        public double Average { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public int SampleCount { get; set; }
        public bool HasWarning { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // SensorLog List Request (chart query)
    public class SensorLogListRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        /// <summary>Interval tính bằng phút để gộp nhóm dữ liệu. Nếu null, trả về dữ liệu thô.</summary>
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Interval phải là số nguyên dương (tối thiểu 1 phút)"
        )]
        public int? Interval { get; set; }

        /// <summary>Số trang hiện tại (mặc định: 1).</summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page phải lớn hơn 0")]
        public int Page { get; set; } = 1;

        /// <summary>Số bản ghi mỗi trang (mặc định: 50, tối đa: 500).</summary>
        [Range(1, 500, ErrorMessage = "PageSize phải từ 1 đến 500")]
        public int PageSize { get; set; } = 50;
    }

    // Create SensorLog DTO (manual entry)
    public class CreateSensorLogDto
    {
        [Required(ErrorMessage = "Giá trị dữ liệu là bắt buộc")]
        public double Data { get; set; }

        /// <summary>Custom timestamp for the log entry. Defaults to current UTC time if not provided.</summary>
        public DateTime? Timestamp { get; set; }
    }

    public class LatestSensorLogDto
    {
        public string SensorName { get; set; } = string.Empty;
        public string SensorTypeName { get; set; } = string.Empty;
        public double LastValue { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
