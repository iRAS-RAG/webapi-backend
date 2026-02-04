using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISensorTypeService
    {
        Task<Result<IEnumerable<SensorTypeDto>>> GetAllSensorTypesAsync();
        Task<Result<SensorTypeDto>> GetSensorTypeByIdAsync(Guid id);
        Task<Result<SensorTypeDto>> CreateSensorTypeAsync(CreateSensorTypeDto createDto);
        Task<Result> UpdateSensorTypeAsync(Guid id, UpdateSensorTypeDto updateDto);
        Task<Result> DeleteSensorTypeAsync(Guid id);
    }
}
