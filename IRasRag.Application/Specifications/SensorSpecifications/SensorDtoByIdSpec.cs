using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SensorSpecifications
{
    /// <summary>
    /// Specification chiếu một Sensor theo Id thành SensorDto,
    /// bao gồm cả thông tin SensorType và MasterBoard.
    /// </summary>
    public class SensorDtoByIdSpec : Specification<Sensor, SensorDto>
    {
        public SensorDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SensorDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PinCode = s.PinCode,
                    SensorTypeId = s.SensorTypeId,
                    SensorTypeName = s.SensorType.Name,
                    MasterBoardId = s.MasterBoardId,
                    MasterBoardName = s.MasterBoard.Name,
                });
        }
    }
}
