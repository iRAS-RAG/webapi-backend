using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISensorService
    {
        Task<PaginatedResult<SensorDto>> GetAllSensorsAsync(SensorListRequest request);
        Task<Result<SensorDto>> GetSensorByIdAsync(Guid id);
        Task<Result<SensorDto>> CreateSensorAsync(CreateSensorDto createDto);
        Task<Result> UpdateSensorAsync(Guid id, UpdateSensorDto updateDto);
        Task<Result> DeleteSensorAsync(Guid id);
        Task<Result<SensorHistoryDto>> GetSensorHistoryAsync(Guid sensorId, DateOnly date);
        Task<Result<SensorLogDto>> CreateSensorLogAsync(Guid sensorId, CreateSensorLogDto dto);
        Task<Result<PaginatedResult<SensorLogDto>>> GetSensorLogsAsync(
            Guid sensorId,
            SensorLogListRequest request
        );
    }
}
