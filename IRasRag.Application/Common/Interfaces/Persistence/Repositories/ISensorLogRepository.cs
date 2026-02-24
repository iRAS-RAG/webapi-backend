using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Interfaces.Persistence.Repositories
{
    public interface ISensorLogRepository
    {
        Task<IReadOnlyList<SensorLogDto>>GetLatestLogPerSensorByTankId(
            Guid fishTankId);
        Task<IReadOnlyList<SensorLogDto>> GetLatestLogPerSensorByFarm(Guid farmId);
    }
}
