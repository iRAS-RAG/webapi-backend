using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task AddAsync(AuditLog auditLog);

        Task WriteSemanticAsync(
            string action,
            string entityType,
            string entityId,
            object? oldValue = null,
            object? newValue = null
        );

        Task<PaginatedResult<AuditLogDto>> GetPagedAsync(AuditLogQueryRequest request);
        Task<byte[]> ExportExcelAsync(AuditLogQueryRequest request);
    }
}
