using IRasRag.Domain.Entities;

namespace IRasRag.Application.Common.Interfaces.Telemetry
{
    public interface ITelemetryCacheService
    {
        Task<MasterBoard?> GetMasterboardByMacAsync(string mac);
        Task<FarmingBatch?> GetActiveBatchAsync(Guid tankId);
        Task<Dictionary<int, Sensor>> GetSensorsByMasterboardAsync(Guid masterboardId);
        Task<Dictionary<Guid, SpeciesThreshold>> GetThresholdsAsync(Guid speciesId, Guid growthStageId);
        Task<SpeciesStageConfig?> GetStageConfigAsync(Guid stageConfigId);
        void InvalidateBatch(Guid tankId);
        void InvalidateStageConfig(Guid stageConfigId);
    }
}
