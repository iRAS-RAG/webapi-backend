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
                    SensorTypeName = s.SensorType.Name,
                    MasterBoardId = s.MasterBoardId,
                    MasterBoardName = s.MasterBoard.Name,
                });
        }
    }

    public class SensorDtoListByMasterBoardIdSpec : Specification<Sensor, SensorDto>
    {
        public SensorDtoListByMasterBoardIdSpec(Guid masterBoardId)
        {
            Query
                .AsNoTracking()
                .Where(s => s.MasterBoardId == masterBoardId)
                .Select(s => new SensorDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PinCode = s.PinCode,
                    SensorTypeName = s.SensorType.Name,
                    MasterBoardId = s.MasterBoardId,
                    MasterBoardName = s.MasterBoard.Name,
                });
        }
    }
}
