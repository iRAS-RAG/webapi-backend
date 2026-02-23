using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FeedTypeSpecifications
{
    public class FeedTypeDtoListSpec : BaseListSpec<FeedType, FeedTypeDto>
    {
        public FeedTypeDtoListSpec(FeedTypeListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<FeedType, object?>>>
            {
                ["name"] = ft => ft.Name,
                ["proteinpercentage"] = ft => ft.ProteinPercentage,
                ["manufacturer"] = ft => ft.Manufacturer,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    ft => ft.Name,
                    ft => ft.Description,
                    ft => ft.Manufacturer,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(ft => new FeedTypeDto
            {
                Id = ft.Id,
                Name = ft.Name,
                Description = ft.Description,
                ProteinPercentage = ft.ProteinPercentage,
                Manufacturer = ft.Manufacturer,
            });
        }
    }
}
