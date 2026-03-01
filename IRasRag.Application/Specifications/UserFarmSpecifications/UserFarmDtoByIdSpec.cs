using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.UserFarmSpecifications
{
    /// <summary>
    /// Specification chiếu một UserFarm theo Id thành UserFarmDto,
    /// bao gồm cả thông tin User và Farm.
    /// </summary>
    public class UserFarmDtoByIdSpec : Specification<UserFarm, UserFarmDto>
    {
        public UserFarmDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(uf => uf.Id == id)
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
