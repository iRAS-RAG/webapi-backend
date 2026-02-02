using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
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
                    SpeciesName = s.Species.Name,
                    GrowthStageName = s.GrowthStage.Name,
                    FeedTypeName = s.FeedType.Name,
                    AmountPer100Fish = s.AmountPer100Fish,
                    FrequencyPerDay = s.FrequencyPerDay,
                    MaxStockingDensity = s.MaxStockingDensity,
                    ExpectedDurationDays = s.ExpectedDurationDays,
                });
        }
    }

    public class SpeciesStageConfigListSpec
        : Specification<SpeciesStageConfig, SpeciesStageConfigDto>
    {
        public SpeciesStageConfigListSpec()
        {
            Query
                .AsNoTracking()
                .Select(s => new SpeciesStageConfigDto
                {
                    Id = s.Id,
                    SpeciesName = s.Species.Name,
                    GrowthStageName = s.GrowthStage.Name,
                    FeedTypeName = s.FeedType.Name,
                    AmountPer100Fish = s.AmountPer100Fish,
                    FrequencyPerDay = s.FrequencyPerDay,
                    MaxStockingDensity = s.MaxStockingDensity,
                    ExpectedDurationDays = s.ExpectedDurationDays,
                });
        }
    }
}
