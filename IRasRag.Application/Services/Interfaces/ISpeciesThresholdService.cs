using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISpeciesThresholdService
    {
        Task<PaginatedResult<SpeciesThresholdDto>> GetAllSpeciesThresholdsAsync(
            int page,
            int pageSize
        );
        Task<Result<SpeciesThresholdDto>> GetSpeciesThresholdById(Guid id);
        Task<Result<SpeciesThresholdDto>> CreateSpeciesThreshold(CreateSpeciesThresholdDto dto);
        Task<Result> UpdateSpeciesThreshold(Guid id, UpdateSpeciesThresholdDto dto);
        Task<Result> DeleteSpeciesThreshold(Guid id);
    }
}
