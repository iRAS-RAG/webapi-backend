using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IControlDeviceTypeService
    {
        Task<Result<IEnumerable<ControlDeviceTypeDto>>> GetAllControlDeviceTypesAsync();
        Task<Result<ControlDeviceTypeDto>> GetControlDeviceTypeByIdAsync(Guid id);
        Task<Result<ControlDeviceTypeDto>> CreateControlDeviceTypeAsync(
            CreateControlDeviceTypeDto createDto
        );
        Task<Result> UpdateControlDeviceTypeAsync(Guid id, UpdateControlDeviceTypeDto updateDto);
        Task<Result> DeleteControlDeviceTypeAsync(Guid id);
    }
}
