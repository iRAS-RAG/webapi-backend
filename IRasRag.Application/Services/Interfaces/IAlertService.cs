using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IAlertService
    {
        Task<AlertPaginatedResult> GetAllAlertsAsync(AlertListRequest request);
        Task<Result<AlertDto>> GetAlertByIdAsync(Guid id);
        Task<Result<AlertDto>> CreateAlertAsync(CreateAlertDto createDto);
        Task<Result> UpdateAlertAsync(Guid id, UpdateAlertDto updateDto);
        Task<Result> UpdateAlertStatusAsync(Guid id, AlertStatus newStatus);
        Task<Result> DeleteAlertAsync(Guid id);
    }
}
