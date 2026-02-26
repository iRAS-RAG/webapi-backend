using System.Threading.Tasks;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
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
            // 1. Fetch all tanks belonging to the farm (id + name only, no full entity needed)
            var tanks = await _context
                .FishTanks.Where(ft => ft.FarmId == farmId)
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();

            // 2. Fetch the single most recent log per sensor across the entire farm
            //    GroupBy SensorId → take the latest CreatedAt entry from each group
            var latestLogs = await _context
                .SensorLogs.Where(sl => sl.Sensor.MasterBoard.FishTank.FarmId == farmId)
                .GroupBy(sl => sl.SensorId)
                .Select(g => g.OrderByDescending(sl => sl.CreatedAt).First())
                .ToListAsync();

            // 3. Fetch warning thresholds from the species of each tank's currently ACTIVE farming batch
            //    Path: FishTank → ActiveFarmingBatch → Species → SpeciesThresholds
            //    FishTankId is carried along so thresholds can be matched back to their tank later
            var thresholds = await _context
                .FishTanks.Where(ft => ft.FarmId == farmId)
                .SelectMany(ft =>
                    ft.FarmingBatches.Where(fb => fb.Status == FarmingBatchStatus.ACTIVE)
                        .SelectMany(fb =>
                            fb.Species.SpeciesThresholds.Select(st => new
                            {
                                st.SensorTypeId,
                                st.MinValue,
                                st.MaxValue,
                                fb.FishTankId,
                            })
                        )
                )
                .ToListAsync();

            // 4. Build lookup dictionaries in-memory to avoid O(n²) scanning in the projection below
            //    logsByTank:       TankId → list of latest sensor logs for that tank
            //    thresholdsByTank: TankId → (SensorTypeId → threshold) for O(1) threshold lookup per log
            var logsByTank = latestLogs
                .GroupBy(sl => sl.Sensor.MasterBoard.FishTankId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var thresholdsByTank = thresholds
                .GroupBy(t => t.FishTankId)
                .ToDictionary(g => g.Key, g => g.ToDictionary(st => st.SensorTypeId));

            // 5. Project each tank into a FishTankMetricDto
            //    For each log, look up the matching threshold by SensorTypeId and compute IsWarning
            var result = tanks
                .Select(t =>
                {
                    var tankLogs = logsByTank.GetValueOrDefault(t.Id) ?? [];
                    var tankThresholds = thresholdsByTank.GetValueOrDefault(t.Id) ?? [];

                    return new FishTankMetricDto
                    {
                        FishTankId = t.Id,
                        FishTankName = t.Name,
                        SensorMetrics = tankLogs
                            .Select(log =>
                            {
                                // O(1) lookup — threshold may be null if no threshold is defined for this sensor type
                                tankThresholds.TryGetValue(
                                    log.Sensor.SensorTypeId,
                                    out var threshold
                                );
                                return new SensorMetricDto
                                {
                                    SensorTypeName = log.Sensor.SensorType.Name,
                                    Value = new SensorMetricValueDto
                                    {
                                        Value = log.Data,
                                        UnitOfMeasure = log.Sensor.SensorType.UnitOfMeasure,
                                        LastUpdated = log.CreatedAt ?? DateTime.MinValue,
                                        // Warning only fires when a threshold exists and the value is out of range
                                        IsWarning =
                                            (threshold != null)
                                            && (
                                                log.Data < threshold.MinValue
                                                || log.Data > threshold.MaxValue
                                            ),
                                    },
                                };
                            })
                            .ToList(),
                    };
                })
                .ToList();

            return result;
        }

        public async Task<FishTankMetricDto> GetLatestFishTankMetricsByTankIdAsync(Guid tankId)
        {
            // 1. Fetch the tank entity — throws if not found
            var tank = await _context.FishTanks.FindAsync(tankId);
            if (tank == null)
                throw new Exception();

            // 2. Fetch the single most recent log per sensor for this tank
            //    GroupBy SensorId → take the latest CreatedAt entry from each group
            var latestLogs = await _context
                .SensorLogs.Where(sl => sl.Sensor.MasterBoard.FishTankId == tankId)
                .GroupBy(sl => sl.SensorId)
                .Select(g => g.OrderByDescending(sl => sl.CreatedAt).First())
                .ToListAsync();

            // 3. Fetch warning thresholds from the species of this tank's currently ACTIVE farming batch
            //    Path: FishTank → ActiveFarmingBatch → Species → SpeciesThresholds
            //    No need to carry FishTankId — already scoped to a single tank
            var thresholds = await _context
                .FishTanks.Where(ft => ft.Id == tankId)
                .SelectMany(ft =>
                    ft.FarmingBatches.Where(fb => fb.Status == FarmingBatchStatus.ACTIVE)
                )
                .SelectMany(fb =>
                    fb.Species.SpeciesThresholds.Select(st => new
                    {
                        st.SensorTypeId,
                        st.MinValue,
                        st.MaxValue,
                    })
                )
                .ToListAsync();

            // 4. Index thresholds by SensorTypeId for O(1) lookup during projection
            //    No per-tank grouping needed — already scoped to one tank
            var thresholdsBySensorType = thresholds.ToDictionary(st => st.SensorTypeId);

            // 5. Project into FishTankMetricDto
            //    For each log, look up the matching threshold by SensorTypeId and compute IsWarning
            return new FishTankMetricDto
            {
                FishTankId = tank.Id,
                FishTankName = tank.Name,
                SensorMetrics = latestLogs
                    .Select(log =>
                    {
                        // O(1) lookup — threshold may be null if no threshold defined for this sensor type
                        thresholdsBySensorType.TryGetValue(
                            log.Sensor.SensorTypeId,
                            out var threshold
                        );
                        return new SensorMetricDto
                        {
                            SensorTypeName = log.Sensor.SensorType.Name,
                            Value = new SensorMetricValueDto
                            {
                                Value = log.Data,
                                UnitOfMeasure = log.Sensor.SensorType.UnitOfMeasure,
                                LastUpdated = log.CreatedAt ?? DateTime.MinValue,
                                // Warning only fires when a threshold exists and the value is out of range
                                IsWarning =
                                    threshold != null
                                    && (
                                        log.Data < threshold.MinValue
                                        || log.Data > threshold.MaxValue
                                    ),
                            },
                        };
                    })
                    .ToList(),
            };
        }
    }
}
