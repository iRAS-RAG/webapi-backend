using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.JobControlMappingSpecifications
{
    /// <summary>
    /// Specification chiếu một JobControlMapping theo Id thành JobControlMappingDto,
    /// bao gồm cả thông tin Job và ControlDevice.
    /// </summary>
    public class JobControlMappingDtoByIdSpec : Specification<JobControlMapping, JobControlMappingDto>
    {
        public JobControlMappingDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(jcm => jcm.Id == id)
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
