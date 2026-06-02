using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.ReportSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportService> _logger;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public ReportService(
            IUnitOfWork unitOfWork,
            ILogger<ReportService> logger,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        #region Dashboard Summary
        public async Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(
            DashboardQueryRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Fetching dashboard summary for period: {Period}",
                    request.Period
                );

                var (periodFrom, periodTo, periodLabel) = ResolvePeriod(request.Period);

                // ── Resolve fish tanks belonging to this user ────────────────────
                var tankIds = await UserScopeHelper.GetUserTankIdsAsync(
                    _unitOfWork,
                    request.UserId
                );

                if (tankIds.Count == 0)
                {
                    return Result<DashboardSummaryDto>.Success(
                        new DashboardSummaryDto
                        {
                            PeriodFrom = periodFrom,
                            PeriodTo = periodTo,
                            PeriodLabel = periodLabel,
                        },
                        "Lấy dữ liệu tổng quan thành công."
                    );
                }

                // ── Alert KPIs — use CountAsync per status (no entity loading) ──────
                var alertRepo = _unitOfWork.GetRepository<Alert>();

                var statusSpec = new AlertStatusCountSpec(periodFrom, periodTo, tankIds);
                var alertStatuses = (await alertRepo.ListAsync(statusSpec)).ToList();

                int totalAlerts = alertStatuses.Count;
                int openAlerts = alertStatuses.Count(s => s == AlertStatus.OPEN);
                int acknowledgedAlerts = alertStatuses.Count(s => s == AlertStatus.ACKNOWLEDGED);
                int resolvedAlerts = alertStatuses.Count(s => s == AlertStatus.RESOLVED);
                int dismissedAlerts = alertStatuses.Count(s => s == AlertStatus.DISMISSED);

                // ── Farming batch KPIs — CountAsync for pure counts ───────────────
                var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();

                int activeBatchCount = await batchRepo.CountAsync(b =>
                    tankIds.Contains(b.FishTankId) && b.Status == FarmingBatchStatus.ACTIVE
                );
                int harvestedBatches = await batchRepo.CountAsync(b =>
                    tankIds.Contains(b.FishTankId)
                    && b.Status == FarmingBatchStatus.HARVESTED
                    && b.ActualHarvestDate.HasValue
                    && b.ActualHarvestDate.Value >= periodFrom
                    && b.ActualHarvestDate.Value < periodTo
                );
                int batchesInPeriod = await batchRepo.CountAsync(b =>
                    tankIds.Contains(b.FishTankId)
                    && b.StartDate >= periodFrom
                    && b.StartDate < periodTo
                );

                // ── Survival rate — projection spec fetches only quantity columns ─
                var survivalSpec = new ActiveBatchSurvivalSpec(tankIds);
                var survivalData = (await batchRepo.ListAsync(survivalSpec)).ToList();

                double totalInitialQuantity = survivalData.Sum(b => b.InitialQuantity);
                double totalCurrentQuantity = survivalData.Sum(b => b.CurrentQuantity);
                double? averageSurvivalRate =
                    totalInitialQuantity > 0
                        ? Math.Round(totalCurrentQuantity / totalInitialQuantity * 100.0, 2)
                        : null;

                // ── Mortality within period ───────────────────────────────────────
                var dashboardBatchIds = (
                    await batchRepo.FindAllAsync(b => tankIds.Contains(b.FishTankId))
                )
                    .Select(b => b.Id)
                    .ToHashSet();

                var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLogs = await mortalityRepo.FindAllAsync(m =>
                    dashboardBatchIds.Contains(m.BatchId)
                    && m.Date >= periodFrom
                    && m.Date < periodTo
                );
                double totalMortality = mortalityLogs.Sum(m => (double)m.Quantity);

                var summary = new DashboardSummaryDto
                {
                    PeriodFrom = periodFrom,
                    PeriodTo = periodTo,
                    PeriodLabel = periodLabel,

                    // Alert KPIs
                    TotalAlerts = totalAlerts,
                    OpenAlerts = openAlerts,
                    AcknowledgedAlerts = acknowledgedAlerts,
                    ResolvedAlerts = resolvedAlerts,
                    DismissedAlerts = dismissedAlerts,

                    // Farming batch KPIs
                    ActiveBatches = activeBatchCount,
                    HarvestedBatches = harvestedBatches,
                    TotalBatches = batchesInPeriod,

                    // Survival KPIs
                    AverageSurvivalRate = averageSurvivalRate,
                    TotalInitialQuantity = totalInitialQuantity,
                    TotalCurrentQuantity = totalCurrentQuantity,
                    TotalMortality = totalMortality,
                };

                _logger.LogInformation(
                    "Dashboard summary fetched successfully: TotalAlerts={TotalAlerts}, ActiveBatches={ActiveBatches}, SurvivalRate={SurvivalRate}",
                    totalAlerts,
                    activeBatchCount,
                    averageSurvivalRate
                );

                await WriteReportAuditLogAsync(
                    action: AuditLogActions.ViewDashboardReport,
                    reportType: nameof(DashboardSummaryDto),
                    entityId: request.UserId.ToString(),
                    oldValue: null,
                    newValue: new
                    {
                        ReportType = "Báo cáo tổng quan",
                        request.Period,
                        PeriodLabel = periodLabel,
                        TotalAlerts = totalAlerts,
                        OpenAlerts = openAlerts,
                        AcknowledgedAlerts = acknowledgedAlerts,
                        ResolvedAlerts = resolvedAlerts,
                        DismissedAlerts = dismissedAlerts,
                        ActiveBatches = activeBatchCount,
                        HarvestedBatches = harvestedBatches,
                        TotalBatches = batchesInPeriod,
                        AverageSurvivalRate = averageSurvivalRate,
                        TotalInitialQuantity = totalInitialQuantity,
                        TotalCurrentQuantity = totalCurrentQuantity,
                        TotalMortality = totalMortality,
                    },
                    operation: "view-dashboard-report"
                );

                return Result<DashboardSummaryDto>.Success(
                    summary,
                    "Lấy dữ liệu tổng quan thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching dashboard summary for period: {Period}",
                    request.Period
                );
                return Result<DashboardSummaryDto>.Failure(
                    "Có lỗi xảy ra khi lấy dữ liệu tổng quan.",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Weekly Report
        public async Task<Result<WeeklyReportDto>> GetWeeklyReportAsync(
            WeeklyReportQueryRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Generating weekly report for period: {Period}",
                    request.Period
                );

                var (weekFrom, weekTo, weekLabel) = ResolveWeekPeriod(request.Period);

                // ── Resolve fish tanks belonging to this user ────────────────────────
                var tankIds = await UserScopeHelper.GetUserTankIdsAsync(
                    _unitOfWork,
                    request.UserId,
                    request.FarmId,
                    request.BatchId
                );

                // Nếu không có tank nào, trả về report với các số liệu tự động bằng 0.
                if (tankIds.Count == 0)
                {
                    return Result<WeeklyReportDto>.Success(
                        new WeeklyReportDto
                        {
                            GeneratedAt = DateTime.UtcNow,
                            PeriodFrom = weekFrom,
                            PeriodTo = weekTo,
                            PeriodLabel = weekLabel,
                        },
                        "Lấy báo cáo tuần thành công."
                    );
                }

                // ── Alert KPIs ────────────────────────────────────────────────
                var alertRepo = _unitOfWork.GetRepository<Alert>();

                var statusSpec = new AlertStatusCountSpec(weekFrom, weekTo, tankIds);
                var alertStatuses = (await alertRepo.ListAsync(statusSpec)).ToList();

                int totalAlerts = alertStatuses.Count;
                int openAlerts = alertStatuses.Count(s => s == AlertStatus.OPEN);
                int acknowledgedAlerts = alertStatuses.Count(s => s == AlertStatus.ACKNOWLEDGED);
                int resolvedAlerts = alertStatuses.Count(s => s == AlertStatus.RESOLVED);
                int dismissedAlerts = alertStatuses.Count(s => s == AlertStatus.DISMISSED);

                // ── Alert breakdown by sensor type ────────────────────────────
                var sensorTypeSpec = new WeeklyAlertSensorTypeSpec(weekFrom, weekTo, tankIds);
                var sensorTypeNames = (await alertRepo.ListAsync(sensorTypeSpec)).ToList();

                var topIssues = sensorTypeNames
                    .GroupBy(name => name)
                    .Select(g => new AlertTypeBreakdownItem
                    {
                        SensorTypeName = g.Key,
                        Count = g.Count(),
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // ── Corrective actions ────────────────────────────────────────
                var correctiveActionRepo = _unitOfWork.GetRepository<CorrectiveAction>();
                var caSpec = new WeeklyCorrectiveActionSpec(weekFrom, weekTo, tankIds);
                var correctiveActions = (await correctiveActionRepo.ListAsync(caSpec)).ToList();

                // ── Recommendations ───────────────────────────────────────────
                var recommendationRepo = _unitOfWork.GetRepository<Recommendation>();
                var recSpec = new WeeklyRecommendationSpec(weekFrom, weekTo, tankIds);
                var recommendations = (await recommendationRepo.ListAsync(recSpec)).ToList();

                // ── Batch health & Mortality ──────────────────────────────────
                var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var weeklyBatchIds = (
                    await batchRepo.FindAllAsync(b => tankIds.Contains(b.FishTankId))
                )
                    .Select(b => b.Id)
                    .ToHashSet();

                var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLogs = await mortalityRepo.FindAllAsync(m =>
                    weeklyBatchIds.Contains(m.BatchId) && m.Date >= weekFrom && m.Date < weekTo
                );
                var mortalityList = mortalityLogs.ToList();
                double totalMortality = mortalityList.Sum(m => (double)m.Quantity);

                int activeBatches = await batchRepo.CountAsync(b =>
                    tankIds.Contains(b.FishTankId) && b.Status == FarmingBatchStatus.ACTIVE
                );

                var survivalSpec = new ActiveBatchSurvivalSpec(tankIds);
                var survivalData = (await batchRepo.ListAsync(survivalSpec)).ToList();
                double totalInitialQty = survivalData.Sum(b => b.InitialQuantity);
                double totalCurrentQty = survivalData.Sum(b => b.CurrentQuantity);
                double? avgSurvivalRate =
                    totalInitialQty > 0
                        ? Math.Round(totalCurrentQty / totalInitialQty * 100.0, 2)
                        : null;

                var report = new WeeklyReportDto
                {
                    GeneratedAt = DateTime.UtcNow,
                    PeriodFrom = weekFrom,
                    PeriodTo = weekTo,
                    PeriodLabel = weekLabel,

                    TotalAlerts = totalAlerts,
                    OpenAlerts = openAlerts,
                    AcknowledgedAlerts = acknowledgedAlerts,
                    ResolvedAlerts = resolvedAlerts,
                    DismissedAlerts = dismissedAlerts,
                    TopIssuesBySensorType = topIssues,

                    TotalCorrectiveActions = correctiveActions.Count,
                    CorrectiveActions = correctiveActions.Take(20).ToList(),

                    TotalRecommendations = recommendations.Count,
                    Recommendations = recommendations.Take(20).ToList(),

                    TotalMortality = totalMortality,
                    MortalityIncidents = mortalityList.Count,

                    ActiveBatches = activeBatches,
                    AverageSurvivalRate = avgSurvivalRate,
                };

                _logger.LogInformation(
                    "Weekly report generated successfully: Week={Label}, TotalAlerts={TotalAlerts}, CorrectiveActions={CA}, Recommendations={Rec}",
                    weekLabel,
                    totalAlerts,
                    correctiveActions.Count,
                    recommendations.Count
                );

                await WriteReportAuditLogAsync(
                    action: AuditLogActions.ViewWeeklyReport,
                    reportType: nameof(WeeklyReportDto),
                    entityId: request.UserId.ToString(),
                    oldValue: null,
                    newValue: new
                    {
                        ReportType = "Báo cáo tuần",
                        request.Period,
                        PeriodLabel = weekLabel,
                        TotalAlerts = totalAlerts,
                        OpenAlerts = openAlerts,
                        AcknowledgedAlerts = acknowledgedAlerts,
                        ResolvedAlerts = resolvedAlerts,
                        DismissedAlerts = dismissedAlerts,
                        TopIssuesBySensorType = topIssues,
                        TotalCorrectiveActions = correctiveActions.Count,
                        TotalRecommendations = recommendations.Count,
                        TotalMortality = totalMortality,
                        MortalityIncidents = mortalityList.Count,
                        ActiveBatches = activeBatches,
                        AverageSurvivalRate = avgSurvivalRate,
                    },
                    operation: "view-weekly-report"
                );

                return Result<WeeklyReportDto>.Success(report, "Lấy báo cáo tuần thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while generating weekly report for period: {Period}",
                    request.Period
                );
                return Result<WeeklyReportDto>.Failure(
                    "Có lỗi xảy ra khi tạo báo cáo tuần.",
                    ResultType.Unexpected
                );
            }
        }

        // ── Period resolution helpers ─────────────────────────────────────────

        private static (DateTime From, DateTime To, string Label) ResolvePeriod(ReportPeriod period)
        {
            var now = DateTime.UtcNow;

            return period switch
            {
                ReportPeriod.TODAY => (
                    new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(
                        1
                    ),
                    "Hôm nay"
                ),
                ReportPeriod.WEEK => (
                    DateTime.SpecifyKind(now.AddDays(-6).Date, DateTimeKind.Utc),
                    new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(
                        1
                    ),
                    "7 ngày qua"
                ),
                ReportPeriod.YEAR => (
                    new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(now.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    $"Năm {now.Year}"
                ),
                // ReportPeriod.MONTH is the default
                _ => (
                    new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1),
                    $"Tháng {now.Month}/{now.Year}"
                ),
            };
        }

        /// <summary>
        /// Resolves the ISO week boundaries (Monday 00:00 → Sunday 23:59:59 UTC) for the weekly
        /// report. Accepted period values:
        ///   "current" – current ISO week
        ///   "last"    – previous ISO week
        ///   any positive integer string N – N weeks ago
        /// Defaults to the current week for any unrecognised input.
        /// </summary>
        private static (DateTime From, DateTime To, string Label) ResolveWeekPeriod(string period)
        {
            var now = DateTime.UtcNow;
            int weeksBack = 0;

            var normalised = period?.Trim().ToLowerInvariant() ?? "current";
            if (normalised == "last")
            {
                weeksBack = 1;
            }
            else if (normalised != "current" && int.TryParse(normalised, out int n) && n > 0)
            {
                weeksBack = n;
            }

            // Find Monday of the target week (ISO 8601: week starts on Monday)
            var today = now.Date;
            int dayOfWeek = (int)today.DayOfWeek; // Sunday=0 … Saturday=6
            // Map to ISO: Monday=1 … Sunday=7
            int isoDayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
            var monday = today.AddDays(-(isoDayOfWeek - 1) - weeksBack * 7);
            var sunday = monday.AddDays(6);

            // Compute ISO week number for the label
            var cal = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            int weekNumber = cal.GetWeekOfYear(
                monday,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday
            );

            var from = new DateTime(
                monday.Year,
                monday.Month,
                monday.Day,
                0,
                0,
                0,
                DateTimeKind.Utc
            );
            var to = new DateTime(
                sunday.Year,
                sunday.Month,
                sunday.Day,
                0,
                0,
                0,
                DateTimeKind.Utc
            ).AddDays(1);
            var label = $"Tuần {weekNumber}/{monday.Year} ({monday:dd/MM} – {sunday:dd/MM})";

            return (from, to, label);
        }
        #endregion
        #region Audit Logging
        private async Task WriteReportAuditLogAsync(
            string action,
            string reportType,
            string entityId,
            object? oldValue,
            object? newValue,
            string operation
        )
        {
            try
            {
                await _auditLogService.WriteSemanticAsync(
                    action,
                    AuditLogEntityType.Report,
                    entityId,
                    oldValue,
                    newValue
                );

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to write {Operation} audit entry for {EntityType} {EntityId}",
                    operation,
                    reportType,
                    entityId
                );
            }
        }
        #endregion
    }
}
