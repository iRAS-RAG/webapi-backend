using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.ControlDeviceTypeSpecifications
{
    public class ControlDeviceTypeDtoListSpec : BaseListSpec<ControlDeviceType, ControlDeviceTypeDto>
    {
        public ControlDeviceTypeDtoListSpec(ControlDeviceTypeListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<ControlDeviceType, object?>>>
            {
                ["name"] = cdt => cdt.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    cdt => cdt.Name,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(cdt => new ControlDeviceTypeDto
            {
                Id = cdt.Id,
                Name = cdt.Name,
            });
        }
    }
}
