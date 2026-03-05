using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Repositories
{
    public class SensorLogRepository : Repository<SensorLog>, ISensorLogRepository
    {
        public SensorLogRepository(AppDbContext context)
            : base(context) { }

        public async Task<(IReadOnlyList<SensorLogDto> Items, int TotalCount)> GetAggregatedLogsAsync(
            Guid sensorId, DateTime from, DateTime to, int interval, int page, int pageSize)
        {
            var intervalTicks = TimeSpan.FromMinutes(interval).Ticks;

            // WHERE in DB (uses index on sensor_id + created_at), grouping in memory
            // because EF Core cannot translate Ticks arithmetic to SQL
            var filtered = await GetQueryable()
                .AsNoTracking()
                .Where(sl =>
                    sl.SensorId == sensorId
                    && sl.CreatedAt >= from
                    && sl.CreatedAt < to
                )
                .ToListAsync();

            var grouped = filtered
                .GroupBy(x => x.CreatedAt!.Value.Ticks / intervalTicks * intervalTicks)
                .Select(g => new
                {
                    Bucket = g.Key,
                    Avg = g.Average(x => x.Data),
                    HasWarning = g.Any(x => x.IsWarning),
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
                    Data = x.Avg,
                    IsWarning = x.HasWarning,
                    DataJson = "{}",
                    CreatedAt = new DateTime(x.Bucket, DateTimeKind.Utc),
                })
                .ToList();

            return (items, totalCount);
        }

        public async Task<SensorHistoryDto> GetLogsByTimeRangeAsync(Guid sensorId, DateTime from, DateTime to, int interval)
        {
            var intervalTicks = TimeSpan.FromMinutes(interval).Ticks;

            // WHERE in DB, grouping in memory — EF Core cannot translate Ticks arithmetic to SQL
            var filtered = await GetQueryable()
                .AsNoTracking()
                .Where(sl =>
                    sl.SensorId == sensorId
                    && sl.CreatedAt >= from
                    && sl.CreatedAt < to
                )
                .ToListAsync();

            var query = filtered
                .GroupBy(x => x.CreatedAt!.Value.Ticks / intervalTicks * intervalTicks)
                .OrderBy(g => g.Key)
                .Select(g => new { Bucket = g.Key, Avg = g.Average(x => x.Data) })
                .ToList();

            var totalMinutes = (int)(to - from).TotalMinutes;
            var bucketCount = totalMinutes / interval;
            var fixedBuckets = Enumerable.Range(0, bucketCount)
                .Select(i => from.AddMinutes(i * interval).Ticks)
                .ToList();

            var dataset = fixedBuckets
                .Select(bucket => query.FirstOrDefault(x => x.Bucket == bucket)?.Avg ?? 0)
                .ToList();

            return new SensorHistoryDto { Datasets = dataset };
        }
    }
}
