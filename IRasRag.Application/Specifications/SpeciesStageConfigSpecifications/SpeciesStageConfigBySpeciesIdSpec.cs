using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesStageConfigSpecifications
{
    public class SpeciesStageConfigBySpeciesIdSpec
        : BaseListSpec<SpeciesStageConfig, SpeciesStageConfigDto>
    {
        public SpeciesStageConfigBySpeciesIdSpec(
            Guid speciesId,
            SpeciesStageConfigListRequest request
        )
        {
            Query.AsNoTracking();
            var sortMap = new Dictionary<string, Expression<Func<SpeciesStageConfig, object?>>>
            {
                ["speciesname"] = s => s.Species.Name,
                ["growthstagename"] = s => s.GrowthStage.Name,
                ["feedtypename"] = s =>
                    s.FeedTypes.OrderBy(ft => ft.Name).Select(ft => ft.Name).FirstOrDefault(),
                ["sequence"] = s => s.Sequence,
            };

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                Query.Where(s =>
                    s.Species.Name.ToLower().Contains(term)
                    || s.GrowthStage.Name.ToLower().Contains(term)
                    || s.FeedTypes.Any(ft => ft.Name.ToLower().Contains(term))
                );
            }

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "speciesname");

            Query
                .Where(config => config.SpeciesId == speciesId)
                .Select(config => new SpeciesStageConfigDto
                {
                    Id = config.Id,
                    SpeciesId = config.SpeciesId,
                    SpeciesName = config.Species.Name,
                    GrowthStageId = config.GrowthStageId,
                    GrowthStageName = config.GrowthStage.Name,
                    Sequence = config.Sequence,
                    FeedTypeIds = config.FeedTypes.Select(ft => ft.Id).ToList(),
                    FeedTypeNames = config.FeedTypes.Select(ft => ft.Name).ToList(),
                    AmountPer100Fish = config.AmountPer100Fish,
                    FrequencyPerDay = config.FrequencyPerDay,
                    MaxStockingDensity = config.MaxStockingDensity,
                    ExpectedDurationDays = config.ExpectedDurationDays,
                });
        }
    }
}
