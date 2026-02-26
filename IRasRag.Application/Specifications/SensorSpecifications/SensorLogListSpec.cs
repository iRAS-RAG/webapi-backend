using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SensorSpecifications
{
    public class SensorLogListSpec : BaseListSpec<SensorLog, SensorLogDto>
    {
        public SensorLogListSpec(Guid sensorId, SensorLogListRequest request)
        {
            Query.AsNoTracking();

            ApplyFilter(sensorId, log => log.SensorId == sensorId);
            ApplyFilter(request.From, log => log.CreatedAt >= request.From);
            ApplyFilter(request.To, log => log.CreatedAt <= request.To);

            Query.OrderBy(log => log.CreatedAt);

            Query.Select(log => new SensorLogDto
            {
                Id = log.Id,
                SensorId = log.SensorId,
                Data = log.Data,
                IsWarning = log.IsWarning,
                DataJson = log.DataJson,
                CreatedAt = log.CreatedAt,
            });
        }
    }
}
