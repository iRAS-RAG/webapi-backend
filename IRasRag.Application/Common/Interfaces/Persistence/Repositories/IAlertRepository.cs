using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Interfaces.Persistence.Repositories
{
    public interface IAlertRepository
    {
        Task<Alert?> GetLatestActiveAlertByScope(
            Guid tankId,
            Guid sensorTypeId,
            Guid? farmingBatchId
        );
    }
}
