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
                    Sequence = s.Sequence,
                    FeedTypeIds = s.FeedTypes.Select(ft => ft.Id).ToList(),
                    FeedTypeNames = s.FeedTypes.Select(ft => ft.Name).ToList(),
                    AmountPer100Fish = s.AmountPer100Fish,
                    FrequencyPerDay = s.FrequencyPerDay,
                    MaxStockingDensity = s.MaxStockingDensity,
                    ExpectedDurationDays = s.ExpectedDurationDays,
                    ExpectedWeightKgPerFish = s.ExpectedWeightKgPerFish,
                    SurvivalRate = s.SurvivalRate,
                });
        }
    }
}
