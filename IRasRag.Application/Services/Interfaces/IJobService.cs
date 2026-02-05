using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IJobService
    {
        Task<Result<IEnumerable<JobDto>>> GetAllJobsAsync();
        Task<Result<JobDto>> GetJobByIdAsync(Guid id);
        Task<Result<JobDto>> CreateJobAsync(CreateJobDto createDto);
        Task<Result> UpdateJobAsync(Guid id, UpdateJobDto updateDto);
        Task<Result> DeleteJobAsync(Guid id);
    }
}
