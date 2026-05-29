using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.Metrics
{
    public class MortalityLogByFarmAndRangeSpec : Specification<MortalityLog>
    {
        public MortalityLogByFarmAndRangeSpec(Guid farmId, DateTime start, DateTime end)
        {
            Query
                .AsNoTracking()
                .Where(ml =>
                    ml.Date >= start && ml.Date <= end && ml.Batch.FishTank.FarmId == farmId
                )
                .Include(ml => ml.Batch)
                .Include(ml => ml.Batch.FishTank);
        }
    }
}
