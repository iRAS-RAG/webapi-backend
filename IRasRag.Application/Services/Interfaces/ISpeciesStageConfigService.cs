using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISpeciesStageConfigService
    {
        public Task<Result<IEnumerable<SpeciesStageConfigDto>>> GetAllSpeciesStageConfigsAsync();
        public Task<Result<SpeciesStageConfigDto>> GetSpeciesStageConfigById(Guid id);
        public Task<Result<SpeciesStageConfigDto>> CreateSpeciesStageConfig(
            CreateSpeciesStageConfigDto dto
        );
        public Task<Result> UpdateSpeciesStageConfig(Guid id, UpdateSpeciesStageConfigDto dto);
        public Task<Result> DeleteSpeciesStageConfig(Guid id);
    }
}
