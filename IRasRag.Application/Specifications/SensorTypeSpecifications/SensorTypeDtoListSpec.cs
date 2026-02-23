using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SensorTypeSpecifications
{
    public class SensorTypeDtoListSpec : BaseListSpec<SensorType, SensorTypeDto>
    {
        public SensorTypeDtoListSpec(SensorTypeListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<SensorType, object?>>>
            {
                ["name"] = st => st.Name,
                ["measuretype"] = st => st.MeasureType,
                ["unitofmeasure"] = st => st.UnitOfMeasure,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    st => st.Name,
                    st => st.MeasureType,
                    st => st.UnitOfMeasure,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(st => new SensorTypeDto
            {
                Id = st.Id,
                Name = st.Name,
                MeasureType = st.MeasureType,
                UnitOfMeasure = st.UnitOfMeasure,
            });
        }
    }
}
