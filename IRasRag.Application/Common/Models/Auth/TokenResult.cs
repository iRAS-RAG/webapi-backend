namespace IRasRag.Application.Common.Models.Auth
{
    public sealed record RefreshTokenResult(string PlainToken, DateTime ExpireDate);
}
