using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class UserListSpec : Specification<User>
    {
        public UserListSpec()
        {
            Query.AsNoTracking().Include(u => u.Role);
        }
    }

    public class UserDtoListSpec : Specification<User, UserDto>
    {
        public UserDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(u => u.Role)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleName = u.Role.Name,
                });
        }
    }
}
