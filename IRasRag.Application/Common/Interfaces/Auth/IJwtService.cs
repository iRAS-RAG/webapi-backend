using IRasRag.Application.Common.Models.Auth;

namespace IRasRag.Application.Common.Interfaces.Auth
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid id, string username, string roleName);
        RefreshTokenResult GenerateRefreshToken();
    }
}
