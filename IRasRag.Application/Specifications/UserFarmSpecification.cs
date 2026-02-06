using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class UserFarmDtoListSpec : Specification<UserFarm, UserFarmDto>
    {
        public UserFarmDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(uf => uf.User)
                .Include(uf => uf.Farm)
                .Select(uf => new UserFarmDto
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
