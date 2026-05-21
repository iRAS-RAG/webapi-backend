using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    public class UserAllowedFishTankSpec : Specification<FishTank, Guid>
    {
        public UserAllowedFishTankSpec(Guid userId, Guid? farmId, Guid? batchId)
        {
            Query.AsNoTracking();

            Query.Where(ft => ft.Farm.UserFarms.Any(uf => uf.UserId == userId));

            if (farmId.HasValue)
                Query.Where(ft => ft.FarmId == farmId.Value);

            if (batchId.HasValue)
                Query.Where(ft => ft.FarmingBatches.Any(b => b.Id == batchId.Value));

            Query.Select(ft => ft.Id);
        }
    }
}
