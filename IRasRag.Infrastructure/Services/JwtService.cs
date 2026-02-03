using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IRasRag.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;

        public JwtService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
        }

        public string GenerateAccessToken(Guid id, string username, string roleName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, id.ToString()),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role, roleName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = creds,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string HashRefreshToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyRefreshToken(string providedToken, string storedHash)
        {
            if (string.IsNullOrEmpty(providedToken) || string.IsNullOrEmpty(storedHash))
                return false;
            var providedHash = HashRefreshToken(providedToken);
            // Use constant-time comparison
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(providedHash),
                Convert.FromBase64String(storedHash)
            );
        }
    }
}
