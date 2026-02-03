using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<string>> Login(LoginRequest request);
        Task<Result<string>> Register(RegisterRequest request);
    }
}
