using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IRasRag.Infrastructure.Services.Telemetry
{
    public class TelemetryCacheService : ITelemetryCacheService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TelemetryCacheService> _logger;

        private static readonly TimeSpan LongTtl = TimeSpan.FromHours(24);
        private static readonly TimeSpan MediumTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan NullTtl = TimeSpan.FromMinutes(1);

        public TelemetryCacheService(
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            ILogger<TelemetryCacheService> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<FarmingBatch?> GetActiveBatchAsync(Guid tankId)
        {
            var key = $"batch:active:{tankId}";

            if (_cache.TryGetValue(key, out FarmingBatch? cached))
                return cached;

            var batch = await _unitOfWork.GetRepository<FarmingBatch>()
                .FirstOrDefaultAsync(b => b.FishTankId == tankId && b.Status == FarmingBatchStatus.ACTIVE);

            if (batch == null)
            {
                _logger.LogInformation("No active farming batch for tank {TankId}", tankId);
                _cache.Set(key, (FarmingBatch?)null, NullTtl);
                return null;
            }

            _cache.Set(key, batch, MediumTtl);
            return batch;
        }

        public async Task<MasterBoard?> GetMasterboardByMacAsync(string mac)
        {
            var key = $"masterboard:mac:{mac}";

            if (_cache.TryGetValue(key, out MasterBoard? cached))
                return cached;

            var masterboard = await _unitOfWork.GetRepository<MasterBoard>()
                .FirstOrDefaultAsync(m => m.MacAddress == mac);

            if (masterboard == null)
            {
                _logger.LogWarning("No masterboard found for MAC {Mac}", mac);
                _cache.Set(key, (MasterBoard?)null, NullTtl);
                return null;
            }

            _cache.Set(key, masterboard, LongTtl);
            return masterboard;
        }

        public async Task<Dictionary<int, Sensor>> GetSensorsByMasterboardAsync(Guid masterboardId)
        {
            var key = $"sensors:masterboard:{masterboardId}";

            if (_cache.TryGetValue(key, out Dictionary<int, Sensor>? cached) && cached != null)
                return cached;

            var sensors = await _unitOfWork.GetRepository<Sensor>()
                .FindAllAsync(s => s.MasterBoardId == masterboardId);

            var dict = sensors.ToDictionary(s => s.PinCode, s => s);

            _cache.Set(key, dict, LongTtl);
            return dict;
        }

        public async Task<Dictionary<Guid, SpeciesThreshold>> GetThresholdsAsync(Guid speciesId, Guid growthStageId)
        {
            var key = $"thresholds:{speciesId}:{growthStageId}";

            if (_cache.TryGetValue(key, out Dictionary<Guid, SpeciesThreshold>? cached) && cached != null)
                return cached;

            var thresholds = await _unitOfWork.GetRepository<SpeciesThreshold>()
                .FindAllAsync(t => t.SpeciesId == speciesId && t.GrowthStageId == growthStageId);

            var dict = thresholds.ToDictionary(t => t.SensorTypeId, t => t);

            _cache.Set(key, dict, MediumTtl);
            return dict;
        }

        public async Task<SpeciesStageConfig?> GetStageConfigAsync(Guid stageConfigId)
        {
            var key = $"stageconfig:{stageConfigId}";

            if (_cache.TryGetValue(key, out SpeciesStageConfig? cached))
                return cached;

            var config = await _unitOfWork.GetRepository<SpeciesStageConfig>()
                .FirstOrDefaultAsync(c => c.Id == stageConfigId);

            if (config == null)
            {
                _logger.LogWarning("No SpeciesStageConfig found for id {StageConfigId}", stageConfigId);
                _cache.Set(key, (SpeciesStageConfig?)null, NullTtl);
                return null;
            }

            _cache.Set(key, config, MediumTtl);
            return config;
        }

        public void InvalidateBatch(Guid tankId)
        {
            var key = $"batch:active:{tankId}";
            _cache.Remove(key);
            _logger.LogInformation("Invalidated active batch cache for tank {TankId}", tankId);
        }

        public void InvalidateStageConfig(Guid stageConfigId)
        {
            var key = $"stageconfig:{stageConfigId}";
            _cache.Remove(key);
            _logger.LogInformation("Invalidated stage config cache for id {StageConfigId}", stageConfigId);
        }

        public void InvalidateMasterboard(string mac)
        {
            var key = $"masterboard:mac:{mac}";
            _cache.Remove(key);
            _logger.LogInformation("Invalidated masterboard cache for MAC {Mac}", mac);
        }

        public void InvalidateSensors(Guid masterboardId)
        {
            var key = $"sensors:masterboard:{masterboardId}";
            _cache.Remove(key);
            _logger.LogInformation("Invalidated sensors cache for masterboard {MasterboardId}", masterboardId);
        }

        public void InvalidateThresholds(Guid speciesId, Guid growthStageId)
        {
            var key = $"thresholds:{speciesId}:{growthStageId}";
            _cache.Remove(key);
            _logger.LogInformation("Invalidated thresholds cache for species {SpeciesId} stage {GrowthStageId}", speciesId, growthStageId);
        }
    }
}
