using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto createDto);
        Task<Result> UpdateUserAsync(Guid id, UpdateUserDto updateDto);
        Task<Result> DeleteUserAsync(Guid id);
    }
}
