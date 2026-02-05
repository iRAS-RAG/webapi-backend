using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class ControlDeviceDtoListSpec : Specification<ControlDevice, ControlDeviceDto>
    {
        public ControlDeviceDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(cd => cd.MasterBoard)
                .Include(cd => cd.ControlDeviceType)
                .Select(cd => new ControlDeviceDto
                {
                    Id = cd.Id,
                    Name = cd.Name,
                    PinCode = cd.PinCode,
                    State = cd.State,
                    CommandOn = cd.CommandOn,
                    CommandOff = cd.CommandOff,
                    MasterBoardId = cd.MasterBoardId,
                    MasterBoardName = cd.MasterBoard.Name,
                    ControlDeviceTypeId = cd.ControlDeviceTypeId,
                    ControlDeviceTypeName = cd.ControlDeviceType.Name,
                });
        }
    }
}
