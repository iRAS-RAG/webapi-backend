using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.ControlDeviceSpecifications
{
    /// <summary>
    /// Specification chiếu một ControlDevice theo Id thành ControlDeviceDto,
    /// bao gồm cả thông tin MasterBoard và ControlDeviceType.
    /// </summary>
    public class ControlDeviceDtoByIdSpec : Specification<ControlDevice, ControlDeviceDto>
    {
        public ControlDeviceDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(cd => cd.Id == id)
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
