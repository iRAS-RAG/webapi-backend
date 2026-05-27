using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Interfaces.Persistence.Repositories
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<PagedResult<AuditLog>> GetPagedAsync(
            AuditLogQueryRequest request,
            Domain.Enums.QueryType type = Domain.Enums.QueryType.ActiveOnly
        );
    }
}
