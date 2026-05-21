using IRasRag.Application.Common.Models.Telemetry;

namespace IRasRag.Application.Common.Interfaces.Telemetry
{
    public interface IAlertStateCacheService
    {
        void Set(Guid fishTankId, Guid sensorTypeId, Guid? farmingBatchId, AlertState state);
        Task<AlertState?> Get(Guid fishTankId, Guid sensorTypeId, Guid? farmingBatchId);
        void Invalidate(Guid fishTankId, Guid sensorTypeId, Guid? farmingBatchId);
    }
}
