using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    public class FishTankDtoListSpec : BaseListSpec<FishTank, FishTankDto>
    {
        public FishTankDtoListSpec(FishTankListRequest request)
        {
            Query.AsNoTracking();

            var farmId = request.FarmId;
            ApplyFilter(farmId, ft => ft.FarmId == farmId);

            var sortMap = new Dictionary<string, Expression<Func<FishTank, object?>>>
            {
                ["name"] = ft => ft.Name,
                ["volume"] = ft => ft.Height * ft.Radius * ft.Radius,
            };

            ApplySearch(request.SearchTerm, [ft => ft.Name, ft => ft.Farm.Name]);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "name");

            Query.Select(ft => new FishTankDto
            {
                Id = ft.Id,
                Name = ft.Name,
                Volume = Math.Round((Math.PI * ft.Radius * ft.Radius * ft.Height), 2),
                FarmId = ft.FarmId,
                FarmName = ft.Farm.Name,
                TopicCode = ft.TopicCode,
                CameraUrl = ft.CameraUrl,
                CurrentSpecies =
                    ft.FarmingBatches.Where(fb => fb.Status == FarmingBatchStatus.ACTIVE)
                        .Select(fb => fb.CurrentStageConfig.Species.Name)
                        .FirstOrDefault()
                    ?? "N/A",
                CurrentCount = ft
                    .FarmingBatches.Where(fb => fb.Status == FarmingBatchStatus.ACTIVE)
                    .Select(fb => fb.CurrentQuantity)
                    .FirstOrDefault(),

                //Check Status using Alerts with OPEN status
                HasOpenAlert = ft.Alerts.Any(a => a.Status == AlertStatus.OPEN),
            });
        }
    }
}
