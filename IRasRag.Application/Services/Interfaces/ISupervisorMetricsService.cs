using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs.Metrics;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISupervisorMetricsService
    {
        Task<Result<FarmSummaryDto>> GetFarmSummaryAsync(
            Guid farmId,
            DateTime? start,
            DateTime? end,
            string groupBy
        );
        Task<Result<TimeSeriesResponseDto>> GetFarmTimeSeriesAsync(
            Guid farmId,
            DateTime? start,
            DateTime? end,
            string metric,
            string interval,
            string groupBy,
            string[] aggregations
        );
        Task<Result<BatchHistoryDto>> GetBatchHistoryAsync(
            Guid batchId,
            DateTime? start,
            DateTime? end,
            string[] metrics,
            string interval
        );
        Task<Result<List<BatchSummaryDto>>> GetTopBatchesAsync(
            Guid farmId,
            DateTime? start,
            DateTime? end,
            string metric,
            int limit
        );
    }
}
