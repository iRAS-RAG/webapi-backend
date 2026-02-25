using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.GrowthStageSpecifications
{
    public class GrowthStageDtoListSpec : BaseListSpec<GrowthStage, GrowthStageDto>
    {
        public GrowthStageDtoListSpec(GrowthStageListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<GrowthStage, object?>>>
            {
                ["name"] = gs => gs.Name,
            };

            ApplySearch(request.SearchTerm, [gs => gs.Name, gs => gs.Description]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(gs => new GrowthStageDto
            {
                Id = gs.Id,
                Name = gs.Name,
                Description = gs.Description,
            });
        }
    }
}
