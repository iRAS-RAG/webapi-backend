using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FarmingBatchSpecifications
{
    public class FarmingBatchDtoByIdSpec : Specification<FarmingBatch, FarmingBatchDto>
    {
        public FarmingBatchDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(fb => fb.Id == id)
                .Select(fb => new FarmingBatchDto
                {
                    Id = fb.Id,
                    FishTankId = fb.FishTankId,
                    FishTankName = fb.FishTank.Name,
                    TankVolume = Math.Round(
                        Math.PI * fb.FishTank.Radius * fb.FishTank.Radius * fb.FishTank.Height,
                        2
                    ),
                    Name = fb.Name,
                    SpeciesStageConfigId = fb.CurrentStageConfigId,
                    SpeciesName = fb.CurrentStageConfig.Species.Name,
                    StageName = fb.CurrentStageConfig.GrowthStage.Name,
                    Status = fb.Status,
                    PausedReason = fb.PausedReason,
                    StartDate = fb.StartDate,
                    EstimatedHarvestDate = fb.EstimatedHarvestDate,
                    ActualHarvestDate = fb.ActualHarvestDate,
                    InitialQuantity = fb.InitialQuantity,
                    CurrentQuantity = fb.CurrentQuantity,
                    UnitOfMeasure = fb.UnitOfMeasure,
                    CreatedAt = fb.CreatedAt,
                    ModifiedAt = fb.ModifiedAt,
                    SpeciesId = fb.CurrentStageConfig.SpeciesId,
                    EstimatedHarvestCount = fb.EstimatedHarvestCount,
                    EstimatedHarvestWeightKg = fb.EstimatedHarvestWeightKg,
                    PlannedStages = fb
                        .BatchStages.Select(bs => new PlannedStageDto
                        {
                            Id = bs.Id,
                            Sequence = bs.Sequence,
                            SpeciesStageConfigId = bs.SpeciesStageConfigId,
                            GrowthStageId = bs.SpeciesStageConfig.GrowthStageId,
                            StageName = bs.SpeciesStageConfig.GrowthStage.Name,
                            EstimatedStartDate = bs.EstimatedStartDate,
                            EstimatedEndDate = bs.EstimatedEndDate,
                            ActualStartDate = bs.ActualStartDate,
                            ActualEndDate = bs.ActualEndDate,
                            FrequencyPerDay = bs.SpeciesStageConfig.FrequencyPerDay,
                            FeedTypeNames = bs
                                .SpeciesStageConfig.FeedTypes.Select(ft => ft.Name)
                                .ToList(),
                            ExpectedWeightKgPerFish = bs.SpeciesStageConfig.ExpectedWeightKgPerFish,
                        })
                        .ToList(),
                });
        }
    }
}
