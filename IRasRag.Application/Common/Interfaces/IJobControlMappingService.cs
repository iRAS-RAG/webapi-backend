using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Interfaces
{
    public interface IJobControlMappingService
    {
        Task<PaginatedResult<JobControlMappingDto>> GetAllJobControlMappingsAsync(
            int page,
            int pageSize
        );
        Task<Result<JobControlMappingDto>> GetJobControlMappingByIdAsync(Guid id);
        Task<Result<JobControlMappingDto>> CreateJobControlMappingAsync(
            CreateJobControlMappingDto createDto
        );
        Task<Result> UpdateJobControlMappingAsync(Guid id, UpdateJobControlMappingDto updateDto);
        Task<Result> DeleteJobControlMappingAsync(Guid id);
    }
}
