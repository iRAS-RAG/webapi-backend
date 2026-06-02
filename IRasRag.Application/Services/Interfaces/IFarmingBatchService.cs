using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFarmingBatchService
    {
        Task<PaginatedResult<FarmingBatchDto>> GetAllFarmingBatchesAsync(
            FarmingBatchListRequest request
        );
        Task<Result<ActiveFarmingBatchResponseDto>> GetActiveFarmingBatchByFishTankIdAsync(
            Guid fishTankId
        );
        Task<Result<FarmingBatchDto>> GetFarmingBatchByIdAsync(Guid id);
        Task<Result<FarmingBatchDto>> CreateFarmingBatchAsync(CreateFarmingBatchDto createDto);
        Task<(int EstimatedCount, double? EstimatedWeightKg)> ComputeEstimatedYieldAsync(
            Domain.Entities.FarmingBatch batch,
            bool persist = false
        );
        Task<double?> ComputeAndPersistFcrAsync(Guid batchId);
        Task<int> RecomputeEstimatedYieldBySpeciesAsync(Guid speciesId);
        Task<Result> UpdateFarmingBatchAsync(Guid id, UpdateFarmingBatchDto updateDto);
        Task<Result> HarvestBatchAsync(
            Guid id,
            DateTime harvestTime,
            bool force = false,
            double? actualHarvestWeightKg = null
        );
        Task<Result<IReadOnlyList<PlannedStageDto>>> GetPlannedStagesByBatchIdAsync(Guid batchId);
        Task<Result<FarmingBatchDto>> StartPausedBatchAsync(Guid id);
        Task<Result<FarmingBatchDto>> TerminateBatchAsync(Guid id);
        Task<Result> DeleteFarmingBatchAsync(Guid id);
    }
}
