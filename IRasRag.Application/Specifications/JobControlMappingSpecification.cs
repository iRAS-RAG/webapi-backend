using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class JobControlMappingDtoListSpec
        : Specification<JobControlMapping, JobControlMappingDto>
    {
        public JobControlMappingDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(jcm => jcm.Job)
                .Include(jcm => jcm.ControlDevice)
                .Select(jcm => new JobControlMappingDto
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
