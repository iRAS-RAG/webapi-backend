using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISensorService
    {
        Task<Result<IEnumerable<SensorDto>>> GetAllSensorsAsync();
        Task<Result<SensorDto>> GetSensorByIdAsync(Guid id);
        Task<Result<SensorDto>> CreateSensorAsync(CreateSensorDto createDto);
        Task<Result> UpdateSensorAsync(Guid id, UpdateSensorDto updateDto);
        Task<Result> DeleteSensorAsync(Guid id);
    }
}
