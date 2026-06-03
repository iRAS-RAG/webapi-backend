using AutoMapper;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs.Metrics;
using IRasRag.Application.Services.Interfaces;
using IRasRag.Application.Specifications.Metrics;
using IRasRag.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IRasRag.Application.Services.Implementations
{
    public class SupervisorMetricsService : ISupervisorMetricsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupervisorMetricsService> _logger;
        private readonly IMapper _mapper;

        public SupervisorMetricsService(
            IUnitOfWork unitOfWork,
            ILogger<SupervisorMetricsService> logger,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<FarmSummaryDto>> GetFarmSummaryAsync(
            Guid farmId,
            DateTime? start,
            DateTime? end,
            string groupBy
        )
        {
            try
            {
                // default to last 30 days if not provided
                var startDt = start ?? DateTime.UtcNow.AddDays(-30);
                var endDt = end ?? DateTime.UtcNow;
                var farm = await _unitOfWork.GetRepository<Farm>().GetByIdAsync(farmId);
                if (farm == null)
                    return Result<FarmSummaryDto>.Failure("Farm not found", ResultType.NotFound);

                // Use specs for efficient queries
                var feedingRepo = _unitOfWork.GetRepository<FeedingLog>();
                var feedingLogs = await feedingRepo.ListAsync(
                    new FeedingLogByFarmAndRangeSpec(farmId, startDt, endDt)
                );
                var totalFeed = feedingLogs.Sum(fl => fl.Amount);
                _logger.LogInformation(
                    "SupervisorMetrics: feedingLogs found={Count} totalFeed={TotalFeed} for farm={FarmId} start={Start} end={End}",
                    feedingLogs?.Count ?? 0,
                    totalFeed,
                    farmId,
                    startDt,
                    endDt
                );

                // Fallback: if spec returned nothing, try repository-level aggregation per batch (handles DB shapes where navigation includes fail)
                if (totalFeed == 0 && (feedingLogs == null || feedingLogs.Count == 0))
                {
                    // compute per-batch sums below when building feedByBatch; totalFeed will be sum of those
                }

                var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();
                var mortalityLogs = await mortalityRepo.ListAsync(
                    new MortalityLogByFarmAndRangeSpec(farmId, startDt, endDt)
                );
                var totalDeaths = mortalityLogs.Sum(ml => ml.Quantity);
                var totalDeadKg = mortalityLogs.Sum(ml => ml.LostWeightKg);
                _logger.LogInformation(
                    "SupervisorMetrics: mortalityLogs found={Count} totalDeaths={TotalDeaths} totalDeadKg={TotalDeadKg} for farm={FarmId} start={Start} end={End}",
                    mortalityLogs?.Count ?? 0,
                    totalDeaths,
                    totalDeadKg,
                    farmId,
                    startDt,
                    endDt
                );

                var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var batches = await batchRepo.FindAllAsync(b => b.FishTank.FarmId == farmId);

                // Build per-batch feed/death aggregations
                var feedByBatch =
                    (feedingLogs != null && feedingLogs.Count > 0)
                        ? feedingLogs
                            .GroupBy(fl => fl.FarmingBatchId)
                            .ToDictionary(g => g.Key, g => g.Sum(fl => fl.Amount))
                        : new Dictionary<Guid, double>();
                var deathsByBatchCount =
                    (mortalityLogs != null && mortalityLogs.Count > 0)
                        ? mortalityLogs
                            .GroupBy(ml => ml.BatchId)
                            .ToDictionary(g => g.Key, g => g.Sum(ml => ml.Quantity))
                        : new Dictionary<Guid, int>();
                var deathsByBatchWeight =
                    (mortalityLogs != null && mortalityLogs.Count > 0)
                        ? mortalityLogs
                            .GroupBy(ml => ml.BatchId)
                            .ToDictionary(g => g.Key, g => g.Sum(ml => ml.LostWeightKg))
                        : new Dictionary<Guid, double>();

                // Mortality fallback per batch
                if (deathsByBatchCount.Count == 0 && deathsByBatchWeight.Count == 0)
                {
                    foreach (var b in batches)
                    {
                        var count = await mortalityRepo.CountAsync(ml =>
                            ml.BatchId == b.Id
                            && (
                                (ml.Date >= startDt && ml.Date <= endDt)
                                || (ml.CreatedAt >= startDt && ml.CreatedAt <= endDt)
                            )
                        );
                        var weight = await mortalityRepo.SumAsync(
                            ml => ml.LostWeightKg,
                            ml =>
                                ml.BatchId == b.Id
                                && (
                                    (ml.Date >= startDt && ml.Date <= endDt)
                                    || (ml.CreatedAt >= startDt && ml.CreatedAt <= endDt)
                                )
                        );
                        if (count > 0)
                            deathsByBatchCount[b.Id] = count;
                        if (weight > 0)
                            deathsByBatchWeight[b.Id] = weight;
                    }
                    totalDeaths = deathsByBatchCount.Values.Sum();
                    totalDeadKg = deathsByBatchWeight.Values.Sum();
                }

                // Load fish tanks for name lookups
                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var fishTanks = await fishTankRepo.FindAllAsync(ft => ft.FarmId == farmId);
                var tankNameById = fishTanks.ToDictionary(
                    ft => ft.Id,
                    ft => ft.Name ?? string.Empty
                );

                // If feedByBatch is empty, compute using repository SumAsync per batch (fallback)
                if (feedByBatch.Count == 0)
                {
                    foreach (var b in batches)
                    {
                        var f = await feedingRepo.SumAsync(
                            fl => fl.Amount,
                            fl =>
                                fl.FarmingBatchId == b.Id
                                && (
                                    (fl.CreatedDate >= startDt && fl.CreatedDate <= endDt)
                                    || (fl.CreatedAt >= startDt && fl.CreatedAt <= endDt)
                                )
                        );
                        if (f > 0)
                            feedByBatch[b.Id] = f;
                        _logger.LogInformation(
                            "SupervisorMetrics: batch={BatchId} fallbackFeed={Feed}",
                            b.Id,
                            f
                        );
                    }
                    totalFeed = Math.Round(feedByBatch.Values.Sum(), 3);
                }

                // compute harvest totals from batch records
                var totalHarvestedBatches = batches.Count(b =>
                    b.ActualHarvestWeightKg.HasValue && b.ActualHarvestWeightKg.Value > 0
                );
                var totalHarvestWeightKg = Math.Round(
                    batches.Sum(b => b.ActualHarvestWeightKg ?? 0.0),
                    3
                );
                double? farmFcr = null;
                if (totalHarvestWeightKg > 0)
                {
                    // farm FCR = total feed (kg) / total harvested weight (kg)
                    farmFcr = Math.Round(totalFeed / totalHarvestWeightKg, 3);
                }

                var summary = new FarmSummaryDto
                {
                    FarmId = farmId,
                    TotalFeedKg = Math.Round(totalFeed, 3),
                    TotalDeathsCount = totalDeaths,
                    TotalDeadWeightKg = Math.Round(totalDeadKg, 3),
                    TotalInitialQuantity = batches.Sum(b => b.InitialQuantity),
                    TotalCurrentQuantity = batches.Sum(b => b.CurrentQuantity),
                    TotalHarvestedBatches = totalHarvestedBatches,
                    TotalHarvestWeightKg = totalHarvestWeightKg,
                    Fcr = farmFcr,
                    Batches = batches
                        .Select(b => new BatchSummaryDto
                        {
                            BatchId = b.Id,
                            BatchName = b.Name,
                            FishTankId = b.FishTankId,
                            FishTankName = tankNameById.TryGetValue(b.FishTankId, out var tn)
                                ? tn
                                : string.Empty,
                            InitialQuantity = b.InitialQuantity,
                            CurrentQuantity = b.CurrentQuantity,
                            TotalFeedKg = Math.Round(
                                feedByBatch.TryGetValue(b.Id, out var f) ? f : 0.0,
                                3
                            ),
                            TotalDeaths = (int)(
                                deathsByBatchCount.TryGetValue(b.Id, out var dc) ? dc : 0
                            ),
                            TotalDeadWeightKg = Math.Round(
                                deathsByBatchWeight.TryGetValue(b.Id, out var dw) ? dw : 0.0,
                                3
                            ),
                            Fcr =
                                b.Fcr
                                ?? (
                                    (
                                        feedByBatch.TryGetValue(b.Id, out var fb)
                                        && (b.ActualHarvestWeightKg ?? 0) > 0
                                    )
                                        ? Math.Round(fb / (b.ActualHarvestWeightKg ?? 0.0), 3)
                                        : b.Fcr
                                ),
                        })
                        .ToList(),
                };

                return Result<FarmSummaryDto>.Success(summary, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting farm summary for {FarmId}", farmId);
                return Result<FarmSummaryDto>.Failure("Error", ResultType.Unexpected);
            }
        }

        public async Task<Result<TimeSeriesResponseDto>> GetFarmTimeSeriesAsync(
            Guid farmId,
            DateTime? start,
            DateTime? end,
            string metric,
            string interval,
            string groupBy,
            string[] aggregations
        )
        {
            try
            {
                var startDt = start ?? DateTime.UtcNow.AddDays(-30);
                var endDt = end ?? DateTime.UtcNow;
                interval = interval?.ToLower() ?? "day";
                groupBy = groupBy?.ToLower() ?? "none";
                metric = metric?.ToLower() ?? "feed";

                var resp = new TimeSeriesResponseDto { Metric = metric };

                if (metric == "feed")
                {
                    var feedingRepo = _unitOfWork.GetRepository<FeedingLog>();
                    var logs = (
                        await feedingRepo.ListAsync(
                            new FeedingLogByFarmAndRangeSpec(farmId, startDt, endDt)
                        )
                    )
                        .Select(fl => new
                        {
                            fl.Amount,
                            Time = fl.CreatedDate,
                            BatchId = fl.FarmingBatchId,
                            BatchName = fl.FarmingBatch?.Name ?? string.Empty,
                            TankId = fl.FarmingBatch?.FishTankId ?? Guid.Empty,
                            TankName = fl.FarmingBatch?.FishTank?.Name ?? string.Empty,
                        })
                        .ToList();

                    // bucket by interval
                    Func<DateTime, DateTime> bucket = dt => dt.Date;
                    if (interval == "hour")
                        bucket = dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

                    var grouped = logs.GroupBy(l => new
                    {
                        Bucket = bucket(l.Time),
                        GroupKey = groupBy == "batch" ? (l.BatchId.ToString())
                        : groupBy == "tank" ? l.TankId.ToString()
                        : "all",
                        GroupName = groupBy == "batch" ? l.BatchName
                        : groupBy == "tank" ? l.TankName
                        : "Nông trại",
                    });

                    foreach (var g in grouped)
                    {
                        var values = g.Select(x => x.Amount).ToList();
                        foreach (var agg in aggregations)
                        {
                            var series = resp.Series.FirstOrDefault(s =>
                                s.GroupId == g.Key.GroupKey + "|" + agg
                            );
                            if (series == null)
                            {
                                series = new TimeSeriesSeriesDto
                                {
                                    GroupId = g.Key.GroupKey + "|" + agg,
                                    GroupName = g.Key.GroupName + " (" + agg + ")",
                                };
                                resp.Series.Add(series);
                            }

                            double value = 0;
                            switch (agg.ToLower())
                            {
                                case "sum":
                                    value = values.Sum();
                                    break;
                                case "avg":
                                    value = values.Count != 0 ? values.Average() : 0;
                                    break;
                                case "min":
                                    value = values.Count != 0 ? values.Min() : 0;
                                    break;
                                case "max":
                                    value = values.Count != 0 ? values.Max() : 0;
                                    break;
                                case "median":
                                    value = values.Count != 0 ? Percentile(values, 0.5) : 0;
                                    break;
                                case "p90":
                                    value = values.Count != 0 ? Percentile(values, 0.9) : 0;
                                    break;
                                default:
                                    value = values.Sum();
                                    break;
                            }

                            series.Points.Add(
                                new TimeSeriesPointDto
                                {
                                    Timestamp = g.Key.Bucket,
                                    Value = Math.Round(value, 3),
                                }
                            );
                        }
                    }

                    // sort points per series
                    foreach (var s in resp.Series)
                        s.Points = s.Points.OrderBy(p => p.Timestamp).ToList();

                    return Result<TimeSeriesResponseDto>.Success(resp, "Ok");
                }

                if (metric == "mortality")
                {
                    var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();
                    var logs = (
                        await mortalityRepo.ListAsync(
                            new MortalityLogByFarmAndRangeSpec(farmId, startDt, endDt)
                        )
                    )
                        .Select(ml => new
                        {
                            Value = ml.LostWeightKg,
                            Time = ml.Date,
                            BatchId = ml.BatchId,
                            BatchName = ml.Batch?.Name ?? string.Empty,
                            TankId = ml.Batch?.FishTankId ?? Guid.Empty,
                            TankName = ml.Batch?.FishTank?.Name ?? string.Empty,
                        })
                        .ToList();

                    Func<DateTime, DateTime> bucket = dt => dt.Date;
                    if (interval == "hour")
                        bucket = dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

                    var grouped = logs.GroupBy(l => new
                    {
                        Bucket = bucket(l.Time),
                        GroupKey = groupBy == "batch" ? (l.BatchId.ToString())
                        : groupBy == "tank" ? l.TankId.ToString()
                        : "all",
                        GroupName = groupBy == "batch" ? l.BatchName
                        : groupBy == "tank" ? l.TankName
                        : "Nông trại",
                    });

                    foreach (var g in grouped)
                    {
                        var values = g.Select(x => x.Value).ToList();
                        foreach (var agg in aggregations)
                        {
                            var series = resp.Series.FirstOrDefault(s =>
                                s.GroupId == g.Key.GroupKey + "|" + agg
                            );
                            if (series == null)
                            {
                                series = new TimeSeriesSeriesDto
                                {
                                    GroupId = g.Key.GroupKey + "|" + agg,
                                    GroupName = g.Key.GroupName + " (" + agg + ")",
                                };
                                resp.Series.Add(series);
                            }

                            double value = 0;
                            switch (agg.ToLower())
                            {
                                case "sum":
                                    value = values.Sum();
                                    break;
                                case "avg":
                                    value = values.Count != 0 ? values.Average() : 0;
                                    break;
                                case "min":
                                    value = values.Count != 0 ? values.Min() : 0;
                                    break;
                                case "max":
                                    value = values.Count != 0 ? values.Max() : 0;
                                    break;
                                case "median":
                                    value = values.Count != 0 ? Percentile(values, 0.5) : 0;
                                    break;
                                case "p90":
                                    value = values.Count != 0 ? Percentile(values, 0.9) : 0;
                                    break;
                                default:
                                    value = values.Sum();
                                    break;
                            }

                            series.Points.Add(
                                new TimeSeriesPointDto
                                {
                                    Timestamp = g.Key.Bucket,
                                    Value = Math.Round(value, 3),
                                }
                            );
                        }
                    }

                    foreach (var s in resp.Series)
                        s.Points = s.Points.OrderBy(p => p.Timestamp).ToList();
                    return Result<TimeSeriesResponseDto>.Success(resp, "Ok");
                }

                // unsupported metric
                return Result<TimeSeriesResponseDto>.Failure(
                    "Unsupported metric",
                    ResultType.BadRequest
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building time series for farm {FarmId}", farmId);
                return Result<TimeSeriesResponseDto>.Failure("Error", ResultType.Unexpected);
            }
        }

        private static double Percentile(List<double> sortedValues, double p)
        {
            var values = sortedValues.OrderBy(x => x).ToList();
            if (values.Count == 0)
                return 0;
            if (p <= 0)
                return values.First();
            if (p >= 1)
                return values.Last();
            var n = values.Count;
            var idx = (int)Math.Ceiling(p * n) - 1;
            idx = Math.Max(0, Math.Min(n - 1, idx));
            return values[idx];
        }

        public async Task<Result<BatchHistoryDto>> GetBatchHistoryAsync(
            Guid batchId,
            DateTime? start,
            DateTime? end,
            string[] metrics,
            string interval
        )
        {
            try
            {
                var batch = await _unitOfWork.GetRepository<FarmingBatch>().GetByIdAsync(batchId);
                if (batch == null)
                    return Result<BatchHistoryDto>.Failure("Batch not found", ResultType.NotFound);
                var startDt = start ?? DateTime.UtcNow.AddDays(-30);
                var endDt = end ?? DateTime.UtcNow;
                // normalize interval and create bucket function (day or hour)
                interval = interval?.ToLower() ?? "day";
                Func<DateTime, DateTime> bucket = dt => dt.Date;
                if (interval == "hour")
                    bucket = dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

                // Basic series: sum feed per bucket (day or hour)
                var feedingRepo = _unitOfWork.GetRepository<FeedingLog>();
                var feeds = await feedingRepo.FindAllAsync(fl =>
                    fl.FarmingBatchId == batchId
                    && fl.CreatedDate >= startDt
                    && fl.CreatedDate <= endDt
                );
                var feedSeries = feeds
                    .GroupBy(f => bucket(f.CreatedDate))
                    .Select(g => new TimeSeriesPointDto
                    {
                        Timestamp = g.Key,
                        Value = g.Sum(x => x.Amount),
                    })
                    .ToList();

                var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();
                var morts = await mortalityRepo.FindAllAsync(ml =>
                    ml.BatchId == batchId && ml.Date >= startDt && ml.Date <= endDt
                );
                var mortSeries = morts
                    .GroupBy(m => bucket(m.Date))
                    .Select(g => new TimeSeriesPointDto
                    {
                        Timestamp = g.Key,
                        Value = g.Sum(x => x.LostWeightKg),
                    })
                    .ToList();
                var countSeries = morts
                    .GroupBy(m => bucket(m.Date))
                    .Select(g => new TimeSeriesPointDto
                    {
                        Timestamp = g.Key,
                        Value = g.Sum(x => x.Quantity),
                    })
                    .ToList();

                var fcrSeries = new List<TimeSeriesPointDto>();
                var harvestWeight = batch.ActualHarvestWeightKg;
                if (harvestWeight.HasValue && harvestWeight.Value > 0)
                {
                    var interpStart = batch.StartDate.Date;
                    var interpEnd = (batch.ActualHarvestDate ?? endDt).Date;
                    if (interpEnd < interpStart)
                        interpEnd = interpStart;

                    // use same bucket resolution for interpolation (day or hour)
                    var feedByDate = feedSeries.ToDictionary(p => p.Timestamp, p => p.Value);
                    var deadWeightByDate = mortSeries.ToDictionary(p => p.Timestamp, p => p.Value);

                    double cumulativeFeed = 0.0;
                    double cumulativeDeadWeight = 0.0;
                    var interpStartBucket = bucket(batch.StartDate);
                    var interpEndBucket = bucket((batch.ActualHarvestDate ?? endDt));
                    var windowStart =
                        bucket(startDt) > interpStartBucket ? bucket(startDt) : interpStartBucket;
                    var windowEnd =
                        bucket(endDt) < interpEndBucket ? bucket(endDt) : interpEndBucket;

                    for (
                        var d = windowStart;
                        d <= windowEnd;
                        d = interval == "hour" ? d.AddHours(1) : d.AddDays(1)
                    )
                    {
                        if (feedByDate.TryGetValue(d, out var feedValue))
                            cumulativeFeed += feedValue;
                        if (deadWeightByDate.TryGetValue(d, out var deadWeightValue))
                            cumulativeDeadWeight += deadWeightValue;
                        // compute fraction of harvest date progress relative to total interpolation span
                        var totalSpanUnits =
                            interval == "hour"
                                ? (int)(interpEndBucket - interpStartBucket).TotalHours + 1
                                : (int)(interpEndBucket - interpStartBucket).TotalDays + 1;
                        var unitsSinceStart =
                            interval == "hour"
                                ? (int)(d - interpStartBucket).TotalHours + 1
                                : (int)(d - interpStartBucket).TotalDays + 1;
                        var fraction = Math.Max(
                            0.0,
                            Math.Min(1.0, (double)unitsSinceStart / Math.Max(1, totalSpanUnits))
                        );
                        var interpolatedHarvestWeight = fraction * harvestWeight.Value;
                        var biomassGain = interpolatedHarvestWeight - cumulativeDeadWeight;
                        if (biomassGain <= 0)
                            continue;

                        fcrSeries.Add(
                            new TimeSeriesPointDto
                            {
                                Timestamp = d,
                                Value = Math.Round(cumulativeFeed / biomassGain, 3),
                            }
                        );
                    }
                }

                var dto = new BatchHistoryDto
                {
                    BatchId = batchId,
                    BatchName = batch.Name,
                    FeedSeries = feedSeries,
                    MortalitySeries = mortSeries,
                    CountSeries = countSeries,
                    FcrSeries = fcrSeries,
                };

                return Result<BatchHistoryDto>.Success(dto, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting batch history for {BatchId}", batchId);
                return Result<BatchHistoryDto>.Failure("Error", ResultType.Unexpected);
            }
        }

        public async Task<Result<List<BatchSummaryDto>>> GetTopBatchesAsync(
            Guid farmId,
            DateTime? start,
            DateTime? end,
            string metric,
            int limit
        )
        {
            try
            {
                var startDt = start ?? DateTime.UtcNow.AddDays(-30);
                var endDt = end ?? DateTime.UtcNow;
                var batchRepo = _unitOfWork.GetRepository<FarmingBatch>();
                var batches = await batchRepo.FindAllAsync(b => b.FishTank.FarmId == farmId);

                var feedingRepo = _unitOfWork.GetRepository<FeedingLog>();
                var mortalityRepo = _unitOfWork.GetRepository<MortalityLog>();

                var feedByBatch = new Dictionary<Guid, double>();
                var deathsByBatchCount = new Dictionary<Guid, int>();
                var deathsByBatchWeight = new Dictionary<Guid, double>();

                foreach (var b in batches)
                {
                    var f = await feedingRepo.SumAsync(
                        fl => fl.Amount,
                        fl =>
                            fl.FarmingBatchId == b.Id
                            && (
                                (fl.CreatedDate >= startDt && fl.CreatedDate <= endDt)
                                || (fl.CreatedAt >= startDt && fl.CreatedAt <= endDt)
                            )
                    );
                    if (f > 0)
                        feedByBatch[b.Id] = f;

                    var count = await mortalityRepo.CountAsync(ml =>
                        ml.BatchId == b.Id
                        && (
                            (ml.Date >= startDt && ml.Date <= endDt)
                            || (ml.CreatedAt >= startDt && ml.CreatedAt <= endDt)
                        )
                    );
                    if (count > 0)
                        deathsByBatchCount[b.Id] = count;

                    var weight = await mortalityRepo.SumAsync(
                        ml => ml.LostWeightKg,
                        ml =>
                            ml.BatchId == b.Id
                            && (
                                (ml.Date >= startDt && ml.Date <= endDt)
                                || (ml.CreatedAt >= startDt && ml.CreatedAt <= endDt)
                            )
                    );
                    if (weight > 0)
                        deathsByBatchWeight[b.Id] = weight;
                }

                var fishTankRepo = _unitOfWork.GetRepository<FishTank>();
                var fishTanks = await fishTankRepo.FindAllAsync(ft => ft.FarmId == farmId);
                var tankNameById = fishTanks.ToDictionary(
                    ft => ft.Id,
                    ft => ft.Name ?? string.Empty
                );

                var metricNormalized = (metric ?? "feed").ToLower();

                var list = batches
                    .Select(b => new BatchSummaryDto
                    {
                        BatchId = b.Id,
                        BatchName = b.Name,
                        FishTankId = b.FishTankId,
                        FishTankName = tankNameById.TryGetValue(b.FishTankId, out var tn)
                            ? tn
                            : string.Empty,
                        InitialQuantity = b.InitialQuantity,
                        CurrentQuantity = b.CurrentQuantity,
                        TotalFeedKg = Math.Round(
                            feedByBatch.TryGetValue(b.Id, out var tf) ? tf : 0.0,
                            3
                        ),
                        TotalDeaths = deathsByBatchCount.TryGetValue(b.Id, out var dc) ? dc : 0,
                        TotalDeadWeightKg = Math.Round(
                            deathsByBatchWeight.TryGetValue(b.Id, out var dw) ? dw : 0.0,
                            3
                        ),
                        Fcr =
                            b.Fcr
                            ?? (
                                (
                                    feedByBatch.TryGetValue(b.Id, out var fb)
                                    && (b.ActualHarvestWeightKg ?? 0) > 0
                                )
                                    ? Math.Round(fb / (b.ActualHarvestWeightKg ?? 0.0), 3)
                                    : b.Fcr
                            ),
                    })
                    .ToList();

                // order by requested metric
                var ordered = metricNormalized switch
                {
                    "mortality" => list.OrderByDescending(x => x.TotalDeaths),
                    "deadweight" => list.OrderByDescending(x => x.TotalDeadWeightKg),
                    "totaldeadweight" => list.OrderByDescending(x => x.TotalDeadWeightKg),
                    "deaths" => list.OrderByDescending(x => x.TotalDeaths),
                    "fcr" => list.OrderBy(x => x.Fcr ?? double.MaxValue),
                    _ => list.OrderByDescending(x => x.TotalFeedKg),
                };

                var resultList = ordered.Take(limit).ToList();

                return Result<List<BatchSummaryDto>>.Success(resultList, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top batches for {FarmId}", farmId);
                return Result<List<BatchSummaryDto>>.Failure("Error", ResultType.Unexpected);
            }
        }
    }
}
