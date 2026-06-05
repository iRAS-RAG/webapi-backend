using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Persistence;
using IRasRag.Infrastructure.Persistence.DbFunctions;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Repositories
{
    public class SensorLogRepository : Repository<SensorLog>, ISensorLogRepository
    {
        public SensorLogRepository(AppDbContext context)
            : base(context) { }

        public async Task<(
            IReadOnlyList<SensorLogDto> Items,
            int TotalCount
        )> GetAggregatedLogsAsync(
            Guid sensorId,
            string sensorName,
            DateTime from,
            DateTime to,
            int interval,
            int page,
            int pageSize
        )
        {
            // Fetch 1-minute summary rows for the range
            var filtered = await GetQueryable()
                .AsNoTracking()
                .Where(sl =>
                    sl.SensorId == sensorId && sl.PeriodStart >= from && sl.PeriodStart < to
                )
                .ToListAsync();

            // Re-bucket in memory for intervals > 1 minute
            // Weighted average preserves accuracy for windows with fewer than 60 samples
            var grouped = filtered
                .GroupBy(x =>
                    x.PeriodStart.Ticks
                    / TimeSpan.FromMinutes(interval).Ticks
                    * TimeSpan.FromMinutes(interval).Ticks
                )
                .Select(g => new
                {
                    Bucket = g.Key,
                    Avg = g.Sum(x => x.Average * x.SampleCount) / g.Sum(x => x.SampleCount),
                    Min = g.Min(x => x.Min),
                    Max = g.Max(x => x.Max),
                    SampleCount = g.Sum(x => x.SampleCount),
                    HasWarning = g.Any(x => x.HasWarning),
                })
                .OrderBy(x => x.Bucket)
                .ToList();

            var totalCount = grouped.Count;

            var items = grouped
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SensorLogDto
                {
                    Id = Guid.Empty,
                    SensorId = sensorId,
                    SensorName = sensorName,
                    Average = x.Avg,
                    Min = x.Min,
                    Max = x.Max,
                    SampleCount = x.SampleCount,
                    HasWarning = x.HasWarning,
                    PeriodStart = new DateTime(x.Bucket, DateTimeKind.Utc),
                    CreatedAt = new DateTime(x.Bucket, DateTimeKind.Utc),
                })
                .ToList();

            return (items, totalCount);
        }

        public async Task<List<SensorHistoryPointDto>> GetLogsByTimeRangeAsync(
            Guid sensorId,
            DateTime from,
            DateTime to,
            int interval
        )
        {
            var utcFrom = from.Kind == DateTimeKind.Utc ? from : from.ToUniversalTime();
            var utcTo = to.Kind == DateTimeKind.Utc ? to : to.ToUniversalTime();

            if (utcTo <= utcFrom)
                throw new ArgumentException("To must be greater than From");
            if (interval <= 0)
                throw new ArgumentException("Interval must be a positive integer");
            if (interval > (utcTo - utcFrom).TotalMinutes)
                throw new ArgumentException("Interval cannot be greater than the total time range");

            var bucketCount = (int)Math.Ceiling((utcTo - utcFrom).TotalMinutes / interval);
            var stride = TimeSpan.FromMinutes(interval);
            var vietnamOffset = TimeSpan.FromHours(7);

            // date_bin buckets the 1-min PeriodStart into the requested interval
            // Sum(Average * SampleCount) / Sum(SampleCount) is the weighted average
            var buckets = await _context
                .Set<SensorLog>()
                .AsNoTracking()
                .Where(sl =>
                    sl.SensorId == sensorId && sl.PeriodStart >= utcFrom && sl.PeriodStart < utcTo
                )
                .GroupBy(sl => PgFunctions.DateBin(stride, sl.PeriodStart, utcFrom))
                .Select(g => new
                {
                    BucketStart = g.Key,
                    Avg = g.Sum(x => x.Average * x.SampleCount) / g.Sum(x => x.SampleCount),
                })
                .ToListAsync();

            var lookup = buckets.ToDictionary(
                b => (long)(b.BucketStart - utcFrom).TotalMinutes / interval,
                b => b.Avg
            );

            return Enumerable
                .Range(0, bucketCount)
                .Select(i => new SensorHistoryPointDto
                {
                    RecordedAt = new DateTimeOffset(
                        utcFrom.AddMinutes((double)i * interval),
                        TimeSpan.Zero
                    ).ToOffset(vietnamOffset),
                    Value = lookup.TryGetValue(i, out var v) ? v : null,
                })
                .ToList();
        }
    }
}
