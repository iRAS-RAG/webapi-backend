using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISensorTypeService
    {
        Task<PaginatedResult<SensorTypeDto>> GetAllSensorTypesAsync(int page, int pageSize);
        Task<Result<SensorTypeDto>> GetSensorTypeByIdAsync(Guid id);
        Task<Result<SensorTypeDto>> CreateSensorTypeAsync(CreateSensorTypeDto createDto);
        Task<Result> UpdateSensorTypeAsync(Guid id, UpdateSensorTypeDto updateDto);
        Task<Result> DeleteSensorTypeAsync(Guid id);
    }
}
