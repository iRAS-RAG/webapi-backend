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
        Task<
            Result<IReadOnlyList<ActiveFarmingBatchResponseDto>>
        > GetActiveFarmingBatchByFishTankIdAsync(Guid fishTankId);
        Task<Result<FarmingBatchDto>> GetFarmingBatchByIdAsync(Guid id);
        Task<Result<FarmingBatchDto>> CreateFarmingBatchAsync(CreateFarmingBatchDto createDto);
        Task<Result> UpdateFarmingBatchAsync(Guid id, UpdateFarmingBatchDto updateDto);
        Task<Result> HarvestBatchAsync(Guid id, HarvestBatchRequest request);
        Task<Result> DeleteFarmingBatchAsync(Guid id);
    }
}
