using IRasRag.Application.Common.Interfaces.Auth;

namespace IRasRag.Infrastructure.Services.Auth
{
    public class BCryptHashingService : IHashingService
    {
        public string Hash(string value)
        {
            return BCrypt.Net.BCrypt.HashPassword(value);
        }

        public bool VerifyHash(string value, string hashedValue)
        {
            return BCrypt.Net.BCrypt.Verify(value, hashedValue);
        }
    }
}
