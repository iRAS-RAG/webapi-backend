using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IControlDeviceService
    {
        Task<Result<IEnumerable<ControlDeviceDto>>> GetAllControlDevicesAsync();
        Task<Result<ControlDeviceDto>> GetControlDeviceByIdAsync(Guid id);
        Task<Result<ControlDeviceDto>> CreateControlDeviceAsync(CreateControlDeviceDto createDto);
        Task<Result> UpdateControlDeviceAsync(Guid id, UpdateControlDeviceDto updateDto);
        Task<Result> DeleteControlDeviceAsync(Guid id);
    }
}
