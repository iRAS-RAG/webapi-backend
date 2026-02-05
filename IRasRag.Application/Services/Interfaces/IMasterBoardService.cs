using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IMasterBoardService
    {
        Task<PaginatedResult<MasterBoardDto>> GetAllMasterBoardsAsync(int page, int pageSize);
        Task<Result<MasterBoardDto>> GetMasterBoardByIdAsync(Guid id);
        Task<Result<MasterBoardDto>> CreateMasterBoardAsync(CreateMasterBoardDto createDto);
        Task<Result> UpdateMasterBoardAsync(Guid id, UpdateMasterBoardDto updateDto);
        Task<Result> DeleteMasterBoardAsync(Guid id);
    }
}
