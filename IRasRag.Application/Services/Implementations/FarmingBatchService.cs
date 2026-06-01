using AutoMapper;
using IRasRag.Application.Common.Constants;
using IRasRag.Application.Common.Interfaces;
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
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly IMapper _mapper;
        private readonly ILogger<FarmingBatchService> _logger;
        private readonly ITelemetryCacheService _telemetryCache;
        private readonly IAuditLogService _auditLogService;

        public FarmingBatchService(
            IUnitOfWork unitOfWork,
            IRecommendationCalculator recommendationCalculator,
            IMapper mapper,
            ILogger<FarmingBatchService> logger,
            ITelemetryCacheService telemetryCache,
            IAuditLogService auditLogService
        )
        {
            _unitOfWork = unitOfWork;
            _recommendationCalculator = recommendationCalculator;
            _mapper = mapper;
            _logger = logger;
            _telemetryCache = telemetryCache;
            _auditLogService = auditLogService;
        }

        #region FCR Calculation
        public async Task<double?> ComputeAndPersistFcrAsync(Guid batchId)
        {
            try
            {
                var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var batch = await batchRepo.FirstOrDefaultAsync(
                    new IRasRag.Application.Specifications.FarmingBatchSpecifications.FarmingBatchWithStagesByIdSpec(
                        batchId
                    )
                );

                if (batch == null)
                    return null;

                // Sum all feed amounts (kg) using repository-level aggregation
                var feedingRepo = _unitOfWork.GetRepository<FeedingLog>();
                var totalFeed = await feedingRepo.SumAsync(
                    fl => fl.Amount,
                    fl => fl.FarmingBatchId == batchId
                );

                // Sum lost weight from mortalities using repository-level aggregation
                var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();
                var totalLostKg = await mortalityRepo.SumAsync(
                    ml => ml.LostWeightKg,
                    ml => ml.BatchId == batchId
                );

                // Determine final biomass: use ActualHarvestWeightKg if harvested and present, else EstimatedHarvestWeightKg
                double finalWeightKg = 0.0;
                if (
                    batch.Status == FarmingBatchStatus.HARVESTED
                    && batch.ActualHarvestWeightKg.HasValue
                )
                {
                    finalWeightKg = batch.ActualHarvestWeightKg.Value;
                }
                else if (batch.EstimatedHarvestWeightKg.HasValue)
                {
                    finalWeightKg = batch.EstimatedHarvestWeightKg.Value;
                }

                // If estimated harvest weight is missing for an active batch, try to compute it now
                if (
                    batch.Status != FarmingBatchStatus.HARVESTED
                    && (
                        !batch.EstimatedHarvestWeightKg.HasValue
                        || batch.EstimatedHarvestWeightKg.Value == 0.0
                    )
                )
                {
                    try
                    {
                        var (estCount, estWeight) = await ComputeEstimatedYieldAsync(
                            batch,
                            persist: true
                        );
                        // reload batch to ensure we have persisted values
                        batch = await batchRepo.GetByIdAsync(batchId);
                        if (batch != null && batch.EstimatedHarvestWeightKg.HasValue)
                        {
                            finalWeightKg = batch.EstimatedHarvestWeightKg.Value;
                        }
                        else if (estWeight.HasValue)
                        {
                            finalWeightKg = estWeight.Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Unable to compute estimated harvest weight for FCR calculation for BatchId {BatchId}",
                            batchId
                        );
                    }
                }

                // Subtract lost weight due to mortality
                finalWeightKg = Math.Max(0.0, finalWeightKg - totalLostKg);

                _logger.LogDebug(
                    "FCR calc for Batch {BatchId}: totalFeed={TotalFeed}, totalLostKg={TotalLostKg}, finalWeightKg={FinalWeightKg}",
                    batchId,
                    totalFeed,
                    totalLostKg,
                    finalWeightKg
                );

                // Compute initial biomass from species' first stage expected weight.
                // Prefer reading expected weight from batch.BatchStages' first sequence entry if available.
                double initialPerFishKg = 0.0;
                var firstStageEntry = batch!.BatchStages?.OrderBy(bs => bs.Sequence).FirstOrDefault();
                if (
                    firstStageEntry?.SpeciesStageConfig != null
                    && firstStageEntry.SpeciesStageConfig.ExpectedWeightKgPerFish.HasValue
                )
                {
                    initialPerFishKg = firstStageEntry
                        .SpeciesStageConfig
                        .ExpectedWeightKgPerFish
                        .Value;
                }
                else
                {
                    // Fallback: try to query species stage configs for the species if available
                    try
                    {
                        var sscRepo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                        Guid speciesId = Guid.Empty;
                        if (firstStageEntry?.SpeciesStageConfig != null)
                            speciesId = firstStageEntry.SpeciesStageConfig.SpeciesId;
                        else if (batch.CurrentStageConfig != null)
                            speciesId = batch.CurrentStageConfig.SpeciesId;

                        if (speciesId != Guid.Empty)
                        {
                            var ordered = (
                                await sscRepo.FindAllAsync(s => s.SpeciesId == speciesId)
                            ).OrderBy(s => s.Sequence);
                            var fs = ordered.FirstOrDefault();
                            if (fs != null && fs.ExpectedWeightKgPerFish.HasValue)
                                initialPerFishKg = fs.ExpectedWeightKgPerFish.Value;
                        }
                    }
                    catch
                    {
                        // ignore and use 0
                    }
                }

                var initialBiomassKg = initialPerFishKg * batch.InitialQuantity;

                var weightGainKg = finalWeightKg - initialBiomassKg;

                _logger.LogDebug(
                    "FCR calc for Batch {BatchId}: initialPerFishKg={InitialPerFishKg}, initialBiomassKg={InitialBiomassKg}, weightGainKg={WeightGainKg}",
                    batchId,
                    initialPerFishKg,
                    initialBiomassKg,
                    weightGainKg
                );

                double? fcr = null;
                if (weightGainKg > 0)
                {
                    fcr = totalFeed / weightGainKg;
                    // Round to 3 decimals for persistence
                    fcr = Math.Round(fcr.Value, 3);
                }

                _logger.LogInformation("Computed FCR for Batch {BatchId}: FCR={Fcr}", batchId, fcr);
                var tracked = await batchRepo.GetByIdAsync(batchId);
                if (tracked != null)
                {
                    tracked.Fcr = fcr;
                    await _unitOfWork.SaveChangesAsync();
                }

                return fcr;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tính FCR cho lô nuôi {BatchId}", batchId);
                return null;
            }
        }
        #endregion

        // Recommendation calculation delegated to IRecommendationCalculator

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
                double cumulativeSurvival = 1.0;
                var baseQuantity = batch.CurrentQuantity;
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
                    // Update cumulative survival using clamped survival rate for this stage
                    var srRaw = ssc?.SurvivalRate ?? 1.0;
                    var sr = srRaw;
                    if (sr < 0)
                        sr = 0;
                    if (sr > 1)
                        sr = 1;
                    cumulativeSurvival *= sr;

                    // Expected count is floor of baseQuantity * cumulativeSurvival
                    var expectedCount = 0;
                    if (baseQuantity > 0 && cumulativeSurvival > 0)
                    {
                        expectedCount = (int)Math.Floor(baseQuantity * cumulativeSurvival);
                    }

                    // Expected total weight (use stage's expected weight per fish if provided)
                    var expectedWeightPerFish = ssc?.ExpectedWeightKgPerFish ?? 0.0;
                    var expectedTotalWeight = Math.Round(expectedCount * expectedWeightPerFish, 3);

                    // Estimated daily feed in kg: (AmountPer100Fish / 100) * expectedCount * FrequencyPerDay
                    var amountPer100 = ssc?.AmountPer100Fish ?? 0.0;
                    var freq = ssc?.FrequencyPerDay ?? 0;
                    var estimatedDailyFeed = Math.Round(
                        (amountPer100 / 100.0) * expectedCount * freq,
                        3
                    );

                    var dto = new PlannedStageDto
                    {
                        Id = bs.Id,
                        Sequence = bs.Sequence,
                        SpeciesStageConfigId = bs.SpeciesStageConfigId,
                        GrowthStageId = ssc?.GrowthStage?.Id ?? ssc?.GrowthStageId ?? Guid.Empty,
                        StageName = ssc?.GrowthStage?.Name ?? string.Empty,
                        EstimatedStartDate = bs.EstimatedStartDate,
                        EstimatedEndDate = bs.EstimatedEndDate,
                        ActualStartDate = bs.ActualStartDate,
                        ActualEndDate = bs.ActualEndDate,
                        FrequencyPerDay = ssc?.FrequencyPerDay ?? 0,
                        FeedTypeNames = ssc?.FeedTypes?.Select(ft => ft.Name).ToList() ?? [],

                        // Calculated fields
                        ExpectedCount = expectedCount,
                        ExpectedTotalWeightKg = expectedTotalWeight,
                        EstimatedDailyFeedKg = estimatedDailyFeed,
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
                // Validate FishTank exists and load it
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var fishTank = await fishTankRepo.GetByIdAsync(createDto.FishTankId);
                if (fishTank == null)
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

                // Check tank capacity vs expected counts per stage using MaxStockingDensity
                // Compute tank volume (assuming units are consistent, e.g., meters -> cubic meters)
                var orderedStageConfigs = stageConfigs.OrderBy(s => s.Sequence).ToList();
                try
                {
                    // First, try to get authoritative recommendation which is computed
                    // as the maximum initial quantity that won't exceed final-stage capacity
                    // at harvest (it already accounts for cumulative survival).
                    var recommended = await _recommendationCalculator.GetRecommendedInitialAsync(
                        fishTank.Id,
                        createDto.SpeciesId
                    );

                    if (recommended.HasValue)
                    {
                        // Enforce maximum allowed initial quantity
                        if (
                            createDto.InitialQuantity > 0
                            && createDto.InitialQuantity > recommended.Value
                        )
                        {
                            return Result<FarmingBatchDto>.Failure(
                                $"Số lượng ban đầu ({createDto.InitialQuantity}) vượt quá mức tối đa cho phép ({recommended.Value}) cho bể này; vui lòng giảm số lượng hoặc chọn bể lớn hơn.",
                                ResultType.BadRequest
                            );
                        }

                        // Also keep the minimum recommended (50% of recommendation) check
                        if (createDto.InitialQuantity > 0)
                        {
                            var minAllowed = (int)Math.Ceiling(recommended.Value * 0.5);
                            if (createDto.InitialQuantity < minAllowed)
                            {
                                return Result<FarmingBatchDto>.Failure(
                                    $"Số lượng ban đầu ({createDto.InitialQuantity}) thấp hơn 50% mức đề nghị ({minAllowed}) cho bể này; vui lòng tăng số lượng để tận dụng bể.",
                                    ResultType.BadRequest
                                );
                            }
                        }
                    }
                    else
                    {
                        // Fallback: if recommendation is not available, validate per-stage capacities
                        var tankVolume =
                            Math.PI * fishTank.Radius * fishTank.Radius * fishTank.Height;
                        if (createDto.InitialQuantity > 0)
                        {
                            double cumulativeSurvival = 1.0;
                            foreach (var sc in orderedStageConfigs)
                            {
                                var sr = sc.SurvivalRate ?? 1.0;
                                if (sr < 0)
                                    sr = 0;
                                if (sr > 1)
                                    sr = 1;
                                cumulativeSurvival *= sr;

                                var expectedAtStage = (int)
                                    Math.Floor(createDto.InitialQuantity * cumulativeSurvival);

                                if (
                                    sc.MaxStockingDensity.HasValue
                                    && sc.MaxStockingDensity.Value > 0
                                    && tankVolume > 0
                                )
                                {
                                    var maxAllowed = (int)
                                        Math.Floor(sc.MaxStockingDensity.Value * tankVolume);
                                    if (expectedAtStage > maxAllowed)
                                    {
                                        var stageName =
                                            sc.GrowthStage?.Name ?? $"Giai đoạn {sc.Sequence}";
                                        return Result<FarmingBatchDto>.Failure(
                                            $"Dự kiến số lượng tại {stageName} ({expectedAtStage}) vượt quá sức chứa tối đa của bể ({maxAllowed}). Vui lòng giảm số lượng ban đầu hoặc chọn bể lớn hơn.",
                                            ResultType.BadRequest
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Không thể kiểm tra sức chứa bể - bỏ qua kiểm tra dung tích"
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

                await WriteCreateAuditLogAsync(farmingBatch, createDto.SpeciesId);
                await _unitOfWork.SaveChangesAsync();
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

                // Minimum recommended check on update (50% of recommended initial based on last stage density)
                if (updateDto.CurrentQuantity.HasValue)
                {
                    try
                    {
                        var stageConfigRepo = _unitOfWork.GetRepository<SpeciesStageConfig>();
                        var currentStageConfig = await stageConfigRepo.GetByIdAsync(
                            farmingBatch.CurrentStageConfigId
                        );
                        var speciesId = currentStageConfig?.SpeciesId ?? Guid.Empty;
                        if (speciesId != Guid.Empty)
                        {
                            var recommended =
                                await _recommendationCalculator.GetRecommendedInitialAsync(
                                    farmingBatch.FishTankId,
                                    speciesId
                                );
                            if (recommended.HasValue)
                            {
                                var minAllowed = (int)Math.Ceiling(recommended.Value * 0.5);
                                if (updateDto.CurrentQuantity.Value < minAllowed)
                                {
                                    return Result.Failure(
                                        $"Số lượng hiện tại ({updateDto.CurrentQuantity.Value}) thấp hơn 50% mức đề nghị ({minAllowed}) cho bể này; vui lòng tăng số lượng hoặc chọn bể khác.",
                                        ResultType.BadRequest
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Không thể thực hiện kiểm tra mức đề nghị khi cập nhật lô nuôi - bỏ qua kiểm tra tối thiểu"
                        );
                    }
                }

                var oldName = farmingBatch.Name;
                var oldCurrentQuantity = farmingBatch.CurrentQuantity;
                var oldUnitOfMeasure = farmingBatch.UnitOfMeasure;

                // Map and update
                _mapper.Map(updateDto, farmingBatch);
                farmingBatchRepo.Update(farmingBatch);
                await _unitOfWork.SaveChangesAsync();

                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();

                if (
                    updateDto.Name != null
                    && !string.Equals(oldName, farmingBatch.Name, StringComparison.Ordinal)
                )
                {
                    oldValues[nameof(FarmingBatch.Name)] = oldName;
                    newValues[nameof(FarmingBatch.Name)] = farmingBatch.Name;
                }

                if (
                    updateDto.CurrentQuantity.HasValue
                    && oldCurrentQuantity != farmingBatch.CurrentQuantity
                )
                {
                    oldValues[nameof(FarmingBatch.CurrentQuantity)] = oldCurrentQuantity;
                    newValues[nameof(FarmingBatch.CurrentQuantity)] = farmingBatch.CurrentQuantity;
                }

                if (
                    updateDto.UnitOfMeasure != null
                    && !string.Equals(oldUnitOfMeasure, farmingBatch.UnitOfMeasure, StringComparison.Ordinal)
                )
                {
                    oldValues[nameof(FarmingBatch.UnitOfMeasure)] = oldUnitOfMeasure;
                    newValues[nameof(FarmingBatch.UnitOfMeasure)] = farmingBatch.UnitOfMeasure;
                }

                if (oldValues.Count > 0 || newValues.Count > 0)
                {
                    await _auditLogService.WriteSemanticAsync(
                        action: AuditLogActions.Update,
                        entityType: AuditLogEntityType.FarmingBatch,
                        entityId: farmingBatch.Id.ToString(),
                        oldValue: oldValues,
                        newValue: newValues
                    );

                    await _unitOfWork.SaveChangesAsync();
                }

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
            bool force = false,
            double? actualHarvestWeightKg = null
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

                if (batch.Status != FarmingBatchStatus.ACTIVE)
                {
                    return Result.Failure(
                        "Chỉ có thể thu hoạch lô nuôi đang hoạt động",
                        ResultType.BadRequest
                    );
                }

                var orderedStages = batch.BatchStages?.OrderBy(s => s.Sequence).ToList() ?? [];
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

                var oldStatus = batch.Status;
                var oldEstimatedHarvestDate = batch.EstimatedHarvestDate;
                var oldActualHarvestDate = batch.ActualHarvestDate;
                var oldActualHarvestWeightKg = batch.ActualHarvestWeightKg;

                batch.Status = FarmingBatchStatus.HARVESTED;
                batch.ActualHarvestDate = harvestDate;
                // If the caller provided an actual harvest weight, set it on the batch so FCR uses it.
                if (actualHarvestWeightKg.HasValue)
                {
                    batch.ActualHarvestWeightKg = actualHarvestWeightKg.Value;
                }
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
                        bs.SpeciesStageConfig = null!;
                    }
                }

                repo.Update(batch);
                await _unitOfWork.SaveChangesAsync();

                await _auditLogService.WriteSemanticAsync(
                    action: AuditLogActions.HarvestBatch,
                    entityType: AuditLogEntityType.FarmingBatch,
                    entityId: batch.Id.ToString(),
                    oldValue: new Dictionary<string, object?>
                    {
                        [nameof(FarmingBatch.Name)] = batch.Name,
                        [nameof(FarmingBatch.Status)] = oldStatus,
                        [nameof(FarmingBatch.EstimatedHarvestDate)] = oldEstimatedHarvestDate,
                        [nameof(FarmingBatch.ActualHarvestDate)] = oldActualHarvestDate,
                        [nameof(FarmingBatch.ActualHarvestWeightKg)] = oldActualHarvestWeightKg,
                    },
                    newValue: new Dictionary<string, object?>
                    {
                        [nameof(FarmingBatch.Name)] = batch.Name,
                        [nameof(FarmingBatch.Status)] = FarmingBatchStatus.HARVESTED,
                        [nameof(FarmingBatch.ActualHarvestDate)] = harvestDate,
                        ["Force"] = force,
                        [nameof(FarmingBatch.ActualHarvestWeightKg)] = actualHarvestWeightKg,
                    }
                );

                await _unitOfWork.SaveChangesAsync();

                try
                {
                    // Recompute and persist FCR after harvest
                    await ComputeAndPersistFcrAsync(batch.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Không thể tính FCR sau khi thu hoạch cho BatchId {BatchId}",
                        batch.Id
                    );
                }

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
        // Hard delete with related-data guards.
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

                await WriteDeleteAuditLogAsync(farmingBatch);
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
        #region Private Helper Methods for Audit Logging

        private async Task WriteCreateAuditLogAsync(FarmingBatch batch, Guid speciesId)
        {
            await _auditLogService.WriteSemanticAsync(
                action: AuditLogActions.Create,
                entityType: AuditLogEntityType.FarmingBatch,
                entityId: batch.Id.ToString(),
                newValue: new Dictionary<string, object?>
                {
                    [nameof(FarmingBatch.Name)] = batch.Name,
                    [nameof(FarmingBatch.FishTankId)] = batch.FishTankId,
                    ["SpeciesId"] = speciesId,
                    [nameof(FarmingBatch.StartDate)] = batch.StartDate,
                    [nameof(FarmingBatch.InitialQuantity)] = batch.InitialQuantity,
                    [nameof(FarmingBatch.UnitOfMeasure)] = batch.UnitOfMeasure,
                }
            );
        }

        private async Task WriteDeleteAuditLogAsync(FarmingBatch batch)
        {
            await _auditLogService.WriteSemanticAsync(
                action: AuditLogActions.Delete,
                entityType: AuditLogEntityType.FarmingBatch,
                entityId: batch.Id.ToString(),
                oldValue: new Dictionary<string, object?>
                {
                    [nameof(FarmingBatch.Name)] = batch.Name,
                    [nameof(FarmingBatch.FishTankId)] = batch.FishTankId,
                    [nameof(FarmingBatch.Status)] = batch.Status,
                    [nameof(FarmingBatch.StartDate)] = batch.StartDate,
                    [nameof(FarmingBatch.InitialQuantity)] = batch.InitialQuantity,
                    [nameof(FarmingBatch.CurrentQuantity)] = batch.CurrentQuantity,
                    [nameof(FarmingBatch.UnitOfMeasure)] = batch.UnitOfMeasure,
                    [nameof(FarmingBatch.EstimatedHarvestDate)] = batch.EstimatedHarvestDate,
                },
                newValue: null
            );
        }
        #endregion
    }
}
