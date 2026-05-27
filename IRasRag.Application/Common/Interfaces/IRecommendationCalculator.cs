namespace IRasRag.Application.Common.Interfaces
{
    public interface IRecommendationCalculator
    {
        Task<int?> GetRecommendedInitialAsync(Guid fishTankId, Guid speciesId);
    }
}
