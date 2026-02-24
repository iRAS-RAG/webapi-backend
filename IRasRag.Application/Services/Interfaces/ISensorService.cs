using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISensorService
    {
        Task<PaginatedResult<SensorDto>> GetAllSensorsAsync(SensorListRequest request);
        Task<Result<IReadOnlyList<SensorLogDto>>> GetLatestSensorLogsByTankAsync(Guid fishTankId);
        Task<Result<IReadOnlyList<SensorLogDto>>> GetLatestSensorLogsByFarmAsync(Guid farmId);
        Task<Result<SensorDto>> GetSensorByIdAsync(Guid id);
        Task<Result<SensorDto>> CreateSensorAsync(CreateSensorDto createDto);
        Task<Result> UpdateSensorAsync(Guid id, UpdateSensorDto updateDto);
        Task<Result> DeleteSensorAsync(Guid id);
    }
}
