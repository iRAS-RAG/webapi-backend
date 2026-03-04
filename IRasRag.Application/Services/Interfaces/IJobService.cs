using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IJobService
    {
        Task<PaginatedResult<JobDto>> GetAllJobsAsync(JobListRequest request);
        Task<Result<JobDto>> GetJobByIdAsync(Guid id);
        Task<Result<JobDto>> CreateJobAsync(CreateJobDto createDto);
        Task<Result<JobDto>> UpdateJobAsync(Guid id, UpdateJobDto updateDto);
        Task<Result> DeleteJobAsync(Guid id);
    }
}
