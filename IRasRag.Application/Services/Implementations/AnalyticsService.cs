using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.AnalyticsSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AnalyticsService> _logger;

        // ── Fixed metric keys ────────────────────────────────────────────────
        private static readonly HashSet<string> _fixedMetrics = new(StringComparer.OrdinalIgnoreCase)
        {
            "survival_rate",
            "mortality",
            "feeding",
            "alerts",
        };

        public AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Compare Batches
        public async Task<Result<BatchCompareResponseDto>> CompareBatchesAsync(
            BatchCompareRequest request
        )
        {
            try
            {
                _logger.LogInformation(
                    "Batch comparison requested for {Count} batch(es). Metrics: [{Metrics}]",
                    request.BatchIds?.Count ?? 0,
                    string.Join(", ", request.Metrics ?? [])
                );

                // ── 1. Validate input ────────────────────────────────────────
                if (request.BatchIds == null || request.BatchIds.Count == 0)
                {
                    _logger.LogWarning(
                        "CompareBatches: User {UserId} sent request with no batch IDs.",
                        request.UserId
                    );
                    return Result<BatchCompareResponseDto>.Failure(
                        "Vui lòng cung cấp ít nhất hai lô nuôi để so sánh.",
                        ResultType.BadRequest
                    );
                }

                var distinctBatchIds = request.BatchIds.Distinct().ToList();

                if (distinctBatchIds.Count < 2)
                {
                    _logger.LogWarning(
                        "CompareBatches: User {UserId} provided only {Count} distinct batch ID(s), minimum 2 required.",
                        request.UserId,
                        distinctBatchIds.Count
                    );
                    return Result<BatchCompareResponseDto>.Failure(
                        "Cần ít nhất hai lô nuôi khác nhau để thực hiện so sánh.",
                        ResultType.BadRequest
                    );
                }

                if (distinctBatchIds.Count > 10)
                {
                    _logger.LogWarning(
                        "CompareBatches: User {UserId} requested {Count} batches, exceeds limit of 10.",
                        request.UserId,
                        distinctBatchIds.Count
                    );
                    return Result<BatchCompareResponseDto>.Failure(
                        "Tối đa 10 lô nuôi có thể được so sánh trong một lần.",
                        ResultType.BadRequest
                    );
                }

                // ── 2. Normalise requested metrics ───────────────────────────
                var requestedMetrics = (request.Metrics ?? new List<string>())
                    .Select(m => m.Trim())
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                bool allMetrics = requestedMetrics.Count == 0;

                bool computeSurvivalRate =
                    allMetrics || requestedMetrics.Contains("survival_rate");
                bool computeMortality = allMetrics || requestedMetrics.Contains("mortality");
                bool computeFeeding = allMetrics || requestedMetrics.Contains("feeding");
                bool computeAlerts = allMetrics || requestedMetrics.Contains("alerts");

                // Sensor-type metrics are any requested entries not in the fixed set
                var sensorMetricFilter = allMetrics
                    ? null
                    : requestedMetrics
                        .Where(m => !_fixedMetrics.Contains(m))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                bool needSensor =
                    allMetrics || (sensorMetricFilter != null && sensorMetricFilter.Count > 0);

                // ── 3. Repositories ──────────────────────────────────────────
                var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var alertRepo = _unitOfWork.GetRepository<Alert>();
                var mortalityLogRepo = _unitOfWork.GetRepository<MortalityLog>();
                var feedingLogRepo = _unitOfWork.GetRepository<FeedingLog>();
                var masterBoardRepo = _unitOfWork.GetRepository<MasterBoard>();
                var sensorRepo = _unitOfWork.GetRepository<Sensor>();
                var sensorLogRepo = _unitOfWork.GetRepository<SensorLog>();
                var sensorTypeRepo = _unitOfWork.GetRepository<SensorType>();

                // ── 3a. Resolve user's allowed tanks ─────────────────────────
                var userTankIds = await GetUserTankIdsAsync(request.UserId);

                // ── 4. Load all sensor types once ────────────────────────────
                var allSensorTypes = await sensorTypeRepo.GetAllAsync();
                var sensorTypeById = allSensorTypes.ToDictionary(st => st.Id);

                // ── 5. Process each batch ────────────────────────────────────

                // Batch-load all requested batches and their fish tanks in two queries
                // to avoid N+1 round-trips when comparing multiple batches.
                var allBatches = (await batchRepo.FindAllAsync(b => distinctBatchIds.Contains(b.Id)))
                    .ToDictionary(b => b.Id);
                var fishTankIds = allBatches.Values.Select(b => b.FishTankId).Distinct().ToList();
                var allFishTanks = (await fishTankRepo.FindAllAsync(ft => fishTankIds.Contains(ft.Id)))
                    .ToDictionary(ft => ft.Id);

                // ── 5a. Ownership check – reject batches the user cannot access ─
                var unauthorizedBatchIds = distinctBatchIds
                    .Where(id => allBatches.TryGetValue(id, out var b) && !userTankIds.Contains(b.FishTankId))
                    .ToList();

                if (unauthorizedBatchIds.Count > 0)
                {
                    _logger.LogWarning(
                        "CompareBatches: User {UserId} attempted to access unauthorized batch(es): [{Ids}].",
                        request.UserId,
                        string.Join(", ", unauthorizedBatchIds)
                    );
                    return Result<BatchCompareResponseDto>.Failure(
                        "Bạn không có quyền truy cập vào một hoặc nhiều lô nuôi đã chọn.",
                        ResultType.Unauthorized
                    );
                }

                var batchResults = new List<BatchMetricsDto>(distinctBatchIds.Count);

                foreach (var batchId in distinctBatchIds)
                {
                    allBatches.TryGetValue(batchId, out var batch);
                    if (batch == null)
                    {
                        _logger.LogWarning(
                            "Batch {BatchId} not found or has been deleted – skipped.",
                            batchId
                        );
                        continue;
                    }

                    allFishTanks.TryGetValue(batch.FishTankId, out var fishTank);

                    var batchFrom = DateTime.SpecifyKind(batch.StartDate, DateTimeKind.Utc);
                    var batchTo = batch.ActualHarvestDate.HasValue
                        ? DateTime.SpecifyKind(batch.ActualHarvestDate.Value, DateTimeKind.Utc)
                        : DateTime.UtcNow;

                    var metricValues = new BatchMetricValuesDto();

                    // ── 5a. Survival rate ──────────────────────────────────
                    if (computeSurvivalRate)
                    {
                        metricValues.SurvivalRate =
                            batch.InitialQuantity > 0
                                ? Math.Round(
                                    (double)batch.CurrentQuantity / batch.InitialQuantity * 100,
                                    2
                                )
                                : 0d;
                    }

                    // ── 5b. Total mortality ────────────────────────────────
                    // NOTE: EF Core DbContext is not thread-safe – queries must be sequential.
                    if (computeMortality)
                    {
                        var mortalityLogs = await mortalityLogRepo.FindAllAsync(ml =>
                            ml.BatchId == batchId
                        );
                        metricValues.TotalMortality = mortalityLogs.Sum(ml => (double)ml.Quantity);
                    }

                    // ── 5c. Total feeding ──────────────────────────────────
                    if (computeFeeding)
                    {
                        var feedingLogs = await feedingLogRepo.FindAllAsync(fl =>
                            fl.FarmingBatchId == batchId
                        );
                        metricValues.TotalFeeding = Math.Round(
                            feedingLogs.Sum(fl => (double)fl.Amount),
                            2
                        );
                    }

                    // ── 5d. Alerts ─────────────────────────────────────────
                    if (computeAlerts)
                    {
                        var alertList = (
                            await alertRepo.FindAllAsync(a =>
                                a.FishTankId == batch.FishTankId
                                && a.RaisedAt >= batchFrom
                                && a.RaisedAt <= batchTo
                            )
                        ).ToList();

                        metricValues.TotalAlerts = alertList.Count;

                        var orphanedAlerts = alertList.Count(a => !sensorTypeById.ContainsKey(a.SensorTypeId));
                        if (orphanedAlerts > 0)
                            _logger.LogWarning(
                                "Batch {BatchId}: {Count} alert(s) reference an unknown SensorTypeId and are excluded from AlertsByType.",
                                batchId,
                                orphanedAlerts
                            );

                        metricValues.AlertsByType = alertList
                            .Where(a => sensorTypeById.ContainsKey(a.SensorTypeId))
                            .GroupBy(a => sensorTypeById[a.SensorTypeId].Name)
                            .ToDictionary(g => g.Key, g => g.Count());
                    }

                    // ── 5e. Sensor averages ────────────────────────────────
                    if (needSensor)
                    {
                        var masterBoards = await masterBoardRepo.FindAllAsync(mb =>
                            mb.FishTankId == batch.FishTankId
                        );
                        var masterBoardIds = masterBoards.Select(mb => mb.Id).ToList();

                        if (masterBoardIds.Count > 0)
                        {
                            var sensors = await sensorRepo.FindAllAsync(s =>
                                masterBoardIds.Contains(s.MasterBoardId)
                            );

                            // Group sensors by SensorTypeId
                            var groupsBySensorType = sensors
                                .Where(s => sensorTypeById.ContainsKey(s.SensorTypeId))
                                .GroupBy(s => s.SensorTypeId)
                                .ToList();

                            foreach (var group in groupsBySensorType)
                            {
                                var sensorType = sensorTypeById[group.Key];
                                var typeName = sensorType.Name;

                                // Apply sensor metric filter when specific metrics were requested.
                                // Exact case-insensitive match on SensorType.Name or MeasureType first;
                                // only then fall back to a prefix/substring match.
                                if (sensorMetricFilter != null && sensorMetricFilter.Count > 0)
                                {
                                    bool matched =
                                        sensorMetricFilter.Contains(typeName)
                                        || sensorMetricFilter.Contains(sensorType.MeasureType)
                                        || sensorMetricFilter.Any(f =>
                                            typeName.StartsWith(f, StringComparison.OrdinalIgnoreCase)
                                            || sensorType.MeasureType.StartsWith(
                                                f,
                                                StringComparison.OrdinalIgnoreCase
                                            )
                                        );

                                    if (!matched)
                                        continue;
                                }

                                // Use List<Guid> – EF Core translates IList<T>.Contains reliably
                                var sensorIds = group.Select(s => s.Id).ToList();

                                var logs = await sensorLogRepo.FindAllAsync(sl =>
                                    sensorIds.Contains(sl.SensorId)
                                    && sl.CreatedAt != null
                                    && sl.CreatedAt.Value >= batchFrom
                                    && sl.CreatedAt.Value <= batchTo
                                );

                                var logList = logs.ToList();
                                if (logList.Count > 0)
                                {
                                    metricValues.SensorAverages[typeName] = Math.Round(
                                        logList.Average(l => l.Data),
                                        4
                                    );
                                }
                            }
                        }
                    }

                    batchResults.Add(
                        new BatchMetricsDto
                        {
                            BatchId = batchId,
                            BatchName = batch.Name,
                            FishTankId = batch.FishTankId,
                            FishTankName = fishTank?.Name ?? string.Empty,
                            Status = batch.Status.ToString(),
                            StartDate = batch.StartDate,
                            EstimatedHarvestDate = batch.EstimatedHarvestDate,
                            ActualHarvestDate = batch.ActualHarvestDate,
                            MetricValues = metricValues,
                        }
                    );
                }

                // ── 6. Guard: no valid batch was found ───────────────────────
                if (batchResults.Count == 0)
                    return Result<BatchCompareResponseDto>.Failure(
                        "Không tìm thấy lô nuôi nào hợp lệ trong danh sách đã cung cấp.",
                        ResultType.NotFound
                    );

                // ── 7. Build evaluated-metrics list ──────────────────────────
                var evaluatedMetrics = new List<string>();
                if (computeSurvivalRate)
                    evaluatedMetrics.Add("survival_rate");
                if (computeMortality)
                    evaluatedMetrics.Add("mortality");
                if (computeFeeding)
                    evaluatedMetrics.Add("feeding");
                if (computeAlerts)
                    evaluatedMetrics.Add("alerts");

                var sensorKeys = batchResults
                    .SelectMany(b => b.MetricValues.SensorAverages.Keys)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(k => k);

                evaluatedMetrics.AddRange(sensorKeys);

                var response = new BatchCompareResponseDto
                {
                    ComparedAt = DateTime.UtcNow,
                    EvaluatedMetrics = evaluatedMetrics,
                    Batches = batchResults,
                };

                _logger.LogInformation(
                    "Batch comparison completed. {BatchCount} batch(es) processed.",
                    batchResults.Count
                );

                return Result<BatchCompareResponseDto>.Success(
                    response,
                    "So sánh lô nuôi thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while comparing batches.");
                return Result<BatchCompareResponseDto>.Failure(
                    "Có lỗi xảy ra khi so sánh lô nuôi, vui lòng thử lại sau.",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        #region Alert Frequency
        public async Task<Result<AlertFrequencyResponseDto>> GetAlertFrequencyAsync(
            AlertFrequencyRequest request
        )
        {
            try
            {
                var to = request.To?.ToUniversalTime() ?? DateTime.UtcNow;
                var from = request.From?.ToUniversalTime() ?? to.AddDays(-30);

                if (from >= to)
                {
                    _logger.LogWarning(
                        "GetAlertFrequency: User {UserId} provided invalid date range: From={From} is not before To={To}.",
                        request.UserId,
                        from,
                        to
                    );
                    return Result<AlertFrequencyResponseDto>.Failure(
                        "Thời điểm bắt đầu phải trước thời điểm kết thúc.",
                        ResultType.BadRequest
                    );
                }

                if ((to - from).TotalDays > 365)
                {
                    _logger.LogWarning(
                        "GetAlertFrequency: User {UserId} requested {Days} days of data, exceeds 365-day limit.",
                        request.UserId,
                        (int)(to - from).TotalDays
                    );
                    return Result<AlertFrequencyResponseDto>.Failure(
                        "Khoảng thời gian thống kê tối đa là 365 ngày.",
                        ResultType.BadRequest
                    );
                }

                _logger.LogInformation(
                    "Alert frequency requested. From={From}, To={To}, FishTankId={TankId}, FarmId={FarmId}, TopN={TopN}",
                    from,
                    to,
                    request.FishTankId,
                    request.FarmId,
                    request.TopN
                );

                // ── 1. Resolve user's allowed tanks ─────────────────────────
                var userTankIds = await GetUserTankIdsAsync(request.UserId);

                // ── 2. Validate optional filter IDs – ownership check ────────
                if (request.FishTankId.HasValue)
                {
                    if (!userTankIds.Contains(request.FishTankId.Value))
                    {
                        _logger.LogWarning(
                            "GetAlertFrequency: User {UserId} attempted access to unauthorized FishTank {FishTankId}.",
                            request.UserId,
                            request.FishTankId.Value
                        );
                        return Result<AlertFrequencyResponseDto>.Failure(
                            "Bạn không có quyền truy cập bể nuôi với ID đã cung cấp.",
                            ResultType.Unauthorized
                        );
                    }
                }

                if (request.FarmId.HasValue)
                {
                    var hasAccess = await _unitOfWork
                        .GetRepository<UserFarm>()
                        .AnyAsync(uf => uf.UserId == request.UserId && uf.FarmId == request.FarmId.Value);
                    if (!hasAccess)
                    {
                        _logger.LogWarning(
                            "GetAlertFrequency: User {UserId} attempted access to unauthorized Farm {FarmId}.",
                            request.UserId,
                            request.FarmId.Value
                        );
                        return Result<AlertFrequencyResponseDto>.Failure(
                            "Bạn không có quyền truy cập trang trại với ID đã cung cấp.",
                            ResultType.Unauthorized
                        );
                    }
                }

                // ── 3. Load projected alerts (scoped to user's tanks) ────────
                var alertRepo = _unitOfWork.GetRepository<Alert>();
                var spec = new AlertFrequencySpec(from, to, userTankIds, request.FishTankId, request.FarmId);
                var projections = (await alertRepo.ListAsync(spec)).ToList();

                int totalAlerts = projections.Count;

                // ── 4. Load sensor type metadata ─────────────────────────────
                var sensorTypeRepo = _unitOfWork.GetRepository<SensorType>();
                var allSensorTypes = await sensorTypeRepo.GetAllAsync();
                var sensorTypeById = allSensorTypes.ToDictionary(st => st.Id);

                // ── 5. Group by SensorTypeId and build per-type stats ─────────────
                var byType = projections
                    .GroupBy(p => p.SensorTypeId)
                    .Select(g =>
                    {
                        var items = g.ToList();
                        sensorTypeById.TryGetValue(g.Key, out var st);

                        // Single-pass status count
                        int openCount = 0, ackCount = 0, resolvedCount = 0, dismissedCount = 0;
                        var resolvedItems = new List<AlertFrequencyProjection>();

                        foreach (var item in items)
                        {
                            switch (item.Status)
                            {
                                case AlertStatus.OPEN:         openCount++;         break;
                                case AlertStatus.ACKNOWLEDGED: ackCount++;          break;
                                case AlertStatus.RESOLVED:     resolvedCount++;     break;
                                case AlertStatus.DISMISSED:    dismissedCount++;    break;
                            }
                            if (item.Status == AlertStatus.RESOLVED && item.ResolvedAt.HasValue)
                                resolvedItems.Add(item);
                        }

                        // Average resolution time (minutes) for RESOLVED alerts only
                        double? avgResolutionMinutes = resolvedItems.Count > 0
                            ? Math.Round(
                                resolvedItems.Average(i =>
                                    (i.ResolvedAt!.Value - i.RaisedAt).TotalMinutes
                                ),
                                2
                            )
                            : null;

                        // Top tanks with most alerts of this type (top 5)
                        var topTanks = items
                            .GroupBy(i => new { i.FishTankId, i.FishTankName })
                            .Select(tg => new TankAlertCountDto
                            {
                                FishTankId = tg.Key.FishTankId,
                                FishTankName = tg.Key.FishTankName,
                                Count = tg.Count(),
                            })
                            .OrderByDescending(t => t.Count)
                            .Take(5)
                            .ToList();

                        double percentage =
                            totalAlerts > 0
                                ? Math.Round((double)items.Count / totalAlerts * 100, 2)
                                : 0d;

                        return new AlertFrequencyItemDto
                        {
                            SensorTypeId = g.Key,
                            SensorTypeName = st?.Name ?? g.Key.ToString(),
                            MeasureType = st?.MeasureType ?? string.Empty,
                            UnitOfMeasure = st?.UnitOfMeasure ?? string.Empty,
                            TotalCount = items.Count,
                            Percentage = percentage,
                            OpenCount = openCount,
                            AcknowledgedCount = ackCount,
                            ResolvedCount = resolvedCount,
                            DismissedCount = dismissedCount,
                            AverageResolutionMinutes = avgResolutionMinutes,
                            TopTanks = topTanks,
                        };
                    })
                    .OrderByDescending(x => x.TotalCount)
                    .Take(request.TopN)
                    .ToList();

                // ── 6. Daily trend ─────────────────────────────────────────────────
                var dailyTrend = projections
                    .GroupBy(p => p.RaisedAt.Date)
                    .Select(g => new DailyAlertTrendDto
                    {
                        Date = g.Key,
                        Count = g.Count(),
                    })
                    .OrderBy(d => d.Date)
                    .ToList();

                // Fill in days with zero alerts so the chart has a continuous x-axis
                var allDays = Enumerable
                    .Range(0, (int)(to.Date - from.Date).TotalDays + 1)
                    .Select(offset => from.Date.AddDays(offset))
                    .ToList();

                var trendLookup = dailyTrend.ToDictionary(d => d.Date);
                var fullTrend = allDays
                    .Select(day =>
                        trendLookup.TryGetValue(day, out var entry)
                            ? entry
                            : new DailyAlertTrendDto { Date = day, Count = 0 }
                    )
                    .ToList();

                var response = new AlertFrequencyResponseDto
                {
                    From = from,
                    To = to,
                    TotalAlerts = totalAlerts,
                    ByAlertType = byType,
                    DailyTrend = fullTrend,
                };

                _logger.LogInformation(
                    "Alert frequency computed. TotalAlerts={Total}, UniqueTypes={Types}, From={From}, To={To}",
                    totalAlerts,
                    byType.Count,
                    from,
                    to
                );

                return Result<AlertFrequencyResponseDto>.Success(
                    response,
                    "Thống kê tần suất cảnh báo thành công."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while computing alert frequency.");
                return Result<AlertFrequencyResponseDto>.Failure(
                    "Có lỗi xảy ra khi thống kê tần suất cảnh báo, vui lòng thử lại sau.",
                    ResultType.Unexpected
                );
            }
        }
        #endregion

        // ── User Scope Helper ─────────────────────────────────────────────────

        /// <summary>
        /// Trả về tập hợp FishTankId mà user được phép truy cập.
        /// Đường đi: User → UserFarm → Farm → FishTank
        /// </summary>
        private async Task<HashSet<Guid>> GetUserTankIdsAsync(Guid userId)
        {
            var userFarmRepo = _unitOfWork.GetRepository<UserFarm>();
            var farmIds = (await userFarmRepo.FindAllAsync(uf => uf.UserId == userId))
                .Select(uf => uf.FarmId)
                .ToHashSet();

            if (farmIds.Count == 0)
                return new HashSet<Guid>();

            var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
            return (await fishTankRepo.FindAllAsync(t => farmIds.Contains(t.FarmId)))
                .Select(t => t.Id)
                .ToHashSet();
        }
    }
}
