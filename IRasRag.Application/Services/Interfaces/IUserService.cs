using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedResult<UserDto>> GetAllUsersAsync(int page, int pageSize);
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto createDto);
        Task<Result> UpdateUserAsync(Guid id, UpdateUserDto updateDto);
        Task<Result> DeleteUserAsync(Guid id);
    }
}
