using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Interfaces
{
    public interface IAlertService
    {
        Task<PaginatedResult<AlertDto>> GetAllAlertsAsync(int page, int pageSize);
        Task<Result<AlertDto>> GetAlertByIdAsync(Guid id);
        Task<Result<AlertDto>> CreateAlertAsync(CreateAlertDto createDto);
        Task<Result> UpdateAlertAsync(Guid id, UpdateAlertDto updateDto);
        Task<Result> DeleteAlertAsync(Guid id);
    }
}
