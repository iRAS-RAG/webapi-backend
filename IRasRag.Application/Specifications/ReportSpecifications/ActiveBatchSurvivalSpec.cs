using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.ReportSpecifications
{
    /// <summary>
    /// Projects only InitialQuantity and CurrentQuantity from ACTIVE FarmingBatch rows
    /// belonging to the given fish tanks.
    /// Used by the dashboard to compute survival rate without loading full entities.
    /// </summary>
    public class ActiveBatchSurvivalSpec : Specification<FarmingBatch, BatchSurvivalProjection>
    {
        public ActiveBatchSurvivalSpec(IEnumerable<Guid> tankIds)
        {
            Query
                .AsNoTracking()
                .Where(b => b.Status == FarmingBatchStatus.ACTIVE && tankIds.Contains(b.FishTankId))
                .Select(b => new BatchSurvivalProjection
                {
                    InitialQuantity = b.InitialQuantity,
                    CurrentQuantity = b.CurrentQuantity,
                });
        }
    }
}
