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
        public SensorLogRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<SensorLogDto>> GetLatestLogPerSensorByTankId(Guid fishTankId)
        {
            var query = GetQueryable();
            var result = await query
                    .AsNoTracking()
                    .Where
                    (
                        sl => sl.Sensor.MasterBoard.FishTankId == fishTankId &&
                        // Keep only the latest log for each sensor (by max CreatedAt)
                        sl.CreatedAt == query.Where(innerSl => innerSl.SensorId == sl.SensorId)
                            .Max(innerSl => innerSl.CreatedAt)
                    )
                    .Select(sl => new SensorLogDto
                    {
                        SensorName = sl.Sensor.Name,
                        SensorTypeName = sl.Sensor.SensorType.Name,
                        LastValue = sl.Data,
                        LastUpdated = sl.CreatedAt!.Value
                    })
                    .ToListAsync();
            return result;
        }

        public async Task<IReadOnlyList<SensorLogDto>> GetLatestLogPerSensorByFarm(Guid farmId)
        {
            var query = GetQueryable();
            var result = await query
                    .AsNoTracking()
                    .Where
                    (
                        sl => sl.Sensor.MasterBoard.FishTank.FarmId == farmId &&
                        // Keep only the latest log for each sensor (by max CreatedAt)
                        sl.CreatedAt == query.Where(innerSl => innerSl.SensorId == sl.SensorId)
                            .Max(innerSl => innerSl.CreatedAt)
                    )
                    .Select(sl => new SensorLogDto
                    {
                        SensorName = sl.Sensor.Name,
                        SensorTypeName = sl.Sensor.SensorType.Name,
                        LastValue = sl.Data,
                        LastUpdated = sl.CreatedAt!.Value
                    })
                    .ToListAsync();
            return result;
        }
    }
}
