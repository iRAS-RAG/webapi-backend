using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Interfaces.Telemetry;
using IRasRag.Application.Common.Models.Telemetry;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace IRasRag.Infrastructure.Services.Telemetry
{
    public class AlertStateCacheService : IAlertStateCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;

        public AlertStateCacheService(IMemoryCache cache, IServiceScopeFactory scopeFactory)
        {
            _cache = cache;
            _scopeFactory = scopeFactory;
        }

        public void Set(Guid fishTankId, Guid sensorTypeId, Guid? farmingBatchId, AlertState state)
        {
            var key = $"alert:state:{fishTankId}:{sensorTypeId}:{farmingBatchId}";
            _cache.Set(key, state, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }

        public async Task<AlertState?> Get(Guid fishTankId, Guid sensorTypeId, Guid? farmingBatchId)
        {
            var key = $"alert:state:{fishTankId}:{sensorTypeId}:{farmingBatchId}";
            if (_cache.TryGetValue(key, out AlertState? state))
                return state;

            using var scope = _scopeFactory.CreateScope();
            var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertRepository>();
            var alert = await alertRepo.GetLatestActiveAlertByScope(fishTankId, sensorTypeId, farmingBatchId);

            if (alert != null)
            {
                var existState = new AlertState
                {
                    ActiveAlertId = alert.Id,
                    LastValue = alert.TriggerValue

                };
                Set(fishTankId, sensorTypeId, farmingBatchId, existState);
                return existState;
            }

            return null;
        }

        public void Invalidate(Guid fishTankId, Guid sensorTypeId, Guid? farmingBatchId)
        {
            var key = $"alert:state:{fishTankId}:{sensorTypeId}:{farmingBatchId}";
            _cache.Remove(key);
        }
    }
}
