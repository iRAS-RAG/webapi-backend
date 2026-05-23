using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.Common.Utils;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.FarmingBatchSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class FarmingBatchService : IFarmingBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FarmingBatchService> _logger;
        private readonly ITelemetryCacheService _telemetryCache;

        public FarmingBatchService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FarmingBatchService> logger,
            ITelemetryCacheService telemetryCache
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _telemetryCache = telemetryCache;
        }

        // Compute estimated yield for a batch and optionally persist
        public async Task<(
            int EstimatedCount,
            double? EstimatedWeightKg
        )> ComputeEstimatedYieldAsync(FarmingBatch batch, bool persist = false)
        {
            // Load species and current stage sequence
            var currentStage = await _unitOfWork
                .GetRepository<SpeciesStageConfig>()
                .GetByIdAsync(batch.CurrentStageConfigId);

            if (currentStage == null)
                return (0, null);

            var speciesId = currentStage.SpeciesId;

            var stages = await _unitOfWork
                .GetRepository<SpeciesStageConfig>()
                .FindAllAsync(s => s.SpeciesId == speciesId && s.Sequence >= currentStage.Sequence);

            var ordered = stages.OrderBy(s => s.Sequence);
            var result = IRasRag.Application.Common.Utils.YieldEstimator.Estimate(
                ordered,
                batch.CurrentQuantity
            );

            if (persist)
            {
                batch.EstimatedHarvestCount = result.EstimatedCount;
                batch.EstimatedHarvestWeightKg = result.EstimatedWeightKg;
                _unitOfWork.GetRepository<FarmingBatch>().Update(batch);
                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }

        public async Task<int> RecomputeEstimatedYieldBySpeciesAsync(Guid speciesId)
        {
            try
            {
                var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var batches = await batchRepo.GetAllAsync();
                var recomputedCount = 0;

                foreach (var batch in batches)
                {
                    // ensure we have current stage loaded
                    var currentStage = await _unitOfWork
                        .GetRepository<SpeciesStageConfig>()
                        .GetByIdAsync(batch.CurrentStageConfigId);

                    if (currentStage == null)
                        continue;
                    if (currentStage.SpeciesId != speciesId)
                        continue;

                    await ComputeEstimatedYieldAsync(batch, persist: true);
                    recomputedCount++;
                }

                return recomputedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi tính lại sản lượng ước tính theo loài với SpeciesId {SpeciesId}",
                    speciesId
                );
                return 0;
            }
        }

        public async Task<Result<IReadOnlyList<PlannedStageDto>>> GetPlannedStagesByBatchIdAsync(
            Guid batchId
        )
        {
            try
            {
                var repo = _unitOfWork.GetRepository<FarmingBatch>();
                var batch = await repo.FirstOrDefaultAsync(
                    new FarmingBatchWithStagesByIdSpec(batchId)
                );

                if (batch == null)
                {
                    return Result<IReadOnlyList<PlannedStageDto>>.Failure(
                        "Không tìm thấy lô nuôi",
                        ResultType.NotFound
                    );
                }

                var ordered = batch.BatchStages.OrderBy(bs => bs.Sequence).ToList();
                var dtos = new List<PlannedStageDto>();
                var sscRepo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                foreach (var bs in ordered)
                {
                    var ssc = bs.SpeciesStageConfig;
                    if (ssc == null || ssc.GrowthStage == null || ssc.GrowthStageId == Guid.Empty)
                    {
                        // try to reload species stage config with includes
                        var reloaded = await sscRepo.FirstOrDefaultAsync(
                            new IRasRag.Application.Specifications.SpeciesStageConfigSpecifications.SpeciesStageConfigWithIncludesByIdSpec(
                                bs.SpeciesStageConfigId
                            )
                        );
                        if (reloaded != null)
                        {
                            bs.SpeciesStageConfig = reloaded;
                            ssc = reloaded;
                        }
                    }

                    // Build DTO explicitly to avoid relying on mapping when navigation is partially loaded
                    var dto = new PlannedStageDto
                    {
                        Id = bs.Id,
                        Sequence = bs.Sequence,
                        SpeciesStageConfigId = bs.SpeciesStageConfigId,
                        GrowthStageId = ssc?.GrowthStage?.Id ?? ssc?.GrowthStageId ?? Guid.Empty,
                        StageName = ssc?.GrowthStage?.Name ?? string.Empty,
                        ExpectedDurationDays = bs.ExpectedDurationDays,
                        EstimatedStartDate = bs.EstimatedStartDate,
                        EstimatedEndDate = bs.EstimatedEndDate,
                        ActualStartDate = bs.ActualStartDate,
                        ActualEndDate = bs.ActualEndDate,
                        AmountPer100Fish = ssc?.AmountPer100Fish ?? 0,
                        FrequencyPerDay = ssc?.FrequencyPerDay ?? 0,
                        MaxStockingDensity = ssc?.MaxStockingDensity,
                        ExpectedWeightKgPerFish = ssc?.ExpectedWeightKgPerFish,
                        SurvivalRate = ssc?.SurvivalRate,
                        FeedTypeNames =
                            ssc?.FeedTypes?.Select(ft => ft.Name).ToList() ?? new List<string>(),
                    };

                    dtos.Add(dto);
                }

                return Result<IReadOnlyList<PlannedStageDto>>.Success(
                    dtos,
                    "Lấy danh sách giai đoạn dự kiến thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi lấy giai đoạn dự kiến cho lô nuôi {BatchId}",
                    batchId
                );
                return Result<IReadOnlyList<PlannedStageDto>>.Failure(
                    "Lỗi khi lấy giai đoạn dự kiến",
                    ResultType.Unexpected
                );
            }
        }

        #region Get Methods
        public async Task<PaginatedResult<FarmingBatchDto>> GetAllFarmingBatchesAsync(
            FarmingBatchListRequest request
        )
        {
            try
            {
                var spec = new FarmingBatchDtoListSpec(request);
                var pagedResult = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .GetPagedAsync(spec, request.Page, request.PageSize);

                var meta = PaginationBuilder.BuildPaginationMetadata(
                    request.Page,
                    request.PageSize,
                    pagedResult.TotalItems
                );

                var links = PaginationBuilder.BuildPaginationLinks(
                    request.Page,
                    request.PageSize,
                    pagedResult.TotalItems
                );

                return new PaginatedResult<FarmingBatchDto>
                {
                    Message = "Lấy danh sách lô nuôi thành công",
                    Data = pagedResult.Items.ToList(),
                    Meta = meta,
                    Links = links,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lô nuôi");
                return new PaginatedResult<FarmingBatchDto>
                {
                    Message = "Lỗi khi lấy danh sách lô nuôi",
                    Data = new List<FarmingBatchDto>(),
                    Meta = new PaginationMeta(),
                    Links = new PaginationLinks(),
                };
            }
        }

        public async Task<
            Result<IReadOnlyList<ActiveFarmingBatchResponseDto>>
        > GetActiveFarmingBatchByFishTankIdAsync(Guid fishTankId)
        {
            try
            {
                var tank = await _unitOfWork.GetRepository<FishTank>().GetByIdAsync(fishTankId);
                if (tank == null)
                {
                    return Result<IReadOnlyList<ActiveFarmingBatchResponseDto>>.Failure(
                        "Bể cá không tồn tại",
                        ResultType.NotFound
                    );
                }

                var list = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .ListAsync(new ActiveFarmingBatchDtoListSpec(fishTankId));
                return Result<IReadOnlyList<ActiveFarmingBatchResponseDto>>.Success(
                    list,
                    "Lấy danh sách lô nuôi đang hoạt động thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi lấy danh sách lô nuôi đang hoạt động cho bể cá với ID {FishTankId}",
                    fishTankId
                );
                return Result<IReadOnlyList<ActiveFarmingBatchResponseDto>>.Failure(
                    "Lỗi khi lấy danh sách lô nuôi đang hoạt động",
                    ResultType.Unexpected
                );
            }
        }

        public async Task<Result<FarmingBatchDto>> GetFarmingBatchByIdAsync(Guid id)
        {
            try
            {
                var farmingBatchDto = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .FirstOrDefaultAsync(new FarmingBatchDtoByIdSpec(id));

                if (farmingBatchDto == null)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Không tìm thấy lô nuôi",
                        ResultType.NotFound
                    );
                }

                return Result<FarmingBatchDto>.Success(
                    farmingBatchDto,
                    "Lấy thông tin lô nuôi thành công"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin lô nuôi với ID {Id}", id);
                return Result<FarmingBatchDto>.Failure(
                    "Lỗi khi lấy thông tin lô nuôi",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        // Recalculate estimates for all batches of the given species
        public async Task RecalculateEstimatesForSpeciesAsync(Guid speciesId)
        {
            var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();
            // find batches whose current stage config belongs to this species
            var allBatches = await batchRepo.GetAllAsync();
            var affected = allBatches
                .Where(b =>
                {
                    var cfg = b.CurrentStageConfig;
                    return cfg != null && cfg.SpeciesId == speciesId;
                })
                .ToList();

            foreach (var batch in affected)
            {
                // reload batch with current data
                var reloaded = await batchRepo.GetByIdAsync(batch.Id);
                if (reloaded == null)
                    continue;
                await ComputeEstimatedYieldAsync(reloaded, persist: true);
                _telemetryCache.InvalidateBatch(reloaded.FishTankId);
            }
        }

        #region Create Methods
        public async Task<Result<FarmingBatchDto>> CreateFarmingBatchAsync(
            CreateFarmingBatchDto createDto
        )
        {
            try
            {
                // Validate FishTank exists
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var fishTankExists = await fishTankRepo.AnyAsync(ft =>
                    ft.Id == createDto.FishTankId
                );
                if (!fishTankExists)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Bể cá không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Guard: no ACTIVE batch in the same tank
                var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var hasActiveBatch = await farmingBatchRepo.AnyAsync(fb =>
                    fb.FishTankId == createDto.FishTankId && fb.Status == FarmingBatchStatus.ACTIVE
                );
                if (hasActiveBatch)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Bể cá đang có lô nuôi hoạt động, không thể tạo lô nuôi mới",
                        ResultType.Conflict
                    );
                }

                // Guard: no PAUSED batch in the same tank that blocks a new batch
                var pausedBatch = await farmingBatchRepo.FirstOrDefaultAsync(fb =>
                    fb.FishTankId == createDto.FishTankId && fb.Status == FarmingBatchStatus.PAUSED
                );
                if (pausedBatch != null)
                {
                    var pausedReason = pausedBatch.PausedReason;
                    if (!pausedReason.HasValue || !pausedReason.Value.AllowsAddNewBatch())
                    {
                        return Result<FarmingBatchDto>.Failure(
                            "Bể cá đang chứa cá từ lô nuôi đang tạm dừng, không thể tạo lô nuôi mới",
                            ResultType.Conflict
                        );
                    }
                }

                // Validate Species exists
                var speciesRepo = _unitOfWork.GetRepository<Species>();
                var speciesExists = await speciesRepo.AnyAsync(s => s.Id == createDto.SpeciesId);
                if (!speciesExists)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Loài cá không tồn tại",
                        ResultType.BadRequest
                    );
                }

                // Load all stage configs for species ordered by sequence
                var stageConfigRepo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                var stageConfigs = await stageConfigRepo.ListAsync(
                    new IRasRag.Application.Specifications.SpecificationHelpers.SpecBySpeciesOrderedSpec(
                        createDto.SpeciesId
                    )
                );
                if (stageConfigs == null || !stageConfigs.Any())
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Không tìm thấy cấu hình giai đoạn cho loài này",
                        ResultType.BadRequest
                    );
                }

                // Ensure each stage has ExpectedDurationDays
                if (stageConfigs.Any(sc => !sc.ExpectedDurationDays.HasValue))
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Một hoặc nhiều cấu hình giai đoạn thiếu ExpectedDurationDays",
                        ResultType.BadRequest
                    );
                }

                // Validate inputs
                var trimmedName = createDto.Name?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(trimmedName))
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Tên lô nuôi không được để trống",
                        ResultType.BadRequest
                    );
                }

                var trimmedUnitOfMeasure = createDto.UnitOfMeasure?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(trimmedUnitOfMeasure))
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Đơn vị đo không được để trống",
                        ResultType.BadRequest
                    );
                }

                if (createDto.InitialQuantity < 0)
                {
                    return Result<FarmingBatchDto>.Failure(
                        "Số lượng ban đầu phải lớn hơn hoặc bằng 0",
                        ResultType.BadRequest
                    );
                }

                // Map and create FarmingBatch
                var farmingBatch = _mapper.Map<FarmingBatch>(createDto);
                farmingBatch.Name = trimmedName;
                farmingBatch.UnitOfMeasure = trimmedUnitOfMeasure;
                farmingBatch.CurrentStageConfigId = stageConfigs
                    .OrderBy(s => s.Sequence)
                    .First()
                    .Id;

                // Build BatchStage entries from stageConfigs
                var orderedStages = stageConfigs.OrderBy(s => s.Sequence).ToList();
                var batchStages = new List<BatchStage>();
                var stageStart = createDto.StartDate;
                foreach (var sc in orderedStages)
                {
                    var duration = sc.ExpectedDurationDays!.Value;
                    var stageEnd = stageStart.AddDays(duration);

                    var bs = new BatchStage
                    {
                        Id = Guid.NewGuid(),
                        SpeciesStageConfigId = sc.Id,
                        Sequence = sc.Sequence,
                        EstimatedStartDate = stageStart,
                        EstimatedEndDate = stageEnd,
                        ExpectedDurationDays = duration,
                        // First stage becomes active immediately at batch StartDate
                        ActualStartDate = batchStages.Count == 0 ? stageStart : null,
                    };

                    batchStages.Add(bs);
                    // next stage start is stageEnd
                    stageStart = stageEnd;
                }

                // Set estimated harvest date to last stage end
                farmingBatch.EstimatedHarvestDate = batchStages.Last().EstimatedEndDate;

                // Add farmingBatch and batchStages
                await farmingBatchRepo.AddAsync(farmingBatch);
                // Ensure batchStages point to farmingBatch after save
                await _unitOfWork.SaveChangesAsync();

                // Assign FarmingBatchId and add batch stages
                foreach (var bs in batchStages)
                {
                    bs.FarmingBatchId = farmingBatch.Id;
                }
                var batchStageRepo = _unitOfWork.GetRepository<BatchStage>();
                await batchStageRepo.AddRangeAsync(batchStages);

                await _unitOfWork.SaveChangesAsync();

                // compute and persist estimated yield after batch and stages are created
                await ComputeEstimatedYieldAsync(farmingBatch, persist: true);

                _telemetryCache.InvalidateBatch(farmingBatch.FishTankId);

                var farmingBatchDto = await _unitOfWork
                    .GetRepository<FarmingBatch>()
                    .FirstOrDefaultAsync(new FarmingBatchDtoByIdSpec(farmingBatch.Id));

                return Result<FarmingBatchDto>.Success(farmingBatchDto!, "Tạo lô nuôi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lô nuôi");
                return Result<FarmingBatchDto>.Failure(
                    "Lỗi khi tạo lô nuôi",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Update Methods
        public async Task<Result> UpdateFarmingBatchAsync(Guid id, UpdateFarmingBatchDto updateDto)
        {
            try
            {
                var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var farmingBatch = await farmingBatchRepo.GetByIdAsync(id);

                if (farmingBatch == null)
                {
                    return Result.Failure("Không tìm thấy lô nuôi", ResultType.NotFound);
                }

                if (
                    farmingBatch.Status == FarmingBatchStatus.HARVESTED
                    || farmingBatch.Status == FarmingBatchStatus.TERMINATED
                )
                {
                    return Result.Failure(
                        "Không thể cập nhật lô nuôi đã thu hoạch/hủy bỏ",
                        ResultType.BadRequest
                    );
                }

                // Validate inputs
                if (updateDto.Name != null)
                {
                    var trimmedName = updateDto.Name.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedName))
                    {
                        return Result.Failure(
                            "Tên lô nuôi không được để trống",
                            ResultType.BadRequest
                        );
                    }
                    updateDto.Name = trimmedName;
                }

                if (updateDto.UnitOfMeasure != null)
                {
                    var trimmedUnitOfMeasure = updateDto.UnitOfMeasure.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedUnitOfMeasure))
                    {
                        return Result.Failure(
                            "Đơn vị đo không được để trống",
                            ResultType.BadRequest
                        );
                    }
                    updateDto.UnitOfMeasure = trimmedUnitOfMeasure;
                }

                if (updateDto.CurrentQuantity.HasValue && updateDto.CurrentQuantity.Value < 0)
                {
                    return Result.Failure(
                        "Số lượng hiện tại phải lớn hơn hoặc bằng 0",
                        ResultType.BadRequest
                    );
                }

                // Map and update
                _mapper.Map(updateDto, farmingBatch);
                farmingBatchRepo.Update(farmingBatch);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Cập nhật lô nuôi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật lô nuôi với ID {Id}", id);
                return Result.Failure("Lỗi khi cập nhật lô nuôi", ResultType.Unexpected);
            }
        }

        public async Task<Result> HarvestBatchAsync(
            Guid id,
            DateTime harvestDate,
            bool force = false
        )
        {
            try
            {
                var repo = _unitOfWork.GetRepository<FarmingBatch>();
                var batch = await repo.FirstOrDefaultAsync(
                    new FarmingBatchWithStagesByIdSpec(id),
                    QueryType.IncludeDeleted
                );

                if (batch == null)
                {
                    return Result.Failure("Không tìm thấy lô nuôi", ResultType.NotFound);
                }

                if (
                    batch.Status == FarmingBatchStatus.HARVESTED
                    || batch.Status == FarmingBatchStatus.TERMINATED
                )
                {
                    return Result.Failure(
                        "Không thể thu hoạch lô nuôi đã thu hoạch/hủy bỏ",
                        ResultType.BadRequest
                    );
                }

                var orderedStages =
                    batch.BatchStages?.OrderBy(s => s.Sequence).ToList() ?? new List<BatchStage>();
                var plannedEnd = orderedStages.LastOrDefault()?.EstimatedEndDate;

                if (plannedEnd.HasValue && harvestDate < plannedEnd.Value && !force)
                {
                    return Result.Failure(
                        "Không thể thu hoạch trước ngày kết thúc dự kiến của kế hoạch. Sử dụng force=true để ghi đè.",
                        ResultType.BadRequest
                    );
                }

                // Update stage actuals
                foreach (var stage in orderedStages)
                {
                    if (!stage.ActualStartDate.HasValue)
                    {
                        stage.ActualStartDate = stage.EstimatedStartDate;
                    }

                    if (stage.EstimatedEndDate <= harvestDate)
                    {
                        // stage completed before harvest
                        stage.ActualEndDate = stage.EstimatedEndDate;
                    }
                    else if (
                        stage.EstimatedStartDate < harvestDate
                        && stage.EstimatedEndDate > harvestDate
                    )
                    {
                        // harvested mid-stage
                        stage.ActualEndDate = harvestDate;
                    }
                    else
                    {
                        // future stages remain with null ActualEndDate
                    }
                }

                batch.Status = FarmingBatchStatus.HARVESTED;
                batch.ActualHarvestDate = harvestDate;
                if (orderedStages.Any())
                {
                    batch.CurrentStageConfigId = orderedStages.Last().SpeciesStageConfigId;
                }

                // Remove navigation instances to prevent EF tracking conflicts when attaching the graph.
                // We only need FK values persisted (e.g., SpeciesStageConfigId), navigation properties are not required here.
                if (batch.BatchStages != null)
                {
                    foreach (var bs in batch.BatchStages)
                    {
                        bs.SpeciesStageConfig = null;
                    }
                }

                repo.Update(batch);
                await _unitOfWork.SaveChangesAsync();
                _telemetryCache.InvalidateBatch(batch.FishTankId);
                return Result.Success("Thu hoạch lô nuôi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thu hoạch lô nuôi với ID {Id}", id);
                return Result.Failure("Lỗi khi thu hoạch lô nuôi", ResultType.Unexpected);
            }
        }
        #endregion

        #region Delete Methods
        public async Task<Result> DeleteFarmingBatchAsync(Guid id)
        {
            try
            {
                var farmingBatchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var farmingBatch = await farmingBatchRepo.GetByIdAsync(id);

                if (farmingBatch == null)
                {
                    return Result.Failure("Không tìm thấy lô nuôi", ResultType.NotFound);
                }

                // Check if there are related FeedingLogs
                var feedingLogRepo = _unitOfWork.GetRepository<FeedingLog>();
                var hasFeedingLogs = await feedingLogRepo.AnyAsync(fl => fl.FarmingBatchId == id);
                if (hasFeedingLogs)
                {
                    return Result.Failure(
                        "Không thể xóa lô nuôi vì có dữ liệu nhật ký cho ăn liên quan",
                        ResultType.BadRequest
                    );
                }

                // Check if there are related MortalityLogs
                var mortalityLogRepo = _unitOfWork.GetRepository<MortalityLog>();
                var hasMortalityLogs = await mortalityLogRepo.AnyAsync(ml => ml.BatchId == id);
                if (hasMortalityLogs)
                {
                    return Result.Failure(
                        "Không thể xóa lô nuôi vì có dữ liệu nhật ký chết liên quan",
                        ResultType.BadRequest
                    );
                }

                // Check if there are related Alerts
                var alertRepo = _unitOfWork.GetRepository<Alert>();
                var hasAlerts = await alertRepo.AnyAsync(a => a.FarmingBatchId == id);
                if (hasAlerts)
                {
                    return Result.Failure(
                        "Không thể xóa lô nuôi vì có cảnh báo liên quan",
                        ResultType.BadRequest
                    );
                }

                farmingBatchRepo.Delete(farmingBatch);
                await _unitOfWork.SaveChangesAsync();
                _telemetryCache.InvalidateBatch(farmingBatch.FishTankId);

                return Result.Success("Xóa lô nuôi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa lô nuôi với ID {Id}", id);
                return Result.Failure("Lỗi khi xóa lô nuôi", ResultType.Unexpected);
            }
        }
        #endregion
    }
}
