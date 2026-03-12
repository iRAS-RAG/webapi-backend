using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Interfaces.Persistence.Repositories
{
    public interface IFishTankRepository
    {
        Task<IReadOnlyList<FishTankMetricDto>> GetLatestFishTankMetricsByFarmIdAsync(Guid farmId);
    }
}
