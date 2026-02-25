using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.UserFarmSpecifications
{
    public class UserFarmDtoListSpec : BaseListSpec<UserFarm, UserFarmDto>
    {
        public UserFarmDtoListSpec(UserFarmListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<UserFarm, object?>>>
            {
                ["useremail"] = uf => uf.User.Email,
                ["farmname"] = uf => uf.Farm.Name,
                ["createdat"] = uf => uf.CreatedAt ?? DateTime.MinValue,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    uf => uf.User.Email,
                    uf => uf.User.FirstName,
                    uf => uf.User.LastName,
                    uf => uf.Farm.Name,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "createdat");

            Query.Select(uf => new UserFarmDto
            {
                Id = uf.Id,
                UserId = uf.UserId,
                UserEmail = uf.User.Email,
                UserFullName = uf.User.FirstName + " " + uf.User.LastName,
                FarmId = uf.FarmId,
                FarmName = uf.Farm.Name,
                CreatedAt = uf.CreatedAt,
                ModifiedAt = uf.ModifiedAt,
            });
        }
    }
}
