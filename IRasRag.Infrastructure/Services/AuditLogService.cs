using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public Task AddAsync(AuditLog auditLog)
        {
            return _auditLogRepository.AddAsync(auditLog);
        }

        public async Task<PagedResult<AuditLogDto>> GetPagedAsync(AuditLogQueryRequest request)
        {
            var result = await _auditLogRepository.GetPagedAsync(request, QueryType.ActiveOnly);
            return new PagedResult<AuditLogDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalItems = result.TotalItems,
            };
        }

        private static AuditLogDto MapToDto(AuditLog auditLog)
        {
            return new AuditLogDto
            {
                Id = auditLog.Id,
                UserId = auditLog.UserId,
                FirstName = auditLog.FirstName,
                LastName = auditLog.LastName,
                Email = auditLog.Email,
                Action = auditLog.Action,
                EntityType = auditLog.EntityType,
                EntityId = auditLog.EntityId,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue,
                Timestamp = auditLog.Timestamp,
            };
        }
    }
}
