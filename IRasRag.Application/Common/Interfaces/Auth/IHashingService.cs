namespace IRasRag.Application.Common.Interfaces.Auth
{
    public interface IHashingService
    {
        string Hash(string value);
        bool VerifyHash(string value, string hashedValue);
    }
}
