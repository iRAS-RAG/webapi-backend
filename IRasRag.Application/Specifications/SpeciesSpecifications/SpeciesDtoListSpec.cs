using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SpeciesSpecifications
{
    public class SpeciesDtoListSpec : BaseListSpec<Species, SpeciesDto>
    {
        public SpeciesDtoListSpec(SpeciesListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<Species, object?>>>
            {
                ["name"] = s => s.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    s => s.Name,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(s => new SpeciesDto
            {
                Id = s.Id,
                Name = s.Name,
            });
        }
    }
}
