using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class JobTypeDtoListSpec : Specification<JobType, JobTypeDto>
    {
        public JobTypeDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Select(jt => new JobTypeDto
                {
                    Id = jt.Id,
                    Name = jt.Name,
                    Description = jt.Description,
                });
        }
    }
}
