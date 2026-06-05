using System.Globalization;
using System.Text;
using System.Text.Json;
using AutoMapper;
using ClosedXML.Excel;
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

                var auditLog = AuditLogHelper.Create(
                    user,
                    action,
                    entityType,
                    entityId,
                    oldValue,
                    newValue
                );

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

                // Load role hiện tại cho từng user (tối đa pageSize user/page)
                var userIds = result
                    .Items.Select(x => x.UserId)
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();

                var roleByUserId = new Dictionary<Guid, string>();
                foreach (var userId in userIds)
                {
                    try
                    {
                        var user =
                            await _unitOfWork.GetRepository<User>().GetByIdAsync(userId)
                            ?? await _unitOfWork
                                .GetRepository<User>()
                                .FirstOrDefaultAsync(
                                    u => u.Id == userId,
                                    Domain.Enums.QueryType.IncludeDeleted
                                );

                        if (user == null)
                            continue;

                        var role = await _unitOfWork
                            .GetRepository<Role>()
                            .GetByIdAsync(user.RoleId);
                        if (role == null)
                            continue;

                        try
                        {
                            roleByUserId[userId] = role.ToSystemRole().ToRoleName();
                        }
                        catch
                        {
                            roleByUserId[userId] = role.Name;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to load role for userId={UserId}.", userId);
                    }
                }

                foreach (var dto in auditLogDtos)
                {
                    if (Guid.TryParse(dto.UserId, out var uid))
                        dto.Role = roleByUserId.GetValueOrDefault(uid, string.Empty);
                }

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

        public async Task<byte[]> ExportExcelAsync(AuditLogQueryRequest request)
        {
            var items = await _auditLogRepository.GetAllAsync(request, QueryType.ActiveOnly);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Audit Logs");

            var headers = new[]
            {
                "Thời gian UTC",
                "Người dùng",
                "Email",
                "Hành động",
                "Đối tượng",
                "Entity ID",
                "Giá trị cũ",
                "Giá trị mới",
            };

            for (var i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var rowIndex = 2;
            foreach (var item in items)
            {
                var actorName = string.Join(
                        " ",
                        new[] { item.FirstName, item.LastName }.Where(v =>
                            !string.IsNullOrWhiteSpace(v)
                        )
                    )
                    .Trim();
                var userDisplay = string.IsNullOrWhiteSpace(actorName) ? item.Email : actorName;

                worksheet.Cell(rowIndex, 1).Value = item.Timestamp.ToUniversalTime();
                worksheet.Cell(rowIndex, 1).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
                worksheet.Cell(rowIndex, 2).Value = userDisplay;
                worksheet.Cell(rowIndex, 3).Value = item.Email;
                worksheet.Cell(rowIndex, 4).Value = item.Action;
                worksheet.Cell(rowIndex, 5).Value = item.EntityType;
                worksheet.Cell(rowIndex, 6).Value = item.EntityId;
                worksheet.Cell(rowIndex, 7).Value = FormatAuditValue(item.OldValue);
                worksheet.Cell(rowIndex, 8).Value = FormatAuditValue(item.NewValue);
                worksheet.Row(rowIndex).Style.Alignment.WrapText = true;
                worksheet.Row(rowIndex).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                rowIndex++;
            }

            var headerRange = worksheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.Black;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var usedRange = worksheet.Range(1, 1, Math.Max(1, rowIndex - 1), headers.Length);
            usedRange.SetAutoFilter();

            worksheet.SheetView.FreezeRows(1);
            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static string FormatAuditValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var text = CollapseWhitespace(value.Trim());
            if (!LooksLikeJson(text))
                return Truncate(text, 220);

            try
            {
                using var doc = JsonDocument.Parse(text);
                return Truncate(FormatJsonElement(doc.RootElement), 320);
            }
            catch
            {
                return Truncate(text, 220);
            }
        }

        private static string FormatJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => string.Join(
                    "; ",
                    element
                        .EnumerateObject()
                        .Select(prop => $"{prop.Name}={FormatJsonElementValue(prop.Value)}")
                ),
                JsonValueKind.Array => string.Join(
                    ", ",
                    element.EnumerateArray().Select(FormatJsonElementValue)
                ),
                _ => FormatJsonElementValue(element),
            };
        }

        private static string FormatJsonElementValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null or JsonValueKind.Undefined => string.Empty,
                JsonValueKind.Object => "{...}",
                JsonValueKind.Array => "[...]",
                _ => element.ToString(),
            };
        }

        private static bool LooksLikeJson(string value)
        {
            return value.StartsWith('{') || value.StartsWith('[');
        }

        private static string CollapseWhitespace(string value)
        {
            return string.Join(
                " ",
                value.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            );
        }

        private static string Truncate(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            return value[..Math.Max(0, maxLength - 1)] + "…";
        }
    }
}
