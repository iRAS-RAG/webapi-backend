using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Interfaces.Persistence.Repositories
{
    public interface ISensorLogRepository
    {
        Task<SensorHistoryDto> GetLogsByTimeRangeAsync(Guid sensorId, DateTime from, DateTime to, int interval);
        Task<(IReadOnlyList<SensorLogDto> Items, int TotalCount)> GetAggregatedLogsAsync(Guid sensorId, DateTime from, DateTime to, int interval, int page, int pageSize);
    }
}
