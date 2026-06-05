using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISpeciesThresholdService
    {
        Task<PaginatedResult<SpeciesThresholdDto>> GetAllSpeciesThresholdsAsync(
            SpeciesThresholdListRequest request
        );
        Task<Result<SpeciesThresholdDto>> GetSpeciesThresholdById(Guid id);
        Task<Result<SpeciesThresholdDto>> GetSpeciesThresholdBySpecies(Guid speciesId);
        Task<Result<SpeciesThresholdDto>> CreateSpeciesThreshold(
            CreateSpeciesThresholdDto dto,
            Guid? userId = null
        );
        Task<Result> UpdateSpeciesThreshold(Guid id, UpdateSpeciesThresholdDto dto);
        Task<Result> DeleteSpeciesThreshold(Guid id);
    }
}
