using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.SensorSpecifications
{
    public class SensorDtoListSpec : BaseListSpec<Sensor, SensorDto>
    {
        public SensorDtoListSpec(SensorListRequest request)
        {
            Query.AsNoTracking();

            ApplyFilter(request.MasterBoardId, s => s.MasterBoardId == request.MasterBoardId!.Value);

            var sortMap = new Dictionary<string, Expression<Func<Sensor, object?>>>
            {
                ["name"] = s => s.Name,
                ["pincode"] = s => s.PinCode,
                ["sensortypename"] = s => s.SensorType.Name,
                ["masterboardname"] = s => s.MasterBoard.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    s => s.Name,
                    s => s.SensorType.Name,
                    s => s.MasterBoard.Name,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(s => new SensorDto
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
