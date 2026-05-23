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
                        .BatchStages.Select(bs => new IRasRag.Application.DTOs.PlannedStageDto
                        {
                            Id = bs.Id,
                            Sequence = bs.Sequence,
                            SpeciesStageConfigId = bs.SpeciesStageConfigId,
                            GrowthStageId = bs.SpeciesStageConfig.GrowthStageId,
                            StageName = bs.SpeciesStageConfig.GrowthStage.Name,
                            ExpectedDurationDays = bs.ExpectedDurationDays,
                            EstimatedStartDate = bs.EstimatedStartDate,
                            EstimatedEndDate = bs.EstimatedEndDate,
                            ActualStartDate = bs.ActualStartDate,
                            ActualEndDate = bs.ActualEndDate,
                            AmountPer100Fish = bs.SpeciesStageConfig.AmountPer100Fish,
                            FrequencyPerDay = bs.SpeciesStageConfig.FrequencyPerDay,
                            MaxStockingDensity = bs.SpeciesStageConfig.MaxStockingDensity,
                            FeedTypeNames = bs
                                .SpeciesStageConfig.FeedTypes.Select(ft => ft.Name)
                                .ToList(),
                            ExpectedWeightKgPerFish = bs.SpeciesStageConfig.ExpectedWeightKgPerFish,
                            SurvivalRate = bs.SpeciesStageConfig.SurvivalRate,
                        })
                        .ToList(),
                });
        }
    }
}
