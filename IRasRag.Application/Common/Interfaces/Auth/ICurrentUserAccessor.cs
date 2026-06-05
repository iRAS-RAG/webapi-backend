namespace IRasRag.Application.Common.Interfaces.Auth
{
    public interface ICurrentUserAccessor
    {
        Guid? GetUserId();
    }
}
