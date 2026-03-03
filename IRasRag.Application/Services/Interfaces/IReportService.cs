using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Returns KPI dashboard summary (survival rate, alert counts, batch stats)
        /// for the requested time period.
        /// </summary>
        Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(DashboardQueryRequest request);

        /// <summary>
        /// Returns an automatically generated weekly report for supervisors (F10).
        /// Covers alerts, top issues, corrective actions, recommendations used, and batch health
        /// for the specified ISO week.
        /// </summary>
        Task<Result<WeeklyReportDto>> GetWeeklyReportAsync(WeeklyReportQueryRequest request);
    }
}
