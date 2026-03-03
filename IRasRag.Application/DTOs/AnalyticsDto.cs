using System.ComponentModel.DataAnnotations;

namespace IRasRag.Application.DTOs
{
    // ─────────────────────────────────────────────────────────────────────────
    // Analytics – F11: Compare batches / tanks
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Query parameters for GET /api/analytics/compare.
    /// </summary>
    public class BatchCompareRequest
    {
        /// <summary>
        /// List of FarmingBatch IDs to compare (at least 1 is required).
        /// </summary>
        public List<Guid> BatchIds { get; set; } = new();

        /// <summary>
        /// Metrics to include in the response.
        /// Supported values (case-insensitive):
        ///   survival_rate | mortality | feeding | alerts
        ///   and any sensor-type metric names (e.g. do, temperature, ph, ammonia …).
        /// Leave empty to return ALL available metrics.
        /// </summary>
        public List<string> Metrics { get; set; } = new();

        /// <summary>
        /// ID của user hiện tại. Được set bởi Controller từ JWT claims.
        /// </summary>
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Top-level response returned by the comparison endpoint.
    /// </summary>
    public class BatchCompareResponseDto
    {
        /// <summary>UTC timestamp when the comparison was computed.</summary>
        public DateTime ComparedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Normalised list of metric keys that were evaluated.
        /// Reflects the effective metrics after merging the request with what is available.
        /// </summary>
        public List<string> EvaluatedMetrics { get; set; } = new();

        /// <summary>One entry per requested batch.</summary>
        public List<BatchMetricsDto> Batches { get; set; } = new();
    }

    /// <summary>
    /// Statistics for a single FarmingBatch.
    /// </summary>
    public class BatchMetricsDto
    {
        public Guid BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; } = string.Empty;

        /// <summary>FarmingBatchStatus as string (ACTIVE, HARVESTED, …).</summary>
        public string Status { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime? EstimatedHarvestDate { get; set; }
        public DateTime? ActualHarvestDate { get; set; }

        /// <summary>
        /// Computed metric values for this batch.
        /// Only the keys requested via the Metrics query parameter are populated
        /// (or all keys when the parameter is omitted).
        /// </summary>
        public BatchMetricValuesDto MetricValues { get; set; } = new();
    }

    /// <summary>
    /// Holds all possible computed metrics for one batch.
    /// Null means the metric was not requested or could not be computed.
    /// </summary>
    public class BatchMetricValuesDto
    {
        /// <summary>
        /// Tỷ lệ sống sót: (CurrentQuantity / InitialQuantity) × 100 (%).
        /// </summary>
        public double? SurvivalRate { get; set; }

        /// <summary>
        /// Tổng số cá chết trong lô (MortalityLog.Quantity).
        /// </summary>
        public double? TotalMortality { get; set; }

        /// <summary>
        /// Tổng lượng thức ăn đã cho ăn trong lô (FeedingLog.Amount, cùng đơn vị).
        /// </summary>
        public double? TotalFeeding { get; set; }

        /// <summary>
        /// Tổng số cảnh báo phát sinh trong bể trong thời gian lô tồn tại.
        /// </summary>
        public int? TotalAlerts { get; set; }

        /// <summary>
        /// Phân tích số cảnh báo theo từng loại cảm biến (SensorType.Name → count).
        /// </summary>
        public Dictionary<string, int> AlertsByType { get; set; } = new();

        /// <summary>
        /// Giá trị trung bình của mỗi loại cảm biến (SensorType.Name → average value).
        /// Đơn vị đo theo SensorType.UnitOfMeasure.
        /// </summary>
        public Dictionary<string, double> SensorAverages { get; set; } = new();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Analytics – F12: Alert frequency stats
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Query parameters for GET /api/analytics/alert-frequency.
    /// </summary>
    public class AlertFrequencyRequest
    {
        /// <summary>
        /// Thời điểm bắt đầu khoảng thời gian cần thống kê (UTC).
        /// Mặc định: 30 ngày trước.
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Thời điểm kết thúc khoảng thời gian cần thống kê (UTC).
        /// Mặc định: hiện tại.
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// Lọc theo bể nuôi cụ thể (tuỳ chọn).
        /// </summary>
        public Guid? FishTankId { get; set; }

        /// <summary>
        /// Lọc theo trang trại cụ thể (tuỳ chọn).
        /// </summary>
        public Guid? FarmId { get; set; }

        /// <summary>
        /// Số loại cảnh báo hiển thị trong danh sách (mặc định: 10, tối đa: 50).
        /// </summary>
        [Range(1, 50, ErrorMessage = "TopN phải từ 1 đến 50.")]
        public int TopN { get; set; } = 10;

        /// <summary>
        /// ID của user hiện tại. Được set bởi Controller từ JWT claims.
        /// </summary>
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Internal lightweight projection used by the AlertFrequencySpec.
    /// Not exposed to API consumers.
    /// </summary>
    public class AlertFrequencyProjection
    {
        public Guid SensorTypeId { get; set; }
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; } = string.Empty;
        public string StatusStr { get; set; } = string.Empty;
        public DateTime RaisedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    /// <summary>
    /// Thống kê tần suất cảnh báo theo từng loại cảm biến.
    /// </summary>
    public class AlertFrequencyItemDto
    {
        public Guid SensorTypeId { get; set; }

        /// <summary>Tên loại cảm biến (ví dụ: DO, Nhiệt độ, Amoniac).</summary>
        public string SensorTypeName { get; set; } = string.Empty;

        /// <summary>Loại chỉ số đo (MeasureType).</summary>
        public string MeasureType { get; set; } = string.Empty;

        /// <summary>Đơn vị đo lường.</summary>
        public string UnitOfMeasure { get; set; } = string.Empty;

        /// <summary>Tổng số cảnh báo loại này trong khoảng thời gian.</summary>
        public int TotalCount { get; set; }

        /// <summary>Tỷ lệ phần trăm so với tổng số cảnh báo (%).</summary>
        public double Percentage { get; set; }

        /// <summary>Số cảnh báo đang mở (OPEN).</summary>
        public int OpenCount { get; set; }

        /// <summary>Số cảnh báo đã ghi nhận (ACKNOWLEDGED).</summary>
        public int AcknowledgedCount { get; set; }

        /// <summary>Số cảnh báo đã giải quyết (RESOLVED).</summary>
        public int ResolvedCount { get; set; }

        /// <summary>Số cảnh báo đã bỏ qua (DISMISSED).</summary>
        public int DismissedCount { get; set; }

        /// <summary>
        /// Thời gian xử lý trung bình (phút) tính trên các cảnh báo đã RESOLVED.
        /// Null nếu không có cảnh báo nào được giải quyết.
        /// </summary>
        public double? AverageResolutionMinutes { get; set; }

        /// <summary>
        /// Top bể nuôi có nhiều cảnh báo loại này nhất.
        /// </summary>
        public List<TankAlertCountDto> TopTanks { get; set; } = new();
    }

    /// <summary>
    /// Số lượng cảnh báo theo bể nuôi.
    /// </summary>
    public class TankAlertCountDto
    {
        public Guid FishTankId { get; set; }
        public string FishTankName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    /// <summary>
    /// Số cảnh báo phát sinh theo ngày (dùng cho biểu đồ xu hướng).
    /// </summary>
    public class DailyAlertTrendDto
    {
        /// <summary>Ngày (UTC, bỏ giờ/phút/giây).</summary>
        public DateTime Date { get; set; }

        /// <summary>Tổng số cảnh báo trong ngày đó.</summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// Kết quả trả về từ GET /api/analytics/alert-frequency.
    /// </summary>
    public class AlertFrequencyResponseDto
    {
        /// <summary>Thời điểm bắt đầu khoảng thống kê (UTC).</summary>
        public DateTime From { get; set; }

        /// <summary>Thời điểm kết thúc khoảng thống kê (UTC).</summary>
        public DateTime To { get; set; }

        /// <summary>Tổng số cảnh báo trong khoảng thời gian.</summary>
        public int TotalAlerts { get; set; }

        /// <summary>
        /// Danh sách thống kê tần suất theo loại cảnh báo, sắp xếp giảm dần theo số lượng.
        /// Giới hạn theo TopN từ request.
        /// </summary>
        public List<AlertFrequencyItemDto> ByAlertType { get; set; } = new();

        /// <summary>
        /// Xu hướng cảnh báo theo ngày trong khoảng thời gian đã chọn.
        /// </summary>
        public List<DailyAlertTrendDto> DailyTrend { get; set; } = new();
    }
}
