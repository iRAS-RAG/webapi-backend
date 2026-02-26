using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.JobTypeSpecifications
{
    public class JobTypeDtoListSpec : BaseListSpec<JobType, JobTypeDto>
    {
        public JobTypeDtoListSpec(JobTypeListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<JobType, object?>>>
            {
                ["name"] = jt => jt.Name,
            };

            ApplySearch(request.SearchTerm, [jt => jt.Name, jt => jt.Description]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(jt => new JobTypeDto
            {
                Id = jt.Id,
                Name = jt.Name,
                Description = jt.Description,
            });
        }
    }
}
