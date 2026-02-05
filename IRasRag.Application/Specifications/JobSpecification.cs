using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class JobDtoListSpec : Specification<Job, JobDto>
    {
        public JobDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(j => j.JobType)
                .Include(j => j.Sensor)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Name = j.Name,
                    Description = j.Description,
                    JobTypeId = j.JobTypeId,
                    JobTypeName = j.JobType.Name,
                    SensorId = j.SensorId,
                    SensorName = j.Sensor != null ? j.Sensor.Name : null,
                    MinValue = j.MinValue,
                    MaxValue = j.MaxValue,
                    DefaultState = j.DefaultState,
                    IsActive = j.IsActive,
                    StartTime = j.StartTime,
                    EndTime = j.EndTime,
                    RepeatIntervalMinutes = j.RepeatIntervalMinutes,
                    ExecutionDays = j.ExecutionDays,
                });
        }
    }
}
