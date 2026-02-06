using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Common.Interfaces
{
    public interface IUserFarmService
    {
        Task<PaginatedResult<UserFarmDto>> GetAllUserFarmsAsync(int page, int pageSize);
        Task<Result<UserFarmDto>> GetUserFarmByIdAsync(Guid id);
        Task<Result<UserFarmDto>> CreateUserFarmAsync(CreateUserFarmDto createDto);
        Task<Result> UpdateUserFarmAsync(Guid id, UpdateUserFarmDto updateDto);
        Task<Result> DeleteUserFarmAsync(Guid id);
    }
}
