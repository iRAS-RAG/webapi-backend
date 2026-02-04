using System.Security.Cryptography;
using System.Text;
using IRasRag.Application.Common.Interfaces.Auth;
using IRasRag.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IRasRag.Infrastructure.Services.Auth
{
    public class HashingService : IHashingService
    {
        private readonly JwtSettings _jwtSettings;

        public HashingService(IOptions<JwtSettings> settings)
        {
            _jwtSettings = settings.Value;
        }

        public string HashPassword(string value)
        {
            return BCrypt.Net.BCrypt.HashPassword(value);
        }

        public string HashToken(string value)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var tokenBytes = Encoding.UTF8.GetBytes(value);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(tokenBytes);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string value, string hashedValue)
        {
            return BCrypt.Net.BCrypt.Verify(value, hashedValue);
        }

        public bool VerifyToken(string value, string hashedValue)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var tokenBytes = Encoding.UTF8.GetBytes(value);
            var storedBytes = Convert.FromBase64String(hashedValue);

            using var hmac = new HMACSHA256(keyBytes);
            var computedHash = hmac.ComputeHash(tokenBytes);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedBytes);
        }
    }
}
