using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ICorrectiveActionService
    {
        Task<PaginatedResult<CorrectiveActionDto>> GetAllCorrectiveActionsAsync(
            int page,
            int pageSize
        );
        Task<Result<CorrectiveActionDto>> GetCorrectiveActionByIdAsync(Guid id);
        Task<Result<CorrectiveActionDto>> CreateCorrectiveActionAsync(
            CreateCorrectiveActionDto createDto
        );
        Task<Result> UpdateCorrectiveActionAsync(Guid id, UpdateCorrectiveActionDto updateDto);
        Task<Result> DeleteCorrectiveActionAsync(Guid id);
    }
}
