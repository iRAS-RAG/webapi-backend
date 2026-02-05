using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IGrowthStageService
    {
        Task<PaginatedResult<GrowthStageDto>> GetAllGrowthStagesAsync(int pageNumber, int pageSize);
        Task<Result<GrowthStageDto>> GetGrowthStageByIdAsync(Guid id);
        Task<Result<GrowthStageDto>> CreateGrowthStageAsync(CreateGrowthStageDto createDto);
        Task<Result> UpdateGrowthStageAsync(Guid id, UpdateGrowthStageDto updateDto);
        Task<Result> DeleteGrowthStageAsync(Guid id);
    }
}
