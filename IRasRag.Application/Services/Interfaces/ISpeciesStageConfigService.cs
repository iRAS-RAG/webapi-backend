using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISpeciesStageConfigService
    {
        public Task<PaginatedResult<SpeciesStageConfigDto>> GetAllSpeciesStageConfigsAsync(
            SpeciesStageConfigListRequest request
        );
        public Task<PaginatedResult<SpeciesStageConfigDto>> GetSpeciesStageConfigsBySpeciesId(
            Guid speciesId,
            SpeciesStageConfigListRequest request
        );
        public Task<Result<SpeciesStageConfigDto>> GetSpeciesStageConfigById(Guid id);
        public Task<Result<SpeciesStageConfigDto>> CreateSpeciesStageConfig(
            CreateSpeciesStageConfigDto dto
        );
        public Task<Result> UpdateSpeciesStageConfig(Guid id, UpdateSpeciesStageConfigDto dto);
        public Task<Result<IReadOnlyList<SpeciesStageConfigDto>>> ReorderSpeciesStageConfigs(
            ReorderSpeciesStageConfigsDto dto
        );
        public Task<Result> DeleteSpeciesStageConfig(Guid id);
    }
}
