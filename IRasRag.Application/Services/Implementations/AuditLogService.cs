using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using AutoMapper;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            IAuditLogRepository auditLogRepository,
            IMapper mapper,
            ILogger<AuditLogService> logger
        )
        {
            _auditLogRepository = auditLogRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public Task AddAsync(AuditLog auditLog)
        {
            return _auditLogRepository.AddAsync(auditLog);
        }

        public async Task<PaginatedResult<AuditLogDto>> GetPagedAsync(AuditLogQueryRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Starting to retrieve audit logs (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var result = await _auditLogRepository.GetPagedAsync(request, QueryType.ActiveOnly);
                var auditLogDtos = _mapper.Map<List<AuditLogDto>>(result.Items);

                _logger.LogInformation(
                    "Successfully retrieved audit logs: {Count} records",
                    auditLogDtos.Count
                );

                return new PaginatedResult<AuditLogDto>
                {
                    Message =
                        auditLogDtos.Count == 0
                            ? "Không có nhật ký nào"
                            : "Lấy danh sách nhật ký thành công",
                    Data = auditLogDtos,
                    Meta = PaginationBuilder.BuildPaginationMetadata(
                        request.Page,
                        request.PageSize,
                        result.TotalItems
                    ),
                    Links = PaginationBuilder.BuildPaginationLinks(
                        request.Page,
                        request.PageSize,
                        result.TotalItems
                    ),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving audit logs");

                return new PaginatedResult<AuditLogDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách nhật ký",
                    Data = Array.Empty<AuditLogDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }
    }
}
