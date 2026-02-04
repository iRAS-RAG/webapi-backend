using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;

namespace IRasRag.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenResponse>> Login(LoginRequest request);
        Task<Result> Logout(string refreshToken);
        Task<Result> RequestPasswordReset(string email);
        Task<Result> ResetPassword(ResetPasswordRequest request);
        Task<Result<TokenResponse>> RefreshBothToken(string refreshToken);
    }
}
