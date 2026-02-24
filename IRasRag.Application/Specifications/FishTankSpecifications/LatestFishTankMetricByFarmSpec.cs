using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    public class LatestFishTankMetricByFarmSpec : Specification<FishTank, FishTankMetricDto>
    {
        public LatestFishTankMetricByFarmSpec(Guid farmId)
        {
            Query
                .AsNoTracking()
                .Where(ft => ft.FarmId == farmId)
                .OrderBy(ft => ft.Name)
                .Select(ft => new FishTankMetricDto
                {
                    FishTankId = ft.Id,
                    FishTankName = ft.Name,
                    SensorMetrics = ft.MasterBoards
                        .SelectMany(mb => mb.Sensors)
                        .Select(s => new
                        {
                            s,
                            LatestLog = s.SensorLogs
                                .OrderByDescending(sl => sl.CreatedAt)
                                .Select(sl => new
                                {
                                    sl.Data,
                                    sl.IsWarning
                                })
                                .FirstOrDefault()
                        })
                        .Where(x => x.LatestLog != null)
                        .Select(x => new SensorMetricDto
                        {
                            SensorName = x.s.Name,
                            SensorTypeName = x.s.SensorType.Name,
                            Value = new SensorMetricValueDto
                            {
                                Value = x.LatestLog != null ? x.LatestLog.Data : 0,
                                UnitOfMeasure = x.s.SensorType.UnitOfMeasure,
                                Status = x.LatestLog != null && x.LatestLog.IsWarning
                                    ? "warning"
                                    : "safe"
                            }
                        })
                        .ToList()
                });
        }
    }
}
