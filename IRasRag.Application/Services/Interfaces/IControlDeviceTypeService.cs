using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IControlDeviceTypeService
    {
        Task<PaginatedResult<ControlDeviceTypeDto>> GetAllControlDeviceTypesAsync(
            int page,
            int pageSize
        );
        Task<Result<ControlDeviceTypeDto>> GetControlDeviceTypeByIdAsync(Guid id);
        Task<Result<ControlDeviceTypeDto>> CreateControlDeviceTypeAsync(
            CreateControlDeviceTypeDto createDto
        );
        Task<Result> UpdateControlDeviceTypeAsync(Guid id, UpdateControlDeviceTypeDto updateDto);
        Task<Result> DeleteControlDeviceTypeAsync(Guid id);
    }
}
