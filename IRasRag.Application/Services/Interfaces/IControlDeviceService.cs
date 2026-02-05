using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IControlDeviceService
    {
        Task<PaginatedResult<ControlDeviceDto>> GetAllControlDevicesAsync(int page, int pageSize);
        Task<Result<ControlDeviceDto>> GetControlDeviceByIdAsync(Guid id);
        Task<Result<ControlDeviceDto>> CreateControlDeviceAsync(CreateControlDeviceDto createDto);
        Task<Result> UpdateControlDeviceAsync(Guid id, UpdateControlDeviceDto updateDto);
        Task<Result> DeleteControlDeviceAsync(Guid id);
    }
}
