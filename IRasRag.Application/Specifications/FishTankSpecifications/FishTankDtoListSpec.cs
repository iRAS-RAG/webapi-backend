using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    public class FishTankDtoListSpec : BaseListSpec<FishTank, FishTankDto>
    {
        public FishTankDtoListSpec(FishTankListRequest request)
        {
            Query.AsNoTracking();

            ApplyFilter(request.FarmId, ft => ft.FarmId == request.FarmId!.Value);

            var sortMap = new Dictionary<string, Expression<Func<FishTank, object?>>>
            {
                ["name"] = ft => ft.Name,
                ["volume"] = ft => ft.Height * ft.Radius * ft.Radius,
            };

            ApplySearch(request.SearchTerm, [ft => ft.Name, ft => ft.Farm.Name, ft => ft.Name]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(ft => new FishTankDto
            {
                Id = ft.Id,
                Name = ft.Name,
                Volume = (float)(Math.PI * ft.Radius * ft.Radius * ft.Height),
                FarmId = ft.FarmId,
                FarmName = ft.Farm.Name,
                TopicCode = ft.TopicCode,
                CameraUrl = ft.CameraUrl,
            });
        }
    }
}
