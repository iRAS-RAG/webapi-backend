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

            if (request.SpeciesId.HasValue)
                Query.Where(st => st.SpeciesId == request.SpeciesId.Value);

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
                SpeciesId = st.SpeciesId,
                SpeciesName = st.Species.Name,
                GrowthStageId = st.GrowthStageId,
                GrowthStageName = st.GrowthStage.Name,
                SensorTypeId = st.SensorTypeId,
                SensorTypeName = st.SensorType.Name,
                MinValue = st.MinValue,
                MaxValue = st.MaxValue,
                UnitOfMeasure = st.SensorType.UnitOfMeasure,
            });
        }
    }
}
