using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FarmingBatchSpecifications
{
    public class FarmingBatchDtoListSpec : BaseListSpec<FarmingBatch, FarmingBatchDto>
    {
        public FarmingBatchDtoListSpec(FarmingBatchListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<FarmingBatch, object?>>>
            {
                ["name"] = fb => fb.Name,
                ["startdate"] = fb => fb.StartDate,
                ["status"] = fb => fb.Status,
                ["currentquantity"] = fb => fb.CurrentQuantity,
                ["speciesname"] = fb => fb.CurrentStageConfig.Species.Name,
                ["fishtankname"] = fb => fb.FishTank.Name,
                ["actualharvestdate"] = fb => fb.ActualHarvestDate,
                ["estimatedharvestdate"] = fb => fb.EstimatedHarvestDate,
            };

            ApplySearch(request.SearchTerm, [fb => fb.Name, fb => fb.FishTank.Name]);

            ApplyFilter(request.Status, fb => fb.Status == request.Status);
            ApplyFilter(request.FishTankId, fb => fb.FishTankId == request.FishTankId);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "startdate");

            Query.Select(fb => new FarmingBatchDto
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
                EstimatedHarvestCount = fb.EstimatedHarvestCount,
                EstimatedHarvestWeightKg = fb.EstimatedHarvestWeightKg,
                ActualHarvestWeightKg = fb.ActualHarvestWeightKg,
                CreatedAt = fb.CreatedAt,
                ModifiedAt = fb.ModifiedAt,
                PlannedStages = new List<PlannedStageDto>(),
                Fcr = fb.Fcr,
            });
        }
    }
}
