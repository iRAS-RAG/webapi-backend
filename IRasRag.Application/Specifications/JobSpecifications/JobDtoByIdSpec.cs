using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.JobSpecifications
{
    /// <summary>
    /// Specification lấy thông tin chi tiết của một Job theo Id,
    /// bao gồm danh sách JobControlMappings liên quan.
    /// </summary>
    public class JobDtoByIdSpec : Specification<Job, JobDto>
    {
        public JobDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(j => j.Id == id)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Name = j.Name,
                    Description = j.Description,
                    JobTypeId = j.JobTypeId,
                    JobTypeName = j.JobType != null ? j.JobType.Name : string.Empty,
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
