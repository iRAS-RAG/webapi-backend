using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IRasRag.Infrastructure.Repositories
{
    public class MoralityLogRepository : Repository<MortalityLog>, IMoralityLogRepository
    {
        public MoralityLogRepository(AppDbContext context)
            : base(context) { }

        public async Task<int> ComputeDeathCountByBatch(Guid batchId)
        {
            return await GetQueryable()
                .Where(log => log.BatchId == batchId)
                .SumAsync(log => log.Quantity);
        }
    }
}
