using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface ISpeciesService
    {
        Task<PaginatedResult<SpeciesDto>> GetAllSpeciesAsync(int page, int pageSize);
        Task<Result<SpeciesDto>> GetSpeciesByIdAsync(Guid id);
        Task<Result<SpeciesDto>> CreateSpeciesAsync(CreateSpeciesDto createDto);
        Task<Result> UpdateSpeciesAsync(Guid id, UpdateSpeciesDto updateDto);
        Task<Result> DeleteSpeciesAsync(Guid id);
    }
}
