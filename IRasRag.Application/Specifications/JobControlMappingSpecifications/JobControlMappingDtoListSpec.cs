using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.JobControlMappingSpecifications
{
    public class JobControlMappingDtoListSpec
        : BaseListSpec<JobControlMapping, JobControlMappingDto>
    {
        public JobControlMappingDtoListSpec(JobControlMappingListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<JobControlMapping, object?>>>
            {
                ["jobname"] = jcm => jcm.Job.Name,
                ["controldevicename"] = jcm => jcm.ControlDevice.Name,
                ["targetstate"] = jcm => jcm.TargetState,
                ["triggercondition"] = jcm => jcm.TriggerCondition,
                ["createdat"] = jcm => jcm.CreatedAt,
            };

            ApplySearch(request.SearchTerm, [jcm => jcm.Job.Name, jcm => jcm.ControlDevice.Name]);

            ApplyFilter(request.TargetState, jcm => jcm.TargetState == request.TargetState);
            ApplyFilter(
                request.TriggerCondition,
                jcm => jcm.TriggerCondition == request.TriggerCondition
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "createdat");

            Query.Select(jcm => new JobControlMappingDto
            {
                Id = jcm.Id,
                JobId = jcm.JobId,
                JobName = jcm.Job.Name,
                ControlDeviceId = jcm.ControlDeviceId,
                ControlDeviceName = jcm.ControlDevice.Name,
                TargetState = jcm.TargetState,
                TriggerCondition = jcm.TriggerCondition,
                CreatedAt = jcm.CreatedAt,
                ModifiedAt = jcm.ModifiedAt,
            });
        }
    }
}
