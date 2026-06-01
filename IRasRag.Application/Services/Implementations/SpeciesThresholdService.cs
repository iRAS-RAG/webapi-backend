using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Advisory;
using IRasRag.Application.Common.Interfaces.BackgroundJobs;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.SpeciesThresholdSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SpeciesThresholdService : ISpeciesThresholdService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeciesThresholdService> _logger;
        private readonly IMapper _mapper;
        private readonly ITelemetryCacheService _telemetryCache;
        private readonly IBackgroundJobService _backgroundJobs;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public SpeciesThresholdService(
            IUnitOfWork unitOfWork,
            ILogger<SpeciesThresholdService> logger,
            IMapper mapper,
            ITelemetryCacheService telemetryCache,
            IBackgroundJobService backgroundJobs,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _telemetryCache = telemetryCache;
            _backgroundJobs = backgroundJobs;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<SpeciesThresholdDto>> CreateSpeciesThreshold(
            CreateSpeciesThresholdDto dto,
            Guid? userId = null
        )
        {
            try
            {
                if (dto.MinValue >= dto.MaxValue)
                    return Result<SpeciesThresholdDto>.Failure(
                        "Giá trị Min phải nhỏ hơn Max.",
                        ResultType.BadRequest
                    );

                if (
                    !await _unitOfWork.GetRepository<Species>().AnyAsync(s => s.Id == dto.SpeciesId)
                )
                    return Result<SpeciesThresholdDto>.Failure(
                        "Loài cá không tồn tại.",
                        ResultType.BadRequest
                    );

                if (
                    !await _unitOfWork
                        .GetRepository<GrowthStage>()
                        .AnyAsync(gs => gs.Id == dto.GrowthStageId)
                )
                    return Result<SpeciesThresholdDto>.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.BadRequest
                    );

                if (
                    !await _unitOfWork
                        .GetRepository<SensorType>()
                        .AnyAsync(st => st.Id == dto.SensorTypeId)
                )
                    return Result<SpeciesThresholdDto>.Failure(
                        "Loại cảm biến không tồn tại.",
                        ResultType.BadRequest
                    );

                var exists = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .AnyAsync(st =>
                        st.SpeciesId == dto.SpeciesId
                        && st.GrowthStageId == dto.GrowthStageId
                        && st.SensorTypeId == dto.SensorTypeId
                    );

                if (exists)
                    return Result<SpeciesThresholdDto>.Failure(
                        "Ngưỡng sinh trưởng này đã tồn tại.",
                        ResultType.Conflict
                    );

                var newThreshold = _mapper.Map<SpeciesThreshold>(dto);

                await _unitOfWork.GetRepository<SpeciesThreshold>().AddAsync(newThreshold);

                await _unitOfWork.SaveChangesAsync();
                _telemetryCache.InvalidateThresholds(dto.SpeciesId, dto.GrowthStageId);

                var thresholdDto = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .FirstOrDefaultAsync(new SpeciesThresholdDtoByIdSpec(newThreshold.Id));

                if (thresholdDto != null)
                    await WriteCreateAuditLogAsync(thresholdDto);

                var userIdStr = userId?.ToString();
                _backgroundJobs.Enqueue<IThresholdSyncJob>(j =>
                    j.SyncCreateAsync(newThreshold.Id, userIdStr)
                );

                return Result<SpeciesThresholdDto>.Success(
                    thresholdDto!,
                    "Tạo ngưỡng sinh trưởng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating species threshold");
                return Result<SpeciesThresholdDto>.Failure(
                    "Lỗi khi tạo ngưỡng sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> DeleteSpeciesThreshold(Guid id)
        {
            try
            {
                var threshold = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .GetByIdAsync(id);
                if (threshold == null)
                    return Result.Failure("Ngưỡng sinh trưởng không tồn tại.", ResultType.NotFound);

                var oldDto = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .FirstOrDefaultAsync(new SpeciesThresholdDtoByIdSpec(id));

                var advisoryId = threshold.AdvisoryThresholdId;
                _unitOfWork.GetRepository<SpeciesThreshold>().Delete(threshold);
                await _unitOfWork.SaveChangesAsync();
                _telemetryCache.InvalidateThresholds(threshold.SpeciesId, threshold.GrowthStageId);

                if (oldDto != null)
                    await WriteDeleteAuditLogAsync(oldDto);

                if (advisoryId != null)
                    _backgroundJobs.Enqueue<IThresholdSyncJob>(j => j.SyncDeleteAsync(advisoryId));

                return Result.Success("Xóa ngưỡng sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting species threshold");
                return Result.Failure("Lỗi khi xóa ngưỡng sinh trưởng.", ResultType.Unexpected);
            }
        }

        public async Task<Result<SpeciesThresholdDto>> GetSpeciesThresholdById(Guid id)
        {
            try
            {
                var threshold = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .FirstOrDefaultAsync(new SpeciesThresholdDtoByIdSpec(id));

                if (threshold == null)
                    return Result<SpeciesThresholdDto>.Failure(
                        "Ngưỡng sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );

                return Result<SpeciesThresholdDto>.Success(
                    threshold,
                    "Lấy ngưỡng sinh trưởng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving species threshold by ID");
                return Result<SpeciesThresholdDto>.Failure(
                    "Lỗi khi truy xuất ngưỡng sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<SpeciesThresholdDto>> GetSpeciesThresholdBySpecies(Guid speciesId)
        {
            try
            {
                var threshold = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .FirstOrDefaultAsync(new SpeciesThresholdDtoBySpeciesIdSpec(speciesId));

                if (threshold == null)
                    return Result<SpeciesThresholdDto>.Failure(
                        "Ngưỡng sinh trưởng không tồn tại.",
                        ResultType.NotFound
                    );

                return Result<SpeciesThresholdDto>.Success(
                    threshold,
                    "Lấy ngưỡng sinh trưởng thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving species threshold by ID");
                return Result<SpeciesThresholdDto>.Failure(
                    "Lỗi khi truy xuất ngưỡng sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<PaginatedResult<SpeciesThresholdDto>> GetAllSpeciesThresholdsAsync(
            SpeciesThresholdListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách ngưỡng sinh trưởng (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<SpeciesThreshold>();
                var pagedResult = await repository.GetPagedAsync(
                    new SpeciesThresholdListSpec(request),
                    request.Page,
                    request.PageSize
                );

                var thresholdDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách ngưỡng sinh trưởng thành công: {Count} ngưỡng",
                    thresholdDtos.Count
                );

                return new PaginatedResult<SpeciesThresholdDto>
                {
                    Message =
                        thresholdDtos.Count == 0
                            ? "Không có ngưỡng sinh trưởng nào"
                            : "Lấy danh sách ngưỡng sinh trưởng thành công",
                    Data = thresholdDtos,
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
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách ngưỡng sinh trưởng");

                return new PaginatedResult<SpeciesThresholdDto>
                {
                    Message = "Lỗi khi truy xuất danh sách ngưỡng sinh trưởng",
                    Data = Array.Empty<SpeciesThresholdDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<Result> UpdateSpeciesThreshold(Guid id, UpdateSpeciesThresholdDto dto)
        {
            try
            {
                if (dto.MinValue >= dto.MaxValue)
                    return Result.Failure("Giá trị Min phải nhỏ hơn Max.", ResultType.BadRequest);

                var threshold = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .GetByIdAsync(id);
                if (threshold == null)
                    return Result.Failure("Ngưỡng sinh trưởng không tồn tại.", ResultType.NotFound);

                var oldDto = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .FirstOrDefaultAsync(new SpeciesThresholdDtoByIdSpec(id));

                _mapper.Map(dto, threshold);
                await _unitOfWork.SaveChangesAsync();
                _telemetryCache.InvalidateThresholds(threshold.SpeciesId, threshold.GrowthStageId);

                var updatedDto = await _unitOfWork
                    .GetRepository<SpeciesThreshold>()
                    .FirstOrDefaultAsync(new SpeciesThresholdDtoByIdSpec(id));

                if (oldDto != null && updatedDto != null)
                    await WriteUpdateAuditLogAsync(oldDto, updatedDto);

                if (threshold.AdvisoryThresholdId != null)
                {
                    var advisoryId = threshold.AdvisoryThresholdId;
                    _backgroundJobs.Enqueue<IThresholdSyncJob>(j =>
                        j.SyncUpdateAsync(advisoryId, threshold.MinValue, threshold.MaxValue)
                    );
                }

                return Result.Success("Cập nhật ngưỡng sinh trưởng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating species threshold");
                return Result.Failure(
                    "Lỗi khi cập nhật ngưỡng sinh trưởng.",
                    ResultType.Unexpected
                );
            }
        }

        #region Audit Log Helpers
        private static object ToAuditSnapshot(SpeciesThresholdDto dto)
        {
            return new
            {
                dto.SpeciesName,
                dto.GrowthStageName,
                dto.SensorTypeName,
                dto.MinValue,
                dto.MaxValue,
                dto.UnitOfMeasure,
            };
        }

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
                        AuditLogEntityType.SpeciesThreshold,
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
                    AuditLogEntityType.SpeciesThreshold,
                    entityId
                );
            }
        }

        private async Task WriteCreateAuditLogAsync(SpeciesThresholdDto dto)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Create,
                dto.Id.ToString(),
                null,
                ToAuditSnapshot(dto),
                "create-species-threshold"
            );
        }

        private async Task WriteUpdateAuditLogAsync(SpeciesThresholdDto oldDto, SpeciesThresholdDto newDto)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Update,
                newDto.Id.ToString(),
                ToAuditSnapshot(oldDto),
                ToAuditSnapshot(newDto),
                "update-species-threshold"
            );
        }

        private async Task WriteDeleteAuditLogAsync(SpeciesThresholdDto dto)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Delete,
                dto.Id.ToString(),
                ToAuditSnapshot(dto),
                null,
                "delete-species-threshold"
            );
        }
        #endregion
    }
}
