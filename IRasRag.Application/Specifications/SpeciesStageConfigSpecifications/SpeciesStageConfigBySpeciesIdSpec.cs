using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;
using System.Linq.Expressions;

namespace IRasRag.Application.Specifications.SpeciesStageConfigSpecifications
{
    public class SpeciesStageConfigBySpeciesIdSpec : BaseListSpec<SpeciesStageConfig, SpeciesStageConfigDto>
    {
        public SpeciesStageConfigBySpeciesIdSpec(Guid speciesId, SpeciesStageConfigListRequest request)
        {
            Query.AsNoTracking();
            var sortMap = new Dictionary<string, Expression<Func<SpeciesStageConfig, object?>>>
            {
                ["speciesname"] = s => s.Species.Name,
                ["growthstagename"] = s => s.GrowthStage.Name,
                ["feedtypename"] = s => s.FeedType.Name,
            };

            ApplySearch(
                request.SearchTerm, [s => s.Species.Name, s => s.GrowthStage.Name, s => s.FeedType.Name]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "speciesname");

            Query.Where(config => config.SpeciesId == speciesId)
                .Select(config => new SpeciesStageConfigDto
                {
                    Id = config.Id,
                    SpeciesId = config.SpeciesId,
                    SpeciesName = config.Species.Name,
                    GrowthStageId = config.GrowthStageId,
                    GrowthStageName = config.GrowthStage.Name,
                    FeedTypeId = config.FeedTypeId,
                    FeedTypeName = config.FeedType.Name,
                    AmountPer100Fish = config.AmountPer100Fish,
                    FrequencyPerDay = config.FrequencyPerDay,
                    MaxStockingDensity = config.MaxStockingDensity,
                    ExpectedDurationDays = config.ExpectedDurationDays
                });
        }
    }
}
