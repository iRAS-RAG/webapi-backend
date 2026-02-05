using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class SensorDtoListSpec : Specification<Sensor, SensorDto>
    {
        public SensorDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(s => s.SensorType)
                .Include(s => s.MasterBoard)
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
