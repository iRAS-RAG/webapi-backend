using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FarmingBatchSpecifications
{
    public class FarmingBatchByUserAccessSpec : Specification<FarmingBatch>
    {
        public FarmingBatchByUserAccessSpec(Guid batchId, Guid userId)
        {
            Query
                .AsNoTracking()
                .Where(fb => fb.Id == batchId && fb.FishTank.Farm.UserFarms.Any(uf => uf.UserId == userId));
        }
    }
}
