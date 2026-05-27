using IRasRag.Domain.Enums;

namespace IRasRag.Application.DTOs
{
    /// <summary>
    /// Query parameters for the dashboard endpoint.
    /// period: TODAY | WEEK | MONTH | YEAR (default: MONTH)
    /// </summary>
    public class DashboardQueryRequest
    {
        /// <summary>
        /// Khoảng thời gian thống kê: TODAY, WEEK, MONTH, YEAR. Mặc định: MONTH.
        /// </summary>
        public ReportPeriod Period { get; set; } = ReportPeriod.MONTH;

        /// <summary>
        /// ID của user hiện tại, được set bởi Controller từ JWT claims.
        /// Dùng để lọc dữ liệu theo farm thuộc về user.
        /// </summary>
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Internal projection used by the dashboard query to fetch only the
    /// quantity columns from active FarmingBatch rows — avoids loading
    /// the full entity when computing the survival rate.
    /// </summary>
    public class BatchSurvivalProjection
    {
        public double InitialQuantity { get; set; }
        public double CurrentQuantity { get; set; }
    }

    /// <summary>
    /// KPI summary returned by GET /api/reports/dashboard.
    /// </summary>
    public class DashboardSummaryDto
    {
        /// <summary>Ngày bắt đầu của kỳ thống kê.</summary>
        public DateTime PeriodFrom { get; set; }

        /// <summary>Ngày kết thúc của kỳ thống kê.</summary>
        public DateTime PeriodTo { get; set; }

        /// <summary>Nhãn kỳ thống kê (ví dụ: "Tháng này").</summary>
        public string PeriodLabel { get; set; } = string.Empty;

        // ── Alert KPIs ──────────────────────────────────────────────
        /// <summary>Tổng số cảnh báo trong kỳ.</summary>
        public int TotalAlerts { get; set; }

        /// <summary>Số cảnh báo đang mở.</summary>
        public int OpenAlerts { get; set; }

        /// <summary>Số cảnh báo đã xác nhận.</summary>
        public int AcknowledgedAlerts { get; set; }

        /// <summary>Số cảnh báo đã xử lý.</summary>
        public int ResolvedAlerts { get; set; }

        /// <summary>Số cảnh báo đã bỏ qua.</summary>
        public int DismissedAlerts { get; set; }

        // ── Farming Batch KPIs ───────────────────────────────────────
        /// <summary>Số lô nuôi đang hoạt động.</summary>
        public int ActiveBatches { get; set; }

        /// <summary>Số lô nuôi đã thu hoạch trong kỳ.</summary>
        public int HarvestedBatches { get; set; }

        /// <summary>Tổng số lô nuôi (bao gồm mọi trạng thái) trong kỳ.</summary>
        public int TotalBatches { get; set; }

        // ── Survival KPIs ────────────────────────────────────────────
        /// <summary>
        /// Tỷ lệ sống sót trung bình (%) của các lô nuôi đang hoạt động.
        /// Được tính theo công thức: CurrentQuantity / InitialQuantity * 100.
        /// Trả về null nếu không có lô nuôi nào.
        /// </summary>
        public double? AverageSurvivalRate { get; set; }

        /// <summary>Tổng số lượng cá/tôm hiện có trong các lô đang hoạt động.</summary>
        public double TotalCurrentQuantity { get; set; }

        /// <summary>Tổng số lượng cá/tôm ban đầu của các lô đang hoạt động.</summary>
        public double TotalInitialQuantity { get; set; }

        /// <summary>Tổng số lượng chết trong kỳ (từ MortalityLog).</summary>
        public double TotalMortality { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Weekly Report DTOs (F10 – Supervisor)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Query parameters for the weekly report endpoint.
    /// period: current | last | 2 | 3 … (default: current)
    /// </summary>
    public class WeeklyReportQueryRequest
    {
        /// <summary>
        /// Tuần cần thống kê:
        ///   "current" – tuần hiện tại (Thứ Hai – Chủ Nhật),
        ///   "last"    – tuần trước,
        ///   số nguyên dương N – N tuần trước (2, 3, …).
        /// Mặc định: "current".
        /// </summary>
        public string Period { get; set; } = "current";

        /// <summary>
        /// ID của user hiện tại, được set bởi Controller từ JWT claims.
        /// Dùng để lọc dữ liệu theo farm thuộc về user.
        /// </summary>
        public Guid UserId { get; set; }

        public Guid? FarmId { get; set; }

        public Guid? BatchId { get; set; }
    }

    /// <summary>Phân tích tần suất cảnh báo theo loại cảm biến.</summary>
    public class AlertTypeBreakdownItem
    {
        /// <summary>Tên loại cảm biến (ví dụ: Nhiệt độ, pH, …).</summary>
        public string SensorTypeName { get; set; } = string.Empty;

        /// <summary>Số lần cảnh báo trong tuần.</summary>
        public int Count { get; set; }
    }

    /// <summary>Tóm tắt hành động khắc phục đã thực hiện trong tuần.</summary>
    public class WeeklyCorrectiveActionItem
    {
        public Guid Id { get; set; }
        public Guid AlertId { get; set; }
        public string ActionTaken { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    /// <summary>Tóm tắt khuyến nghị đã sử dụng trong tuần.</summary>
    public class WeeklyRecommendationItem
    {
        public Guid Id { get; set; }
        public Guid AlertId { get; set; }
        public string SuggestionText { get; set; } = string.Empty;
        public string DocumentTitle { get; set; } = string.Empty;
    }

    /// <summary>
    /// Báo cáo tuần tự động dành cho Supervisor (F10).
    /// Tổng hợp cảnh báo, sự cố chính, khuyến nghị đã dùng và sức khỏe lô nuôi.
    /// </summary>
    public class WeeklyReportDto
    {
        /// <summary>Thời điểm tạo báo cáo.</summary>
        public DateTime GeneratedAt { get; set; }

        /// <summary>Ngày bắt đầu tuần thống kê.</summary>
        public DateTime PeriodFrom { get; set; }

        /// <summary>Ngày kết thúc tuần thống kê.</summary>
        public DateTime PeriodTo { get; set; }

        /// <summary>Nhãn mô tả tuần (ví dụ: "Tuần 10/2026 (02/03 – 08/03)").</summary>
        public string PeriodLabel { get; set; } = string.Empty;

        // ── Alert KPIs ───────────────────────────────────────────────
        /// <summary>Tổng số cảnh báo trong tuần.</summary>
        public int TotalAlerts { get; set; }

        /// <summary>Số cảnh báo đang mở.</summary>
        public int OpenAlerts { get; set; }

        /// <summary>Số cảnh báo đã xác nhận.</summary>
        public int AcknowledgedAlerts { get; set; }

        /// <summary>Số cảnh báo đã xử lý.</summary>
        public int ResolvedAlerts { get; set; }

        /// <summary>Số cảnh báo đã bỏ qua.</summary>
        public int DismissedAlerts { get; set; }

        /// <summary>Top loại cảm biến gây cảnh báo nhiều nhất trong tuần (sắp xếp giảm dần).</summary>
        public List<AlertTypeBreakdownItem> TopIssuesBySensorType { get; set; } = [];

        // ── Corrective Actions ───────────────────────────────────────
        /// <summary>Tổng số hành động khắc phục đã thực hiện trong tuần.</summary>
        public int TotalCorrectiveActions { get; set; }

        /// <summary>Danh sách hành động khắc phục trong tuần (tối đa 20 mục).</summary>
        public List<WeeklyCorrectiveActionItem> CorrectiveActions { get; set; } = [];

        // ── Recommendations ──────────────────────────────────────────
        /// <summary>Tổng số khuyến nghị đã được tạo ra trong tuần.</summary>
        public int TotalRecommendations { get; set; }

        /// <summary>Danh sách khuyến nghị đã sử dụng trong tuần (tối đa 20 mục).</summary>
        public List<WeeklyRecommendationItem> Recommendations { get; set; } = [];

        // ── Mortality ────────────────────────────────────────────────
        /// <summary>Tổng số lượng chết được ghi nhận trong tuần.</summary>
        public double TotalMortality { get; set; }

        /// <summary>Số sự kiện chết (số dòng MortalityLog) trong tuần.</summary>
        public int MortalityIncidents { get; set; }

        // ── Batch Health ─────────────────────────────────────────────
        /// <summary>Số lô nuôi đang hoạt động.</summary>
        public int ActiveBatches { get; set; }

        /// <summary>
        /// Tỷ lệ sống sót trung bình (%) của các lô đang hoạt động.
        /// Null nếu không có lô nào.
        /// </summary>
        public double? AverageSurvivalRate { get; set; }
    }
}
