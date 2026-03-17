namespace IRasRag.Application.Common.Interfaces.Persistence.Repositories
{
    public interface IMoralityLogRepository
    {
        Task<int> ComputeDeathCountByBatch(Guid batchId);
    }
}
