using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFeedingLogService
    {
        Task<PaginatedResult<FeedingLogDto>> GetAllFeedingLogsAsync(int page, int pageSize);
        Task<Result<FeedingLogDto>> GetFeedingLogByIdAsync(Guid id);
        Task<Result<FeedingLogDto>> CreateFeedingLogAsync(CreateFeedingLogDto createDto);
        Task<Result> UpdateFeedingLogAsync(Guid id, UpdateFeedingLogDto updateDto);
        Task<Result> DeleteFeedingLogAsync(Guid id);
    }
}
