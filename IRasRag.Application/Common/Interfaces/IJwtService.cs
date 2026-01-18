namespace IRasRag.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid id, string username, string roleName);
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
        bool VerifyRefreshToken(string refreshToken, string hashedRefreshToken);
    }
}
