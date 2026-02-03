namespace IRasRag.Application.Common.Interfaces.Auth
{
    public interface IHashingService
    {
        string HashPassword(string value);
        bool VerifyPassword(string value, string hashedValue);
        string HashToken(string value);
        bool VerifyToken(string value, string hashedValue);
    }
}
