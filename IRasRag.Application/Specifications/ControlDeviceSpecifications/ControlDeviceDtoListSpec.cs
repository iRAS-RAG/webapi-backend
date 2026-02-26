using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.ControlDeviceSpecifications
{
    public class ControlDeviceDtoListSpec : BaseListSpec<ControlDevice, ControlDeviceDto>
    {
        public ControlDeviceDtoListSpec(ControlDeviceListRequest request)
        {
            Query.AsNoTracking();

            var sortKeyMap = new Dictionary<string, Expression<Func<ControlDevice, object?>>>
            {
                ["state"] = cd => cd.State,
                ["name"] = cd => cd.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [cd => cd.Name, cd => cd.MasterBoard.Name, cd => cd.ControlDeviceType.Name]
            );

            ApplySort(request.SortBy, request.SortDir, sortKeyMap, defaultSortKey: "name");

            ApplyFilter(request.State, cd => cd.State == request.State);
            ApplyFilter(request.TankId, cd => cd.MasterBoard.FishTankId == request.TankId!.Value);

            Query.Select(cd => new ControlDeviceDto
            {
                Id = cd.Id,
                Name = cd.Name,
                PinCode = cd.PinCode,
                State = cd.State,
                CommandOn = cd.CommandOn,
                CommandOff = cd.CommandOff,
                MasterBoardId = cd.MasterBoardId,
                MasterBoardName = cd.MasterBoard.Name,
                ControlDeviceTypeName = cd.ControlDeviceType.Name,
            });
        }
    }
}
