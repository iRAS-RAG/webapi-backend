using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FarmSpecifications
{
    public class FarmListDtoSpec : BaseListSpec<Farm, FarmDto>
    {
        public FarmListDtoSpec(FarmListRequest request)
        {
            Query.AsNoTracking();

            ApplyFilter(
                request.UserId,
                f => f.UserFarms.Any(uf => uf.UserId == request.UserId!.Value)
            );

            var sortMap = new Dictionary<string, Expression<Func<Farm, object?>>>
            {
                ["name"] = f => f.Name,
                ["address"] = f => f.Address,
                ["email"] = f => f.Email,
            };

            ApplySearch(request.SearchTerm, [f => f.Name, f => f.Address, f => f.Email]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(f => new FarmDto
            {
                Id = f.Id,
                Name = f.Name,
                Address = f.Address,
                PhoneNumber = f.PhoneNumber,
                Email = f.Email,
            });
        }
    }
}
