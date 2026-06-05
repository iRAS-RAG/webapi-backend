using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.CameraSpecifications
{
    /// <summary>
    /// Specification chiếu một Camera theo Id thành CameraDto,
    /// bao gồm cả thông tin Farm.
    /// </summary>
    public class CameraDtoByIdSpec : Specification<Camera, CameraDto>
    {
        public CameraDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CameraDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Url = c.Url,
                    FarmId = c.FarmId,
                    FarmName = c.Farm.Name,
                    CreatedAt = c.CreatedAt,
                    ModifiedAt = c.ModifiedAt,
                });
        }
    }
}
