using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class FarmingBatchDtoListSpec : Specification<FarmingBatch, FarmingBatchDto>
    {
        public FarmingBatchDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(fb => fb.FishTank)
                .Include(fb => fb.Species)
                .Select(fb => new FarmingBatchDto
                {
                    Id = fb.Id,
                    FishTankId = fb.FishTankId,
                    FishTankName = fb.FishTank.Name,
                    Name = fb.Name,
                    SpeciesId = fb.SpeciesId,
                    SpeciesName = fb.Species.Name,
                    Status = fb.Status,
                    StartDate = fb.StartDate,
                    EstimatedHarvestDate = fb.EstimatedHarvestDate,
                    ActualHarvestDate = fb.ActualHarvestDate,
                    InitialQuantity = fb.InitialQuantity,
                    CurrentQuantity = fb.CurrentQuantity,
                    UnitOfMeasure = fb.UnitOfMeasure,
                    CreatedAt = fb.CreatedAt,
                    ModifiedAt = fb.ModifiedAt,
                });
        }
    }
}
