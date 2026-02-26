using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Interfaces.Persistence.Repositories
{
    public interface ISensorLogRepository
    {
        Task<SensorHistoryDto> GetLogsByTimeRangeAsync(Guid sensorId, DateOnly date);
    }
}
