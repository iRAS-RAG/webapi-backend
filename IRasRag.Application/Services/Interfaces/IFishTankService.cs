using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFishTankService
    {
        Task<PaginatedResult<FishTankDto>> GetAllFishTanksAsync(FishTankListRequest request);
        Task<Result<FishTankDto>> GetFishTankByIdAsync(Guid id);
        Task<Result<FishTankDto>> CreateFishTankAsync(CreateFishTankDto createDto);
        Task<Result<FishTankDto>> UpdateFishTankAsync(Guid id, UpdateFishTankDto updateDto);
        Task<Result> DeleteFishTankAsync(Guid id);
        Task<Result<List<TankSensorLatestDataDto>>> GetTankLatestDataAsync(Guid tankId);
        Task<Result<TankStatusDto>> GetTankStatusAsync(Guid tankId);
    }
}
