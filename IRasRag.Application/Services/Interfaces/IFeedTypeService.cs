using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFeedTypeService
    {
        Task<Result<IEnumerable<FeedTypeDto>>> GetAllFeedTypesAsync();
        Task<Result<FeedTypeDto>> GetFeedTypeByIdAsync(Guid id);
        Task<Result<FeedTypeDto>> CreateFeedTypeAsync(CreateFeedTypeDto createDto);
        Task<Result> UpdateFeedTypeAsync(Guid id, UpdateFeedTypeDto updateDto);
        Task<Result> DeleteFeedTypeAsync(Guid id);
    }
}
