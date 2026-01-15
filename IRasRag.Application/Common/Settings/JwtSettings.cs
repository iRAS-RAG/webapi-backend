namespace IRasRag.Application.Common.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }

        public void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
                throw new InvalidOperationException(
                    "SecretKey is missing or empty");

            // HMAC SHA256 needs a sufficiently long key
            if (SecretKey.Length < 32)
                throw new InvalidOperationException(
                    "SecretKey must be at least 32 characters long");

            if (string.IsNullOrWhiteSpace(Issuer))
                throw new InvalidOperationException(
                    "Issuer is missing or empty");

            if (string.IsNullOrWhiteSpace(Audience))
                throw new InvalidOperationException(
                    "Audience is missing or empty");

            if (AccessTokenExpirationMinutes <= 0)
                throw new InvalidOperationException(
                    "AccessTokenExpirationMinutes must be greater than 0");

            if (RefreshTokenExpirationDays <= 0)
                throw new InvalidOperationException(
                    "RefreshTokenExpirationDays must be greater than 0");
        }
    }
}
