using System.Globalization;
using System.Text;
using System.Text.Json;
using AutoMapper;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserAccessor currentUserAccessor,
            IMapper mapper,
            ILogger<AuditLogService> logger
        )
        {
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
            _currentUserAccessor = currentUserAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        public Task AddAsync(AuditLog auditLog)
        {
            return _auditLogRepository.AddAsync(auditLog);
        }

        public async Task WriteSemanticAsync(
            string action,
            string entityType,
            string entityId,
            object? oldValue = null,
            object? newValue = null
        )
        {
            try
            {
                var userId = _currentUserAccessor.GetUserId();
                if (userId is null)
                {
                    _logger.LogDebug(
                        "Skipping semantic audit entry for {Action} because no authenticated user was found.",
                        action
                    );
                    return;
                }

                var user = await _unitOfWork
                    .GetRepository<User>()
                    .FirstOrDefaultAsync(
                        u => u.Id == userId.Value,
                        Domain.Enums.QueryType.IncludeDeleted
                    );

                if (user == null)
                {
                    _logger.LogWarning(
                        "Skipping semantic audit entry for {Action} because the current user {UserId} could not be resolved.",
                        action,
                        userId.Value
                    );
                    return;
                }

                var auditLog = new AuditLog
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValue = SerializeAuditPayload(oldValue),
                    NewValue = SerializeAuditPayload(newValue),
                    Timestamp = DateTime.UtcNow,
                };

                await _auditLogRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to write semantic audit entry for action {Action} on {EntityType} {EntityId}.",
                    action,
                    entityType,
                    entityId
                );
            }
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

        public async Task<byte[]> ExportCsvAsync(AuditLogQueryRequest request)
        {
            try
            {
                var items = await _auditLogRepository.GetAllAsync(request, QueryType.ActiveOnly);

                var builder = new StringBuilder();
                builder.AppendLine(
                    string.Join(
                        ',',
                        new[]
                        {
                            "Id",
                            "UserId",
                            "FirstName",
                            "LastName",
                            "Email",
                            "Action",
                            "EntityType",
                            "EntityId",
                            "OldValue",
                            "NewValue",
                            "Timestamp",
                        }
                    )
                );

                foreach (var item in items)
                {
                    builder.AppendLine(
                        string.Join(
                            ',',
                            new[]
                            {
                                CsvEscape(item.Id.ToString()),
                                CsvEscape(item.UserId.ToString()),
                                CsvEscape(item.FirstName),
                                CsvEscape(item.LastName),
                                CsvEscape(item.Email),
                                CsvEscape(item.Action),
                                CsvEscape(item.EntityType),
                                CsvEscape(item.EntityId),
                                CsvEscape(item.OldValue),
                                CsvEscape(item.NewValue),
                                CsvEscape(
                                    item.Timestamp.ToUniversalTime()
                                        .ToString("O", CultureInfo.InvariantCulture)
                                ),
                            }
                        )
                    );
                }

                return Encoding.UTF8.GetBytes(builder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while exporting audit logs to CSV");
                return Array.Empty<byte>();
            }
        }

        private static string? CsvEscape(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var needsQuotes =
                value.Contains(',')
                || value.Contains('"')
                || value.Contains('\n')
                || value.Contains('\r');
            var escaped = value.Replace("\"", "\"\"");
            return needsQuotes ? $"\"{escaped}\"" : escaped;
        }

        private static string? SerializeAuditPayload(object? value)
        {
            if (value == null)
                return null;

            return value switch
            {
                string text => text,
                DateTime dateTime => dateTime
                    .ToUniversalTime()
                    .ToString("O", CultureInfo.InvariantCulture),
                DateTimeOffset dateTimeOffset => dateTimeOffset
                    .ToUniversalTime()
                    .ToString("O", CultureInfo.InvariantCulture),
                _ => JsonSerializer.Serialize(value),
            };
        }
    }
}
