using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    /// <summary>
    /// Returns all sensors that belong to a given tank (via MasterBoard → FishTank),
    /// together with their SensorType info and the latest SensorLog value per sensor.
    ///
    /// Replaces the previous 4-query chain:
    ///   FindAllAsync(masterboards for tank)
    ///   → FindAllAsync(sensors for masterboards)
    ///   → FindAllAsync(sensorTypes)
    ///   → FindAllAsync(logs for sensors) + in-memory GroupBy
    ///
    /// A single SQL query with correlated subqueries for the latest log fields.
    /// </summary>
    public class TankSensorLatestDataSpec : BaseListSpec<Sensor, TankSensorLatestDataDto>
    {
        public TankSensorLatestDataSpec(Guid tankId)
        {
            Query.AsNoTracking();

            // Filter sensors whose MasterBoard belongs to the requested tank
            Query.Where(s => s.MasterBoard.FishTankId == tankId);

            Query.Select(s => new TankSensorLatestDataDto
            {
                SensorId = s.Id,
                SensorName = s.Name,
                SensorTypeId = s.SensorTypeId,
                SensorTypeName = s.SensorType.Name,
                MeasureType = s.SensorType.MeasureType,
                UnitOfMeasure = s.SensorType.UnitOfMeasure,
                MasterBoardId = s.MasterBoardId,
                MasterBoardName = s.MasterBoard.Name,
                LatestData = s
                    .SensorLogs.OrderByDescending(l => l.CreatedAt)
                    .Select(l => new TankSensorLatestDataValueDto
                    {
                        LatestAvg = l.Average,
                        LatestMax = l.Max,
                        LatestMin = l.Min,
                        HasWarning = l.HasWarning,
                        RecordedAt = l.CreatedAt,
                    })
                    .FirstOrDefault(),
            });
        }
    }
}
