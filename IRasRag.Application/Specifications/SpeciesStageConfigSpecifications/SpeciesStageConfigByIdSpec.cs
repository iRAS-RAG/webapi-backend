using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesStageConfigSpecifications
{
    public class SpeciesStageConfigByIdSpec
        : Specification<SpeciesStageConfig, SpeciesStageConfigDto>
    {
        public SpeciesStageConfigByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SpeciesStageConfigDto
                {
                    Id = s.Id,
                    SpeciesId = s.SpeciesId,
                    SpeciesName = s.Species.Name,
                    GrowthStageId = s.GrowthStageId,
                    GrowthStageName = s.GrowthStage.Name,
                    FeedTypeId = s.FeedTypeId,
                    FeedTypeName = s.FeedType.Name,
                    AmountPer100Fish = s.AmountPer100Fish,
                    FrequencyPerDay = s.FrequencyPerDay,
                    MaxStockingDensity = s.MaxStockingDensity,
                    ExpectedDurationDays = s.ExpectedDurationDays,
                });
        }
    }
}
