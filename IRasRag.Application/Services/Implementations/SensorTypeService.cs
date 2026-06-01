using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.SensorTypeSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SensorTypeService : ISensorTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SensorTypeService> _logger;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobService _backgroundJobs;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public SensorTypeService(
            IUnitOfWork unitOfWork,
            ILogger<SensorTypeService> logger,
            IMapper mapper,
            IBackgroundJobService backgroundJobs,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _backgroundJobs = backgroundJobs;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        #region Get Methods
        public async Task<PaginatedResult<SensorTypeDto>> GetAllSensorTypesAsync(
            SensorTypeListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách loại cảm biến (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<SensorType>();
                var spec = new SensorTypeDtoListSpec(request);
                var pagedResult = await repository.GetPagedAsync(
                    spec,
                    request.Page,
                    request.PageSize
                );

                var sensorTypeDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách loại cảm biến thành công: {Count} loại",
                    pagedResult.Items.Count
                );

                return new PaginatedResult<SensorTypeDto>
                {
                    Message =
                        sensorTypeDtos.Count == 0
                            ? "Không có loại cảm biến nào"
                            : "Lấy danh sách loại cảm biến thành công",
                    Data = sensorTypeDtos,
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách loại cảm biến");

                return new PaginatedResult<SensorTypeDto>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách loại cảm biến",
                    Data = Array.Empty<SensorTypeDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result<SensorTypeDto>> GetSensorTypeByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu lấy loại cảm biến với Id: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại cảm biến không hợp lệ");
                    return Result<SensorTypeDto>.Failure(
                        "Id loại cảm biến không hợp lệ",
                        ResultType.BadRequest
                    );
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepository.GetByIdAsync(id);

                if (sensorType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại cảm biến với Id: {Id}", id);
                    return Result<SensorTypeDto>.Failure(
                        "Không tìm thấy loại cảm biến",
                        ResultType.NotFound
                    );
                }

                var sensorTypeDto = _mapper.Map<SensorTypeDto>(sensorType);
                _logger.LogInformation("Lấy loại cảm biến thành công: {Id}", id);

                return Result<SensorTypeDto>.Success(sensorTypeDto, "Lấy loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy loại cảm biến với Id: {Id}", id);
                return Result<SensorTypeDto>.Failure(
                    "Đã xảy ra lỗi khi lấy loại cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Create Method
        public async Task<Result<SensorTypeDto>> CreateSensorTypeAsync(
            CreateSensorTypeDto createDto
        )
        {
            try
            {
                _logger.LogInformation("Bắt đầu tạo loại cảm biến mới: {Name}", createDto.Name);

                // Validate input
                if (string.IsNullOrWhiteSpace(createDto.Name))
                {
                    _logger.LogWarning("Tên loại cảm biến không được để trống");
                    return Result<SensorTypeDto>.Failure(
                        "Tên loại cảm biến không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (string.IsNullOrWhiteSpace(createDto.MeasureType))
                {
                    _logger.LogWarning("Loại đo không được để trống");
                    return Result<SensorTypeDto>.Failure(
                        "Loại đo không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (string.IsNullOrWhiteSpace(createDto.UnitOfMeasure))
                {
                    _logger.LogWarning("Đơn vị đo không được để trống");
                    return Result<SensorTypeDto>.Failure(
                        "Đơn vị đo không được để trống",
                        ResultType.BadRequest
                    );
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();

                // Check duplicate name
                var existingSensorType = await sensorTypeRepository.FirstOrDefaultAsync(st =>
                    st.Name.ToLower() == createDto.Name.Trim().ToLower()
                );

                if (existingSensorType != null)
                {
                    _logger.LogWarning("Loại cảm biến với tên {Name} đã tồn tại", createDto.Name);
                    return Result<SensorTypeDto>.Failure(
                        "Loại cảm biến với tên này đã tồn tại",
                        ResultType.Conflict
                    );
                }

                var sensorType = _mapper.Map<SensorType>(createDto);
                sensorType.Name = createDto.Name.Trim();
                sensorType.MeasureType = createDto.MeasureType.Trim();
                sensorType.UnitOfMeasure = createDto.UnitOfMeasure.Trim();

                await sensorTypeRepository.AddAsync(sensorType);
                await _unitOfWork.SaveChangesAsync();

                await WriteCreateAuditLogAsync(sensorType);

                if (sensorType.Code != null)
                {
                    var code = sensorType.Code;
                    var name = sensorType.Name;
                    var unit = sensorType.UnitOfMeasure;
                    _backgroundJobs.Enqueue<ICatalogSyncJob>(j =>
                        j.SyncUpsertAsync(code, name, unit)
                    );
                }

                var sensorTypeDto = _mapper.Map<SensorTypeDto>(sensorType);
                _logger.LogInformation(
                    "Tạo loại cảm biến thành công: {Id} - {Name}",
                    sensorType.Id,
                    sensorType.Name
                );

                return Result<SensorTypeDto>.Success(sensorTypeDto, "Tạo loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo loại cảm biến: {Name}", createDto.Name);
                return Result<SensorTypeDto>.Failure(
                    "Đã xảy ra lỗi khi tạo loại cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Method
        public async Task<Result> UpdateSensorTypeAsync(Guid id, UpdateSensorTypeDto updateDto)
        {
            try
            {
                _logger.LogInformation("Bắt đầu cập nhật loại cảm biến: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại cảm biến không hợp lệ");
                    return Result.Failure("Id loại cảm biến không hợp lệ", ResultType.BadRequest);
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepository.GetByIdAsync(id);

                if (sensorType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại cảm biến với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy loại cảm biến", ResultType.NotFound);
                }

                var oldSnapshot = new
                {
                    sensorType.Name,
                    sensorType.MeasureType,
                    sensorType.UnitOfMeasure,
                    sensorType.Code,
                };

                // Check duplicate name if name is being updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                {
                    var existingSensorType = await sensorTypeRepository.FirstOrDefaultAsync(st =>
                        st.Name.ToLower() == updateDto.Name.Trim().ToLower() && st.Id != id
                    );

                    if (existingSensorType != null)
                    {
                        _logger.LogWarning(
                            "Loại cảm biến với tên {Name} đã tồn tại",
                            updateDto.Name
                        );
                        return Result.Failure(
                            "Loại cảm biến với tên này đã tồn tại",
                            ResultType.Conflict
                        );
                    }
                }

                _mapper.Map(updateDto, sensorType);

                // Trim string values if they were updated
                if (!string.IsNullOrWhiteSpace(updateDto.Name))
                    sensorType.Name = updateDto.Name.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.MeasureType))
                    sensorType.MeasureType = updateDto.MeasureType.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.UnitOfMeasure))
                    sensorType.UnitOfMeasure = updateDto.UnitOfMeasure.Trim();

                sensorTypeRepository.Update(sensorType);
                await _unitOfWork.SaveChangesAsync();

                await WriteUpdateAuditLogAsync(sensorType, oldSnapshot);

                if (sensorType.Code != null)
                {
                    var code = sensorType.Code;
                    var name = sensorType.Name;
                    var unit = sensorType.UnitOfMeasure;
                    _backgroundJobs.Enqueue<ICatalogSyncJob>(j =>
                        j.SyncUpsertAsync(code, name, unit)
                    );
                }

                _logger.LogInformation("Cập nhật loại cảm biến thành công: {Id}", id);
                return Result.Success("Cập nhật loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại cảm biến: {Id}", id);
                return Result.Failure(
                    "Đã xảy ra lỗi khi cập nhật loại cảm biến",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Delete Method
        public async Task<Result> DeleteSensorTypeAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa loại cảm biến: {Id}", id);

                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Id loại cảm biến không hợp lệ");
                    return Result.Failure("Id loại cảm biến không hợp lệ", ResultType.BadRequest);
                }

                var sensorTypeRepository = _unitOfWork.GetRepository<SensorType>();
                var sensorType = await sensorTypeRepository.GetByIdAsync(id);

                if (sensorType == null)
                {
                    _logger.LogWarning("Không tìm thấy loại cảm biến với Id: {Id}", id);
                    return Result.Failure("Không tìm thấy loại cảm biến", ResultType.NotFound);
                }

                var oldSnapshot = new
                {
                    sensorType.Name,
                    sensorType.MeasureType,
                    sensorType.UnitOfMeasure,
                    sensorType.Code,
                };

                var code = sensorType.Code;
                sensorTypeRepository.Delete(sensorType);
                await _unitOfWork.SaveChangesAsync();

                await WriteDeleteAuditLogAsync(sensorType.Id, oldSnapshot);

                if (code != null)
                    _backgroundJobs.Enqueue<ICatalogSyncJob>(j => j.SyncDeleteAsync(code));

                _logger.LogInformation("Xóa loại cảm biến thành công: {Id}", id);
                return Result.Success("Xóa loại cảm biến thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loại cảm biến: {Id}", id);
                return Result.Failure("Đã xảy ra lỗi khi xóa loại cảm biến", ResultType.Unexpected);
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
                        AuditLogEntityType.SensorType,
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
                    AuditLogEntityType.SensorType,
                    entityId
                );
            }
        }

        private async Task WriteCreateAuditLogAsync(SensorType sensorType)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Create,
                sensorType.Id.ToString(),
                null,
                    new
                    {
                        sensorType.Name,
                        sensorType.MeasureType,
                        sensorType.UnitOfMeasure,
                        sensorType.Code,
                    },
                "create-sensor-type"
            );
        }

        private async Task WriteUpdateAuditLogAsync(SensorType sensorType, object oldSnapshot)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Update,
                sensorType.Id.ToString(),
                oldSnapshot,
                    new
                    {
                        sensorType.Name,
                        sensorType.MeasureType,
                        sensorType.UnitOfMeasure,
                        sensorType.Code,
                    },
                "update-sensor-type"
            );
        }

        private async Task WriteDeleteAuditLogAsync(Guid id, object oldSnapshot)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Delete,
                id.ToString(),
                oldSnapshot,
                null,
                "delete-sensor-type"
            );
        }
        #endregion
    }
}
