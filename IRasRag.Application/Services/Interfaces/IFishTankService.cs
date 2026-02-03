using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFishTankService
    {
        Task<Result<IEnumerable<FishTankDto>>> GetAllFishTanksAsync();
        Task<Result<FishTankDto>> GetFishTankByIdAsync(Guid id);
        Task<Result<FishTankDto>> CreateFishTankAsync(CreateFishTankDto createDto);
        Task<Result> UpdateFishTankAsync(Guid id, UpdateFishTankDto updateDto);
        Task<Result> DeleteFishTankAsync(Guid id);
    }
}
