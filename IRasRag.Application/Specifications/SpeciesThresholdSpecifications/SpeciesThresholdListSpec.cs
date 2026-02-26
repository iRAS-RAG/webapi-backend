using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesThresholdSpecifications
{
    public class SpeciesThresholdListSpec : BaseListSpec<SpeciesThreshold, SpeciesThresholdDto>
    {
        public SpeciesThresholdListSpec(SpeciesThresholdListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<SpeciesThreshold, object?>>>
            {
                ["speciesname"] = st => st.Species.Name,
                ["growthstagename"] = st => st.GrowthStage.Name,
                ["sensortypename"] = st => st.SensorType.Name,
            };

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "speciesname");

            ApplySearch(
                request.SearchTerm,
                [st => st.Species.Name, st => st.GrowthStage.Name, st => st.SensorType.Name]
            );

            Query.Select(st => new SpeciesThresholdDto
            {
                Id = st.Id,
                SpeciesName = st.Species.Name,
                GrowthStageName = st.GrowthStage.Name,
                Low = st.MinValue,
                High = st.MaxValue,
                Unit = st.SensorType.Name,
            });
        }
    }
}
