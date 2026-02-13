using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class FarmListDtoByUserSpec : Specification<Farm, FarmDto>
    {
        public FarmListDtoByUserSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(f => f.UserFarms.Any(uf => uf.UserId == id))
                .Select(f => new FarmDto
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
