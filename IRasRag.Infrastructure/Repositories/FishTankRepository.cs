using System.Threading.Tasks;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Repositories
{
    public class FishTankRepository : Repository<FishTank>, IFishTankRepository
    {
        private readonly AppDbContext _context;

        public FishTankRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<FishTankMetricDto>> GetLatestFishTankMetricsByFarmIdAsync(
            Guid farmId
        )
        {
            var tanks = await _context
                .FishTanks.Where(ft => ft.FarmId == farmId)
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();

            var latestLogIds = await _context
                .SensorLogs.Where(sl => sl.Sensor.MasterBoard.FishTank.FarmId == farmId)
                .GroupBy(sl => sl.SensorId)
                .Select(g => g.OrderByDescending(sl => sl.CreatedAt).First().Id)
                .ToListAsync();

            var latestLogs = await _context
                .SensorLogs.Where(sl => latestLogIds.Contains(sl.Id))
                .Select(sl => new
                {
                    FishTankId = sl.Sensor.MasterBoard.FishTankId,
                    SensorName = sl.Sensor.Name,
                    SensorTypeName = sl.Sensor.SensorType.Name,
                    UnitOfMeasure = sl.Sensor.SensorType.UnitOfMeasure,
                    sl.Data,
                    sl.IsWarning,
                    sl.CreatedAt,
                })
                .ToListAsync();

            var logsByTank = latestLogs
                .GroupBy(sl => sl.FishTankId)
                .ToDictionary(g => g.Key, g => g.ToList());

            return tanks
                .Select(t =>
                {
                    var tankLogs = logsByTank.GetValueOrDefault(t.Id) ?? [];
                    return new FishTankMetricDto
                    {
                        FishTankId = t.Id,
                        FishTankName = t.Name,
                        SensorMetrics = tankLogs
                            .Select(log => new SensorMetricDto
                            {
                                SensorName = log.SensorName,
                                SensorTypeName = log.SensorTypeName,
                                Value = new SensorMetricValueDto
                                {
                                    Value = log.Data,
                                    UnitOfMeasure = log.UnitOfMeasure,
                                    LastUpdated = log.CreatedAt ?? DateTime.MinValue,
                                    IsWarning = log.IsWarning,
                                },
                            })
                            .ToList(),
                    };
                })
                .ToList();
        }

    }
}
