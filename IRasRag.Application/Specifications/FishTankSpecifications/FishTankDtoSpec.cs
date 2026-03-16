using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.FishTankSpecifications
{
    public class FishTankDtoSpec : Specification<FishTank, FishTankDto>
    {
        public FishTankDtoSpec(Guid fishTankId)
        {
            Query
                .AsNoTracking()
                .Where(ft => ft.Id == fishTankId)
                .Select(ft => new FishTankDto
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
                    HasOpenAlert = ft.Alerts.Any(a => a.Status == AlertStatus.OPEN),
                });
        }
    }
}
