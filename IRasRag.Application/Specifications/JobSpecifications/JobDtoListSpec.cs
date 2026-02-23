using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.JobSpecifications
{
    public class JobDtoListSpec : BaseListSpec<Job, JobDto>
    {
        public JobDtoListSpec(JobListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<Job, object?>>>
            {
                ["name"] = j => j.Name,
                ["isactive"] = j => j.IsActive,
                ["defaultstate"] = j => j.DefaultState,
                ["jobtypename"] = j => j.JobType.Name,
                ["sensorname"] = j => j.Sensor != null ? j.Sensor.Name : null,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    j => j.Name,
                    j => j.Description,
                    j => j.JobType.Name,
                    j => j.Sensor != null ? j.Sensor.Name : null,
                ]
            );

            ApplyFilter(request.DefaultState, j => j.DefaultState == request.DefaultState);
            ApplyFilter(request.IsActive, j => j.IsActive == request.IsActive);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(j => new JobDto
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
