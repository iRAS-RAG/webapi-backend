using Ardalis.Specification;
using IRasRag.Domain.Entities;

public sealed class MasterboardByMacSpec : Specification<MasterBoard>
{
    public MasterboardByMacSpec(string mac)
    {
        Query
            .AsNoTracking()
            .Where(m => m.MacAddress == mac)
            .Include(m => m.FishTank);
    }
}
