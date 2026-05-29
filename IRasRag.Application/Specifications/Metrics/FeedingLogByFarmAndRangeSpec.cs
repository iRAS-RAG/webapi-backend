using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.Metrics
{
    public class FeedingLogByFarmAndRangeSpec : Specification<FeedingLog>
    {
        public FeedingLogByFarmAndRangeSpec(Guid farmId, DateTime start, DateTime end)
        {
            Query
                .AsNoTracking()
                .Where(fl =>
                    fl.CreatedDate >= start
                    && fl.CreatedDate <= end
                    && fl.FarmingBatch.FishTank.FarmId == farmId
                )
                .Include(fl => fl.FarmingBatch)
                .Include(fl => fl.FarmingBatch.FishTank);
        }
    }
}
