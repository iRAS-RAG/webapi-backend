using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IMortalityLogService
    {
        Task<PaginatedResult<MortalityLogDto>> GetAllMortalityLogsAsync(
            MortalityLogListRequest request
        );
        Task<Result<MortalityLogDto>> GetMortalityLogByIdAsync(Guid id);
        Task<Result<MortalityLogDto>> CreateMortalityLogAsync(CreateMortalityLogDto createDto);
        Task<Result<MortalityValidationResultDto>> ValidateMortalityAsync(
            Guid batchId,
            int quantity,
            double? lostWeightKg = null
        );
        Task<Result> UpdateMortalityLogAsync(Guid id, UpdateMortalityLogDto updateDto);
        Task<Result> DeleteMortalityLogAsync(Guid id);
    }
}
