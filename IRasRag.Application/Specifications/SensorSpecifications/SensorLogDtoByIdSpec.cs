using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SensorSpecifications
{
    /// <summary>
    /// Specification chiếu một SensorLog theo Id thành SensorLogDto,
    /// bao gồm cả thông tin Sensor (SensorName).
    /// </summary>
    public class SensorLogDtoByIdSpec : Specification<SensorLog, SensorLogDto>
    {
        public SensorLogDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(sl => sl.Id == id)
                .Select(sl => new SensorLogDto
                {
                    Id = sl.Id,
                    SensorId = sl.SensorId,
                    SensorName = sl.Sensor.Name,
                    Data = sl.Data,
                    IsWarning = sl.IsWarning,
                    DataJson = sl.DataJson,
                    CreatedAt = sl.CreatedAt,
                });
        }
    }
}
