using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Interfaces
{
    public interface IFarmingBatchService
    {
        Task<PaginatedResult<FarmingBatchDto>> GetAllFarmingBatchesAsync(int page, int pageSize);
        Task<Result<FarmingBatchDto>> GetFarmingBatchByIdAsync(Guid id);
        Task<Result<FarmingBatchDto>> CreateFarmingBatchAsync(CreateFarmingBatchDto createDto);
        Task<Result> UpdateFarmingBatchAsync(Guid id, UpdateFarmingBatchDto updateDto);
        Task<Result> DeleteFarmingBatchAsync(Guid id);
    }
}
