using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFarmService
    {
        Task<PaginatedResult<FarmDto>> GetAllFarmsAsync(int page, int pageSize);
        Task<Result<FarmDto>> GetFarmByIdAsync(Guid id);
        Task<Result<FarmDto>> CreateFarmAsync(CreateFarmDto createDto);
        Task<Result> UpdateFarmAsync(Guid id, UpdateFarmDto updateDto);
        Task<Result> DeleteFarmAsync(Guid id);
    }
}
