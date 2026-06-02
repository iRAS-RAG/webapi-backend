using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.SpeciesStageConfigSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SpeciesStageConfigService : ISpeciesStageConfigService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeciesStageConfigService> _logger;
        private readonly IMapper _mapper;
        private readonly ITelemetryCacheService _telemetryCache;
        private readonly IFarmingBatchService _farmingBatchService;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public SpeciesStageConfigService(
            IUnitOfWork unitOfWork,
            ILogger<SpeciesStageConfigService> logger,
            IMapper mapper,
            ITelemetryCacheService telemetryCache,
            IFarmingBatchService farmingBatchService,
            IAuditLogService auditLogService,
            ICurrentUserAccessor currentUserAccessor
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _telemetryCache = telemetryCache;
            _farmingBatchService = farmingBatchService;
            _auditLogService = auditLogService;
            _currentUserAccessor = currentUserAccessor;
        }

        public async Task<Result<SpeciesStageConfigDto>> CreateSpeciesStageConfig(
            CreateSpeciesStageConfigDto dto
        )
        {
            try
            {
                if (
                    await _unitOfWork.GetRepository<Species>().AnyAsync(s => s.Id == dto.SpeciesId)
                    == false
                )
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Loài cá không tồn tại.",
                        ResultType.BadRequest
                    );

                if (
                    await _unitOfWork
                        .GetRepository<GrowthStage>()
                        .AnyAsync(gs => gs.Id == dto.GrowthStageId) == false
                )
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Giai đoạn sinh trưởng không tồn tại.",
                        ResultType.BadRequest
                    );

                // ensure the growth stage belongs to the same species
                var growthStage = await _unitOfWork
                    .GetRepository<GrowthStage>()
                    .GetByIdAsync(dto.GrowthStageId);

                if (growthStage == null || growthStage.SpeciesId != dto.SpeciesId)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Giai đoạn sinh trưởng không thuộc loài cá được chỉ định.",
                        ResultType.BadRequest
                    );

                if (dto.FeedTypeIds == null || dto.FeedTypeIds.Count == 0)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Phải chọn ít nhất một kiểu cho ăn.",
                        ResultType.BadRequest
                    );

                var requestedFeedTypeIds = dto.FeedTypeIds.Distinct().ToList();
                var feedTypes = await _unitOfWork
                    .GetRepository<FeedType>()
                    .FindAllAsync(ft => requestedFeedTypeIds.Contains(ft.Id));

                if (feedTypes.Count != requestedFeedTypeIds.Count)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Kiểu cho ăn không tồn tại.",
                        ResultType.BadRequest
                    );

                var exists = await _unitOfWork
                    .GetRepository<SpeciesStageConfig>()
                    .AnyAsync(ssc =>
                        ssc.SpeciesId == dto.SpeciesId && ssc.GrowthStageId == dto.GrowthStageId
                    );

                if (exists)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá ở giai đoạn này đã tồn tại.",
                        ResultType.Conflict
                    );

                var newConfig = _mapper.Map<SpeciesStageConfig>(dto);
                newConfig.FeedTypes = feedTypes.ToList();

                // Auto-adjust sequences: shift existing sequences >= requested sequence up by 1
                var repo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                var existingForSpecies = await repo.FindAllAsync(ssc =>
                    ssc.SpeciesId == dto.SpeciesId
                );
                var maxSeq =
                    existingForSpecies.Count == 0 ? 0 : existingForSpecies.Max(s => s.Sequence);

                // If no sequence provided, auto-assign to end
                if (!dto.Sequence.HasValue)
                {
                    dto.Sequence = maxSeq + 1;
                }

                // normalize requested sequence to be at least 1 and at most maxSeq+1
                var requestedSeq = dto.Sequence.Value < 1 ? 1 : dto.Sequence.Value;
                if (requestedSeq > maxSeq + 1)
                    requestedSeq = maxSeq + 1;

                // shift using a safe two-phase approach to avoid unique index cycles
                var toShift = existingForSpecies
                    .Where(s => s.Sequence >= requestedSeq)
                    .OrderByDescending(s => s.Sequence)
                    .ToList();
                const int OFFSET = 1000000;
                if (toShift.Count > 0)
                {
                    foreach (var s in toShift)
                        s.Sequence = s.Sequence + OFFSET;

                    await _unitOfWork.SaveChangesAsync();

                    foreach (var s in toShift)
                        s.Sequence = s.Sequence - OFFSET + 1;
                }

                newConfig.Sequence = requestedSeq;

                await repo.AddAsync(newConfig);
                await _unitOfWork.SaveChangesAsync();

                var createdDto = await repo.FirstOrDefaultAsync(
                    new SpeciesStageConfigByIdSpec(newConfig.Id)
                );

                if (createdDto != null)
                    await WriteCreateAuditLogAsync(createdDto);

                // Recompute estimates for batches of this species
                await _farmingBatchService.RecomputeEstimatedYieldBySpeciesAsync(
                    newConfig.SpeciesId
                );

                return Result<SpeciesStageConfigDto>.Success(
                    _mapper.Map<SpeciesStageConfigDto>(newConfig),
                    "Tạo cấu hình giai đoạn sinh trưởng của cá thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SpeciesStageConfig");
                return Result<SpeciesStageConfigDto>.Failure(
                    "Lỗi khi tạo cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<IReadOnlyList<SpeciesStageConfigDto>>> ReorderSpeciesStageConfigs(
            ReorderSpeciesStageConfigsDto dto
        )
        {
            try
            {
                var repo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                var allForSpecies = await repo.FindAllAsync(s => s.SpeciesId == dto.SpeciesId);

                // Validate all provided IDs exist for this species
                var idSet = allForSpecies.Select(s => s.Id).ToHashSet();
                var invalidIds = dto.OrderedIds.Where(id => !idSet.Contains(id)).ToList();
                if (invalidIds.Count != 0)
                {
                    return Result<IReadOnlyList<SpeciesStageConfigDto>>.Failure(
                        "Một hoặc nhiều ID cấu hình giai đoạn không hợp lệ.",
                        ResultType.BadRequest
                    );
                }

                // Ensure all existing configs are included in the order
                var missingIds = idSet.Where(id => !dto.OrderedIds.Contains(id)).ToList();
                if (missingIds.Count != 0)
                {
                    return Result<IReadOnlyList<SpeciesStageConfigDto>>.Failure(
                        "Phải bao gồm tất cả cấu hình giai đoạn hiện có trong danh sách sắp xếp.",
                        ResultType.BadRequest
                    );
                }

                // Use two-phase approach to assign new sequences
                const int OFFSET = 1000000;
                var byId = allForSpecies.ToDictionary(s => s.Id);

                // Phase 1: set all to temporary large values
                foreach (var s in allForSpecies)
                    s.Sequence = s.Sequence + OFFSET;
                await _unitOfWork.SaveChangesAsync();

                // Phase 2: assign new contiguous sequence based on position in orderedIds
                for (int i = 0; i < dto.OrderedIds.Count; i++)
                {
                    byId[dto.OrderedIds[i]].Sequence = i + 1;
                }
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache for all changed configs
                foreach (var s in allForSpecies)
                    _telemetryCache.InvalidateStageConfig(s.Id);

                // Recompute estimated yields for all batches of this species
                await _farmingBatchService.RecomputeEstimatedYieldBySpeciesAsync(dto.SpeciesId);

                // Return the updated ordered list
                var dtos = new List<SpeciesStageConfigDto>();
                foreach (var id in dto.OrderedIds)
                {
                    var dto_ = await repo.FirstOrDefaultAsync(new SpeciesStageConfigByIdSpec(id));
                    if (dto_ != null)
                        dtos.Add(dto_);
                }

                return Result<IReadOnlyList<SpeciesStageConfigDto>>.Success(
                    dtos,
                    "Sắp xếp cấu hình giai đoạn thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering SpeciesStageConfigs");
                return Result<IReadOnlyList<SpeciesStageConfigDto>>.Failure(
                    "Lỗi khi sắp xếp cấu hình giai đoạn.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result> DeleteSpeciesStageConfig(Guid id)
        {
            try
            {
                var config = await _unitOfWork.GetRepository<SpeciesStageConfig>().GetByIdAsync(id);
                if (config == null)
                    return Result.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá không tồn tại.",
                        ResultType.NotFound
                    );

                var oldDto = await _unitOfWork
                    .GetRepository<SpeciesStageConfig>()
                    .FirstOrDefaultAsync(new SpeciesStageConfigByIdSpec(id));

                // Capture species and sequence before deleting so we can re-sequence remaining items
                var speciesId = config.SpeciesId;
                var deletedSeq = config.Sequence;

                var repo = _unitOfWork.GetRepository<SpeciesStageConfig>();

                repo.Delete(config);
                await _unitOfWork.SaveChangesAsync();

                // Re-sequence all remaining configs for the species to be contiguous starting at 1
                var allForSpecies = await repo.FindAllAsync(s => s.SpeciesId == speciesId);
                var ordered = allForSpecies.OrderBy(s => s.Sequence).ToList();

                const int OFFSET = 1000000;
                if (ordered.Count > 0)
                {
                    // assign temporary large values to avoid unique index conflicts
                    foreach (var s in ordered)
                        s.Sequence = s.Sequence + OFFSET;

                    await _unitOfWork.SaveChangesAsync();

                    // set contiguous sequence values starting from 1
                    for (int i = 0; i < ordered.Count; i++)
                    {
                        ordered[i].Sequence = i + 1;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    // Invalidate telemetry cache for adjusted configs
                    foreach (var s in ordered)
                        _telemetryCache.InvalidateStageConfig(s.Id);
                }

                // Invalidate cache for the deleted config id as well
                _telemetryCache.InvalidateStageConfig(id);

                if (oldDto != null)
                    await WriteDeleteAuditLogAsync(oldDto);

                return Result.Success("Xóa cấu hình giai đoạn sinh trưởng của cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting SpeciesStageConfig");
                return Result.Failure(
                    "Lỗi khi xóa cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<SpeciesStageConfigDto>> GetSpeciesStageConfigById(Guid id)
        {
            try
            {
                var config = await _unitOfWork
                    .GetRepository<SpeciesStageConfig>()
                    .FirstOrDefaultAsync(new SpeciesStageConfigByIdSpec(id));

                if (config == null)
                    return Result<SpeciesStageConfigDto>.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá không tồn tại.",
                        ResultType.NotFound
                    );

                return Result<SpeciesStageConfigDto>.Success(
                    _mapper.Map<SpeciesStageConfigDto>(config),
                    "Lấy cấu hình giai đoạn sinh trưởng của cá thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SpeciesStageConfig by ID");
                return Result<SpeciesStageConfigDto>.Failure(
                    "Lỗi khi truy xuất cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<PaginatedResult<SpeciesStageConfigDto>> GetAllSpeciesStageConfigsAsync(
            SpeciesStageConfigListRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Bắt đầu lấy danh sách cấu hình giai đoạn sinh trưởng (Page: {Page}, PageSize: {PageSize})",
                    request.Page,
                    request.PageSize
                );

                var repository = _unitOfWork.GetRepository<SpeciesStageConfig>();
                var pagedResult = await repository.GetPagedAsync(
                    new SpeciesStageConfigListSpec(request),
                    request.Page,
                    request.PageSize
                );

                var configDtos = pagedResult.Items;

                _logger.LogInformation(
                    "Lấy danh sách cấu hình giai đoạn sinh trưởng thành công: {Count} cấu hình",
                    configDtos.Count
                );

                return new PaginatedResult<SpeciesStageConfigDto>
                {
                    Message =
                        configDtos.Count == 0
                            ? "Không có cấu hình giai đoạn sinh trưởng nào"
                            : "Lấy danh sách cấu hình giai đoạn sinh trưởng thành công",
                    Data = configDtos,
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
                _logger.LogError(ex, "Lỗi khi truy xuất danh sách cấu hình giai đoạn sinh trưởng");

                return new PaginatedResult<SpeciesStageConfigDto>
                {
                    Message = "Lỗi khi truy xuất danh sách cấu hình giai đoạn sinh trưởng",
                    Data = Array.Empty<SpeciesStageConfigDto>(),
                    Meta = null,
                    Links = null,
                };
            }
        }

        public async Task<PaginatedResult<SpeciesStageConfigDto>> GetSpeciesStageConfigsBySpeciesId(
            Guid speciesId,
            SpeciesStageConfigListRequest request
        )
        {
            var species = await _unitOfWork
                .GetRepository<Species>()
                .AnyAsync(s => s.Id == speciesId);

            if (species == false)
                return new PaginatedResult<SpeciesStageConfigDto>
                {
                    Message = "Loài cá không tồn tại",
                    Data = Array.Empty<SpeciesStageConfigDto>(),
                    Meta = null,
                    Links = null,
                };

            var pagedResult = await _unitOfWork
                .GetRepository<SpeciesStageConfig>()
                .GetPagedAsync(
                    new SpeciesStageConfigBySpeciesIdSpec(speciesId, request),
                    request.Page,
                    request.PageSize
                );

            return new PaginatedResult<SpeciesStageConfigDto>
            {
                Message =
                    pagedResult.Items.Count == 0
                        ? "Không có cấu hình giai đoạn sinh trưởng nào cho loài cá này"
                        : "Lấy danh sách cấu hình giai đoạn sinh trưởng cho loài cá thành công",
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

        public async Task<Result> UpdateSpeciesStageConfig(Guid id, UpdateSpeciesStageConfigDto dto)
        {
            try
            {
                var config = await _unitOfWork.GetRepository<SpeciesStageConfig>().GetByIdAsync(id);

                if (config == null)
                    return Result.Failure(
                        "Cấu hình giai đoạn sinh trưởng của cá không tồn tại.",
                        ResultType.NotFound
                    );

                var oldDto = await _unitOfWork
                    .GetRepository<SpeciesStageConfig>()
                    .FirstOrDefaultAsync(new SpeciesStageConfigByIdSpec(id));

                if (dto.FeedTypeIds != null)
                {
                    if (dto.FeedTypeIds.Count == 0)
                        return Result.Failure(
                            "Phải chọn ít nhất một kiểu cho ăn.",
                            ResultType.BadRequest
                        );

                    var requestedFeedTypeIds = dto.FeedTypeIds.Distinct().ToList();
                    var feedTypes = await _unitOfWork
                        .GetRepository<FeedType>()
                        .FindAllAsync(ft => requestedFeedTypeIds.Contains(ft.Id));

                    if (feedTypes.Count != requestedFeedTypeIds.Count)
                        return Result<SpeciesStageConfigDto>.Failure(
                            "Kiểu cho ăn không tồn tại.",
                            ResultType.BadRequest
                        );

                    // Avoid reassigning the same feed types which can cause EF to try inserting
                    // duplicate join rows. Fetch existing feed type ids via spec and compare.
                    var existingFeedTypeIds = oldDto?.FeedTypeIds ?? [];

                    var setsEqual =
                        existingFeedTypeIds.Count == requestedFeedTypeIds.Count
                        && !existingFeedTypeIds.Except(requestedFeedTypeIds).Any();

                    if (!setsEqual)
                    {
                        config.FeedTypes = feedTypes.ToList();
                    }
                }

                // If sequence is provided, auto-adjust other sequences to make room
                if (dto.Sequence != null)
                {
                    var repo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                    var existing = await repo.FindAllAsync(s =>
                        s.SpeciesId == config.SpeciesId && s.Id != id
                    );
                    var maxSeq = existing.Count == 0 ? 0 : existing.Max(s => s.Sequence);

                    var requestedSeq = dto.Sequence.Value < 1 ? 1 : dto.Sequence.Value;
                    if (requestedSeq > maxSeq + 1)
                        requestedSeq = maxSeq + 1;

                    // If moving down (increasing sequence), shift others between old+1 and new down by -1
                    // using a two-phase offset update to avoid unique index cycles
                    var oldSeq = config.Sequence;
                    const int OFFSET = 1000000;
                    if (requestedSeq > oldSeq)
                    {
                        var between = existing
                            .Where(s => s.Sequence > oldSeq && s.Sequence <= requestedSeq)
                            .OrderByDescending(s => s.Sequence)
                            .ToList();
                        if (between.Count > 0)
                        {
                            foreach (var s in between)
                                s.Sequence = s.Sequence + OFFSET;

                            await _unitOfWork.SaveChangesAsync();

                            foreach (var s in between)
                                s.Sequence = s.Sequence - OFFSET - 1;
                        }
                    }
                    else if (requestedSeq < oldSeq)
                    {
                        var between = existing
                            .Where(s => s.Sequence >= requestedSeq && s.Sequence < oldSeq)
                            .OrderBy(s => s.Sequence)
                            .ToList();
                        if (between.Count > 0)
                        {
                            foreach (var s in between)
                                s.Sequence = s.Sequence + OFFSET;

                            await _unitOfWork.SaveChangesAsync();

                            foreach (var s in between)
                                s.Sequence = s.Sequence - OFFSET + 1;
                        }
                    }

                    // finally map and set sequence
                    _mapper.Map(dto, config);
                    config.Sequence = requestedSeq;
                    // only update amount/frequency when provided
                    if (dto.AmountPer100Fish != null)
                        config.AmountPer100Fish = dto.AmountPer100Fish.Value;
                    if (dto.FrequencyPerDay != null)
                        config.FrequencyPerDay = dto.FrequencyPerDay.Value;
                }
                else
                {
                    _mapper.Map(dto, config);
                    if (dto.AmountPer100Fish != null)
                        config.AmountPer100Fish = dto.AmountPer100Fish.Value;
                    if (dto.FrequencyPerDay != null)
                        config.FrequencyPerDay = dto.FrequencyPerDay.Value;
                }
                await _unitOfWork.SaveChangesAsync();

                var updatedDto = await _unitOfWork
                    .GetRepository<SpeciesStageConfig>()
                    .FirstOrDefaultAsync(new SpeciesStageConfigByIdSpec(id));

                if (oldDto != null && updatedDto != null)
                    await WriteUpdateAuditLogAsync(oldDto, updatedDto);

                _telemetryCache.InvalidateStageConfig(id);

                // Recompute for species after change
                await _farmingBatchService.RecomputeEstimatedYieldBySpeciesAsync(config.SpeciesId);

                return Result.Success("Cập nhật cấu hình giai đoạn sinh trưởng của cá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating SpeciesStageConfig");
                return Result.Failure(
                    "Lỗi khi cập nhật cấu hình giai đoạn sinh trưởng của cá.",
                    ResultType.Unexpected
                );
            }
        }

        #region Audit Log Helpers
        private static object ToAuditSnapshot(SpeciesStageConfigDto dto)
        {
            return new
            {
                dto.SpeciesName,
                dto.GrowthStageName,
                dto.Sequence,
                dto.FeedTypeNames,
                dto.AmountPer100Fish,
                dto.FrequencyPerDay,
                dto.MaxStockingDensity,
                dto.ExpectedDurationDays,
                dto.ExpectedWeightKgPerFish,
                dto.SurvivalRate,
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
                        AuditLogEntityType.SpeciesStageConfig,
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
                    AuditLogEntityType.SpeciesStageConfig,
                    entityId
                );
            }
        }

        private async Task WriteCreateAuditLogAsync(SpeciesStageConfigDto dto)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Create,
                dto.Id.ToString(),
                null,
                ToAuditSnapshot(dto),
                "create-species-stage-config"
            );
        }

        private async Task WriteUpdateAuditLogAsync(
            SpeciesStageConfigDto oldDto,
            SpeciesStageConfigDto newDto
        )
        {
            await WriteAuditLogAsync(
                AuditLogActions.Update,
                newDto.Id.ToString(),
                ToAuditSnapshot(oldDto),
                ToAuditSnapshot(newDto),
                "update-species-stage-config"
            );
        }

        private async Task WriteDeleteAuditLogAsync(SpeciesStageConfigDto dto)
        {
            await WriteAuditLogAsync(
                AuditLogActions.Delete,
                dto.Id.ToString(),
                ToAuditSnapshot(dto),
                null,
                "delete-species-stage-config"
            );
        }
        #endregion
    }
}
