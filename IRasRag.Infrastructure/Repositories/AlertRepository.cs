using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Repositories
{
    public class AlertRepository : Repository<Alert>, IAlertRepository
    {
        public AlertRepository(AppDbContext context)
            : base(context) { }

        public async Task<Alert?> GetLatestActiveAlertByScope(Guid tankId, Guid sensorTypeId, Guid? batchId)
        {
            return await GetQueryable()
               .Where(a => 
                   (batchId.HasValue ? a.FarmingBatchId == batchId.Value : a.FarmingBatchId == null) &&
                   a.FishTankId == tankId &&
                   a.SensorTypeId == sensorTypeId &&
                   (a.Status == AlertStatus.OPEN || a.Status == AlertStatus.ACKNOWLEDGED))
               .OrderByDescending(a => a.RaisedAt)
               .FirstOrDefaultAsync();
        }
    }
}
