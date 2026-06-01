using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.ControlDeviceTypeSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class ControlDeviceTypeService : IControlDeviceTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ControlDeviceTypeService> _logger;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public ControlDeviceTypeService(
            IUnitOfWork unitOfWork,
            ILogger<ControlDeviceTypeService> logger,
            IMapper mapper,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        #region Get Methods
        public async Task<PaginatedResult<ControlDeviceTypeDto>> GetAllControlDeviceTypesAsync(
            ControlDeviceTypeListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách loại thiết bị điều khiển (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<ControlDeviceType>();
                var spec = new ControlDeviceTypeDtoListSpec(request);
                var pagedResult = await repository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                _logger.LogInformation(
                    "Lấy danh sách loại thiết bị điều khiển thành công: {Count} loại",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<ControlDeviceTypeDto>
                {
                    Message =
                        pagedResult.Items.Count == 0
                            ? "Không có loại thiết bị điều khiển nào"
                            : "Lấy danh sách loại thiết bị điều khiển thành công",
                    Data = pagedResult.Items,
                    Meta = PaginationBuilder.BuildPaginationMetadata(
                        request.Page,
                        request.PageSize,
                        pagedResult.TotalItems
                    ),
                    Links = PaginationBuilder.BuildPaginationLinks(
                        request.Page,
                        request.PageSize,
                        pagedResult.TotalItems
                    ),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách loại thiết bị điều khiển");

                return new PaginatedResult<ControlDeviceTypeDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách loại thiết bị điều khiển",
                    Data = Array.Empty<ControlDeviceTypeDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<ControlDeviceTypeDto>> GetControlDeviceTypeByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy loại thiết bị điều khiển với Id: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại thiết bị điều khiển không hợp lệ");
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Id loại thiết bị điều khiển không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(id);

                if (controlDeviceType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại thiết bị điều khiển với Id: {Id}", id);
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Không tìm thấy loại thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                var controlDeviceTypeDto = _mapper.Map<ControlDeviceTypeDto>(controlDeviceType);
                _logger.LogInformation("Lấy loại thiết bị điều khiển thành công: {Id}", id);

                return Result<ControlDeviceTypeDto>.Success(
                    controlDeviceTypeDto,
                    "Lấy loại thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy loại thiết bị điều khiển với Id: {Id}", id);
                return Result<ControlDeviceTypeDto>.Failure(
                    "Đã xảy ra lỗi khi lấy loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<ControlDeviceTypeDto>> CreateControlDeviceTypeAsync(
            CreateControlDeviceTypeDto createDto
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu tạo loại thiết bị điều khiển mới: {Name}",
                    createDto.Name
                );

                // Validate input
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên loại thiết bị điều khiển không được để trống");
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Tên loại thiết bị điều khiển không được để trống",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();

                // Check duplicate name
                var existingControlDeviceType =
                    await controlDeviceTypeRepository.FirstOrDefaultAsync(cdt =>
                        cdt.Name.ToLower() == createDto.Name.Trim().ToLower()
                    );

                if (existingControlDeviceType != null)
                {
                    _logger.LogWarning(
                        "Loại thiết bị điều khiển với tên {Name} đã tồn tại",
                        createDto.Name
                    );
                    return Result<ControlDeviceTypeDto>.Failure(
                        "Loại thiết bị điều khiển với tên này đã tồn tại",
                        ResultType.Conflict
                    );
                }

                var controlDeviceType = _mapper.Map<ControlDeviceType>(createDto);
                controlDeviceType.Name = createDto.Name.Trim();

                if (!string.IsNullOrWhiteSpace(createDto.Description))
                    controlDeviceType.Description = createDto.Description.Trim();

                await controlDeviceTypeRepository.AddAsync(controlDeviceType);
                await _unitOfWork.SaveChangesAsync();

                await WriteCreateAuditLogAsync(controlDeviceType);

                var controlDeviceTypeDto = _mapper.Map<ControlDeviceTypeDto>(controlDeviceType);
                _logger.LogInformation(
                    "Tạo loại thiết bị điều khiển thành công: {Id} - {Name}",
                    controlDeviceType.Id,
                    controlDeviceType.Name
                );

                return Result<ControlDeviceTypeDto>.Success(
                    controlDeviceTypeDto,
                    "Tạo loại thiết bị điều khiển thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi tạo loại thiết bị điều khiển: {Name}",
                    createDto.Name
                );
                return Result<ControlDeviceTypeDto>.Failure(
                    "Đã xảy ra lỗi khi tạo loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateControlDeviceTypeAsync(
            Guid id,
            UpdateControlDeviceTypeDto updateDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật loại thiết bị điều khiển: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại thiết bị điều khiển không hợp lệ");
                    return Result.Failure(
                        "Id loại thiết bị điều khiển không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(id);

                if (controlDeviceType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại thiết bị điều khiển với Id: {Id}", id);
                    return Result.Failure(
                        "Không tìm thấy loại thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                var oldSnapshot = new
                {
                    controlDeviceType.Name,
                    controlDeviceType.Description,
                };

                // Check duplicate name if name is being updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    var existingControlDeviceType =
                        await controlDeviceTypeRepository.FirstOrDefaultAsync(cdt =>
                            cdt.Name.ToLower() == updateDto.Name.Trim().ToLower() && cdt.Id != id
                        );

                    if (existingControlDeviceType != null)
                    {
                        _logger.LogWarning(
                            "Loại thiết bị điều khiển với tên {Name} đã tồn tại",
                            updateDto.Name
                        );
                        return Result.Failure(
                            "Loại thiết bị điều khiển với tên này đã tồn tại",
                            ResultType.Conflict
                        );
                    }
                }

                _mapper.Map(updateDto, controlDeviceType);

                // Trim string values if they were updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                    controlDeviceType.Name = updateDto.Name.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.Description))
                    controlDeviceType.Description = updateDto.Description.Trim();

                controlDeviceTypeRepository.Update(controlDeviceType);
                await _unitOfWork.SaveChangesAsync();

                await WriteUpdateAuditLogAsync(controlDeviceType, oldSnapshot);

                _logger.LogInformation("Cập nhật loại thiết bị điều khiển thành công: {Id}", id);
                return Result.Success("Cập nhật loại thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại thiết bị điều khiển: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteControlDeviceTypeAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa loại thiết bị điều khiển: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại thiết bị điều khiển không hợp lệ");
                    return Result.Failure(
                        "Id loại thiết bị điều khiển không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var controlDeviceTypeRepository = _unitOfWork.GetRepository<ControlDeviceType>();
                var controlDeviceType = await controlDeviceTypeRepository.GetByIdAsync(id);

                if (controlDeviceType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại thiết bị điều khiển với Id: {Id}", id);
                    return Result.Failure(
                        "Không tìm thấy loại thiết bị điều khiển",
                        ResultType.NotFound
                    );
                }

                var oldSnapshot = new
                {
                    controlDeviceType.Name,
                    controlDeviceType.Description,
                };

                controlDeviceTypeRepository.Delete(controlDeviceType);
                await _unitOfWork.SaveChangesAsync();

                await WriteDeleteAuditLogAsync(controlDeviceType.Id, oldSnapshot);

                _logger.LogInformation("Xóa loại thiết bị điều khiển thành công: {Id}", id);
                return Result.Success("Xóa loại thiết bị điều khiển thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loại thiết bị điều khiển: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi xóa loại thiết bị điều khiển",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Audit Log Helpers
        private async Task<User?> GetAuditActorAsync(string operation)
        {
            var currentUserId = _currentUserAccessor.GetUserId();
            if (currentUserId is null)
            {
                _logger.LogDebug(
                    "Skipping {Operation} audit entry because no authenticated user was found.",
                    operation
                );
                return null;
            }

            var actor = await _unitOfWork
                .GetRepository<User>()
                .FirstOrDefaultAsync(u => u.Id == currentUserId.Value, QueryType.IncludeDeleted);

            if (actor == null)
            {
                _logger.LogWarning(
                    "Skipping {Operation} audit entry because the current user {UserId} could not be resolved.",
                    operation,
                    currentUserId.Value
                );
            }

            return actor;
        }

        private async Task WriteAuditLogAsync(
            string action,
            string entityId,
            object? oldValue,
            object? newValue,
            string operation
        )
        {
            try
            {
                var actor = await GetAuditActorAsync(operation);
                if (actor == null)
                    return;

                await _auditLogService.AddAsync(
                    AuditLogHelper.Create(
                        actor,
                        action,
                        nameof(ControlDeviceType),
                        entityId,
                        oldValue,
                        newValue
                    )
                );

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to write {Operation} audit entry for {EntityType} {EntityId}",
                    operation,
                    nameof(ControlDeviceType),
                    entityId
                );
            }
        }

        private Task WriteCreateAuditLogAsync(ControlDeviceType controlDeviceType)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Create,
                controlDeviceType.Id.ToString(),
                null,
                new
                {
                    Created = "Đã được tạo",
                    controlDeviceType.Name,
                    controlDeviceType.Description,
                },
                "create-control-device-type"
            );
        }

        private Task WriteUpdateAuditLogAsync(ControlDeviceType controlDeviceType, object oldSnapshot)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Update,
                controlDeviceType.Id.ToString(),
                oldSnapshot,
                new
                {
                    Updated = "Đã được cập nhật",
                    controlDeviceType.Name,
                    controlDeviceType.Description,
                },
                "update-control-device-type"
            );
        }

        private Task WriteDeleteAuditLogAsync(Guid id, object oldSnapshot)
        {
            return WriteAuditLogAsync(
                AuditLogActions.Delete,
                id.ToString(),
                oldSnapshot,
                new { Deleted = "Đã được xóa" },
                "delete-control-device-type"
            );
        }
        #endregion
    }
}
