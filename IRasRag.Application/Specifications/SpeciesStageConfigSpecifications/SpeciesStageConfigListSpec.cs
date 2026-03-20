using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesStageConfigSpecifications
{
    public class SpeciesStageConfigListSpec
        : BaseListSpec<SpeciesStageConfig, SpeciesStageConfigDto>
    {
        public SpeciesStageConfigListSpec(SpeciesStageConfigListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<SpeciesStageConfig, object?>>>
            {
                ["speciesname"] = s => s.Species.Name,
                ["growthstagename"] = s => s.GrowthStage.Name,
                ["feedtypename"] = s =>
                    s.FeedTypes.OrderBy(ft => ft.Name).Select(ft => ft.Name).FirstOrDefault(),
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

            Query.Select(s => new SpeciesStageConfigDto
            {
                Id = s.Id,
                SpeciesId = s.SpeciesId,
                SpeciesName = s.Species.Name,
                GrowthStageId = s.GrowthStageId,
                GrowthStageName = s.GrowthStage.Name,
                FeedTypeIds = s.FeedTypes.Select(ft => ft.Id).ToList(),
                FeedTypeNames = s.FeedTypes.Select(ft => ft.Name).ToList(),
                AmountPer100Fish = s.AmountPer100Fish,
                FrequencyPerDay = s.FrequencyPerDay,
                MaxStockingDensity = s.MaxStockingDensity,
                ExpectedDurationDays = s.ExpectedDurationDays,
            });
        }
    }
}
