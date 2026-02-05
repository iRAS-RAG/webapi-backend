using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ICameraService
    {
        Task<PaginatedResult<CameraDto>> GetAllCamerasAsync(int page, int pageSize);
        Task<Result<CameraDto>> GetCameraByIdAsync(Guid id);
        Task<Result<CameraDto>> CreateCameraAsync(CreateCameraDto createDto);
        Task<Result> UpdateCameraAsync(Guid id, UpdateCameraDto updateDto);
        Task<Result> DeleteCameraAsync(Guid id);
    }
}
