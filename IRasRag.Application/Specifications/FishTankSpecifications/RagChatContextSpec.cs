using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    public class RagChatContextSpec : Specification<FishTank, RagChatContext>
    {
        public RagChatContextSpec(Guid tankId)
        {
            Query.AsNoTracking()
                .Where(t => t.Id == tankId)
                .Select(t => t.FarmingBatches
                    .Where(fb =>
                        fb.Status == FarmingBatchStatus.ACTIVE &&
                        fb.CurrentStageConfig != null)
                    .Select(fb => new RagChatContext
                    {
                        SpeciesName = fb.CurrentStageConfig!.Species.Name,
                        StageName = fb.CurrentStageConfig!.GrowthStage.Name
                    })
                    .FirstOrDefault() ?? new RagChatContext
                    {
                        SpeciesName = "unknown",
                        StageName = "unknown"
                    });
        }
    }
}
