using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task AddAsync(AuditLog auditLog);

        Task<PagedResult<AuditLogDto>> GetPagedAsync(AuditLogQueryRequest request);
    }
}
