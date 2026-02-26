using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Specifications.FarmingBatchSpecifications
{
    public class ActiveFarmingBatchDtoListSpec
        : Specification<FarmingBatch, ActiveFarmingBatchResponseDto>
    {
        public ActiveFarmingBatchDtoListSpec(Guid fishTankId)
        {
            Query
                .AsNoTracking()
                .Where(fb => fb.Status == FarmingBatchStatus.ACTIVE && fb.FishTank.Id == fishTankId)
                .Select(fb => new ActiveFarmingBatchResponseDto
                {
                    FishTankName = fb.FishTank.Name,
                    SpeciesName = fb.CurrentStageConfig.Species.Name,
                    CurrentQuantity = (int)fb.CurrentQuantity,
                    TankVolume = Math.Round(
                        Math.PI * fb.FishTank.Height * fb.FishTank.Height * fb.FishTank.Radius,
                        2
                    ),
                });
        }
    }
}
