using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SensorSpecifications
{
    public sealed class SensorsByMasterboardSpec : Specification<Sensor>
    {
        public SensorsByMasterboardSpec(Guid masterboardId)
        {
            Query
                .AsNoTracking()
                .Where(s => s.MasterBoardId == masterboardId)
                .Include(s => s.SensorType);
        }
    }
}
