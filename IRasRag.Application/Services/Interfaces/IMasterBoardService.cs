using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IMasterBoardService
    {
        Task<Result<IEnumerable<MasterBoardDto>>> GetAllMasterBoardsAsync();
        Task<Result<MasterBoardDto>> GetMasterBoardByIdAsync(Guid id);
        Task<Result<MasterBoardDto>> CreateMasterBoardAsync(CreateMasterBoardDto createDto);
        Task<Result> UpdateMasterBoardAsync(Guid id, UpdateMasterBoardDto updateDto);
        Task<Result> DeleteMasterBoardAsync(Guid id);
    }
}
