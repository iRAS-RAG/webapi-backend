namespace IRasRag.Application.Services.Interfaces
{
    public interface IUserTankAccessService
    {
        Task<HashSet<Guid>> GetAllowedTankIdsAsync(Guid userId);
        Task<bool> CanAccessTankAsync(Guid userId, Guid tankId);
    }
}
