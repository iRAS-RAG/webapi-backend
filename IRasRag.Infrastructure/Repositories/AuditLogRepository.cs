using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Repositories
{
    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context)
            : base(context) { }

        public async Task<PagedResult<AuditLog>> GetPagedAsync(
            AuditLogQueryRequest request,
            QueryType type = QueryType.ActiveOnly
        )
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
            var query = ApplyFiltering(GetQueryable(type).AsNoTracking(), request);
            query = ApplySorting(query, request);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<AuditLog> { Items = items, TotalItems = totalItems };
        }

        public async Task<IReadOnlyList<AuditLog>> GetAllAsync(
            AuditLogQueryRequest request,
            QueryType type = QueryType.ActiveOnly
        )
        {
            var query = ApplyFiltering(GetQueryable(type).AsNoTracking(), request);
            query = ApplySorting(query, request);
            return await query.ToListAsync();
        }

        private static IQueryable<AuditLog> ApplyFiltering(
            IQueryable<AuditLog> query,
            AuditLogQueryRequest request
        )
        {
            if (request.UserId.HasValue)
                query = query.Where(x => x.UserId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.Action))
            {
                var action = request.Action.Trim();
                query = query.Where(x => x.Action == action);
            }

            if (!string.IsNullOrWhiteSpace(request.EntityType))
            {
                var entityType = request.EntityType.Trim();
                query = query.Where(x => x.EntityType == entityType);
            }

            if (!string.IsNullOrWhiteSpace(request.EntityId))
            {
                var entityId = request.EntityId.Trim();
                query = query.Where(x => x.EntityId == entityId);
            }

            if (request.FromDate.HasValue)
            {
                var fromDate = DateTime.SpecifyKind(request.FromDate.Value, DateTimeKind.Utc);
                query = query.Where(x => x.Timestamp >= fromDate);
            }

            if (request.ToDate.HasValue)
            {
                var toDate = DateTime.SpecifyKind(request.ToDate.Value, DateTimeKind.Utc);
                query = query.Where(x => x.Timestamp <= toDate);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var q = request.SearchQuery.Trim().ToLower();
                query = query.Where(x =>
                    x.Email.ToLower().Contains(q)
                    || (x.FirstName != null && x.FirstName.ToLower().Contains(q))
                    || (x.LastName != null && x.LastName.ToLower().Contains(q))
                    ||
                    // Tìm full name "LastName FirstName" — VD: "Nguyễn Văn A"
                    (
                        x.LastName != null
                        && x.FirstName != null
                        && (x.LastName + " " + x.FirstName).ToLower().Contains(q)
                    )
                    ||
                    // Tìm full name "FirstName LastName" — VD: "Văn A Nguyễn"
                    (
                        x.LastName != null
                        && x.FirstName != null
                        && (x.FirstName + " " + x.LastName).ToLower().Contains(q)
                    )
                );
            }

            return query;
        }

        private static IQueryable<AuditLog> ApplySorting(
            IQueryable<AuditLog> query,
            AuditLogQueryRequest request
        )
        {
            var sortDir = request.SortDir?.Trim().ToLowerInvariant() == "desc";
            var sortBy = request.SortBy?.Trim().ToLowerInvariant();

            return sortBy switch
            {
                "userid" => sortDir
                    ? query.OrderByDescending(x => x.UserId).ThenByDescending(x => x.Timestamp)
                    : query.OrderBy(x => x.UserId).ThenByDescending(x => x.Timestamp),
                "action" => sortDir
                    ? query.OrderByDescending(x => x.Action).ThenByDescending(x => x.Timestamp)
                    : query.OrderBy(x => x.Action).ThenByDescending(x => x.Timestamp),
                "entitytype" => sortDir
                    ? query.OrderByDescending(x => x.EntityType).ThenByDescending(x => x.Timestamp)
                    : query.OrderBy(x => x.EntityType).ThenByDescending(x => x.Timestamp),
                "entityid" => sortDir
                    ? query.OrderByDescending(x => x.EntityId).ThenByDescending(x => x.Timestamp)
                    : query.OrderBy(x => x.EntityId).ThenByDescending(x => x.Timestamp),
                "timestamp" => sortDir
                    ? query.OrderByDescending(x => x.Timestamp).ThenByDescending(x => x.Id)
                    : query.OrderBy(x => x.Timestamp).ThenByDescending(x => x.Id),
                _ => query.OrderByDescending(x => x.Timestamp).ThenByDescending(x => x.Id),
            };
        }
    }
}
