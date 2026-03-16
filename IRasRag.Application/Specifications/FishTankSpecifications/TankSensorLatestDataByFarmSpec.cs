using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    public class TankSensorLatestDataByFarmSpec
     : Specification<Sensor, TankSensorLatestDataDto>
    {
        public TankSensorLatestDataByFarmSpec(Guid farmId)
        {
            Query.Where(s => s.MasterBoard.FishTank.FarmId == farmId)
                 .Select(s => new TankSensorLatestDataDto
                 {
                     FishTankId = s.MasterBoard.FishTankId,
                     FishTankName = s.MasterBoard.FishTank.Name,
                     SensorId = s.Id,
                     SensorName = s.Name,
                     SensorTypeId = s.SensorTypeId,
                     SensorTypeName = s.SensorType.Name,
                     MeasureType = s.SensorType.MeasureType,
                     UnitOfMeasure = s.SensorType.UnitOfMeasure,
                     MasterBoardId = s.MasterBoardId,
                     MasterBoardName = s.MasterBoard.Name,

                     LatestData = s.SensorLogs
                         .OrderByDescending(l => l.CreatedAt)
                         .Select(l => new TankSensorLatestDataValueDto
                         {
                             LatestValue = (double?)l.Data,
                             IsWarning = (bool?)l.IsWarning,
                             RecordedAt = l.CreatedAt
                         })
                         .FirstOrDefault()
                 });
        }
    }
}
