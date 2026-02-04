using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IFeedingLogService
    {
        Task<Result<IEnumerable<FeedingLogDto>>> GetAllFeedingLogsAsync();
        Task<Result<FeedingLogDto>> GetFeedingLogByIdAsync(Guid id);
        Task<Result<FeedingLogDto>> CreateFeedingLogAsync(CreateFeedingLogDto createDto);
        Task<Result> UpdateFeedingLogAsync(Guid id, UpdateFeedingLogDto updateDto);
        Task<Result> DeleteFeedingLogAsync(Guid id);
    }
}
