using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<PaginatedResult<RecommendationDto>> GetAllRecommendationsAsync(int page, int pageSize);
        Task<Result<RecommendationDto>> GetRecommendationByIdAsync(Guid id);
        Task<Result<RecommendationDto>> CreateRecommendationAsync(
            CreateRecommendationDto createDto
        );
        Task<Result> UpdateRecommendationAsync(Guid id, UpdateRecommendationDto updateDto);
        Task<Result> DeleteRecommendationAsync(Guid id);
    }
}
