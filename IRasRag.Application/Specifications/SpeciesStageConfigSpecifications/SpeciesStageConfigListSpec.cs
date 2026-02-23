using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesStageConfigSpecifications
{
    public class SpeciesStageConfigListSpec : BaseListSpec<SpeciesStageConfig, SpeciesStageConfigDto>
    {
        public SpeciesStageConfigListSpec(SpeciesStageConfigListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<SpeciesStageConfig, object?>>>
            {
                ["speciesname"] = s => s.Species.Name,
                ["growthstagename"] = s => s.GrowthStage.Name,
                ["feedtypename"] = s => s.FeedType.Name,
            };

            ApplySearch(request.SearchTerm,
                [
                    s => s.Species.Name,
                    s => s.GrowthStage.Name,
                    s => s.FeedType.Name,
                ]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "speciesname");

            Query.Select(s => new SpeciesStageConfigDto
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
