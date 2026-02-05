using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IJobTypeService
    {
        Task<PaginatedResult<JobTypeDto>> GetAllJobTypesAsync(int page, int pageSize);
        Task<Result<JobTypeDto>> GetJobTypeByIdAsync(Guid id);
        Task<Result<JobTypeDto>> CreateJobTypeAsync(CreateJobTypeDto createDto);
        Task<Result> UpdateJobTypeAsync(Guid id, UpdateJobTypeDto updateDto);
        Task<Result> DeleteJobTypeAsync(Guid id);
    }
}
