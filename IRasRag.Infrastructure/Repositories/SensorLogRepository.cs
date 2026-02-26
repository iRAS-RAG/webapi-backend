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

        public async Task<SensorHistoryDto> GetLogsByTimeRangeAsync(Guid sensorId, DateOnly date)
        {
            var start = date;
            var end = date.AddDays(1);
            var query = await GetQueryable()
                .AsNoTracking()
                .Where(sl =>
                    sl.SensorId == sensorId
                    && sl.CreatedAt >= start.ToDateTime(TimeOnly.MinValue)
                    && sl.CreatedAt < end.ToDateTime(TimeOnly.MinValue)
                )
                .GroupBy(x => (x.CreatedAt.Value.Hour / 4) * 4)
                .OrderBy(g => g.Key)
                .Select(g => new { Bucket = g.Key, Avg = g.Average(x => x.Data) })
                .ToListAsync();

            var fixedBuckets = new[] { 0, 4, 8, 12, 16, 20 };

            var dataset = fixedBuckets
                .Select(hour => query.FirstOrDefault(x => x.Bucket == hour)?.Avg ?? 0)
                .ToList();

            return new SensorHistoryDto { Datasets = dataset };
        }
    }
}
