using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.UserSpecifications
{
    public class UserDtoListSpec : BaseListSpec<User, UserDto>
    {
        public UserDtoListSpec(UserListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<User, object?>>>
            {
                ["email"] = u => u.Email,
                ["firstname"] = u => u.FirstName,
                ["lastname"] = u => u.LastName,
                ["rolename"] = u => u.Role.Name,
                ["isdeleted"] = u => u.IsDeleted,
            };

            ApplySearch(
                request.SearchTerm,
                [u => u.Email, u => u.FirstName, u => u.LastName, u => u.Role.Name]
            );

            ApplyFilter(request.IsDeleted, u => u.IsDeleted == request.IsDeleted);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "email");

            Query.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                RoleName = u.Role.Name,
                IsDeleted = u.IsDeleted,
            });
        }
    }
}
