using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFeedTypeService
    {
        Task<PaginatedResult<FeedTypeDto>> GetAllFeedTypesAsync(FeedTypeListRequest request);
        Task<Result<FeedTypeDto>> GetFeedTypeByIdAsync(Guid id);
        Task<Result<FeedTypeDto>> CreateFeedTypeAsync(CreateFeedTypeDto createDto);
        Task<Result> UpdateFeedTypeAsync(Guid id, UpdateFeedTypeDto updateDto);
        Task<Result> DeleteFeedTypeAsync(Guid id);
    }
}
