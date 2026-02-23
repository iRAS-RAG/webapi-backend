using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.MasterBoardSpecifications
{
    public class MasterBoardDtoListSpec : BaseListSpec<MasterBoard, MasterBoardDto>
    {
        public MasterBoardDtoListSpec(MasterBoardListRequest request)
        {
            Query.AsNoTracking();

            ApplyFilter(request.FishTankId, mb => mb.FishTankId == request.FishTankId!.Value);

            var sortMap = new Dictionary<string, Expression<Func<MasterBoard, object?>>>
            {
                ["name"] = mb => mb.Name,
                ["macaddress"] = mb => mb.MacAddress,
                ["fishtankname"] = mb => mb.FishTank.Name,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    mb => mb.Name,
                    mb => mb.MacAddress,
                    mb => mb.FishTank.Name,
                ]
            );

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(mb => new MasterBoardDto
            {
                Id = mb.Id,
                Name = mb.Name,
                MacAddress = mb.MacAddress,
                FishTankName = mb.FishTank.Name,
            });
        }
    }
}
