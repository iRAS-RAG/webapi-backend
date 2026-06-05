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
            ApplyFilter(request.From, log => log.PeriodStart >= request.From);
            ApplyFilter(request.To, log => log.PeriodStart <= request.To);

            Query.OrderByDescending(log => log.PeriodStart);

            Query.Select(log => new SensorLogDto
            {
                Id = log.Id,
                SensorId = log.SensorId,
                Average = log.Average,
                Min = log.Min,
                Max = log.Max,
                SampleCount = log.SampleCount,
                HasWarning = log.HasWarning,
                PeriodStart = log.PeriodStart,
                CreatedAt = log.CreatedAt,
            });
        }
    }
}
