using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IJobTypeService
    {
        Task<Result<IEnumerable<JobTypeDto>>> GetAllJobTypesAsync();
        Task<Result<JobTypeDto>> GetJobTypeByIdAsync(Guid id);
        Task<Result<JobTypeDto>> CreateJobTypeAsync(CreateJobTypeDto createDto);
        Task<Result> UpdateJobTypeAsync(Guid id, UpdateJobTypeDto updateDto);
        Task<Result> DeleteJobTypeAsync(Guid id);
    }
}
