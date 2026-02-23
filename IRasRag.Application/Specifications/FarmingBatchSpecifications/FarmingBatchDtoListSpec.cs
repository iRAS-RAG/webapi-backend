using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
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
                ["startdate"] = fb => fb.StartDate,
                ["status"] = fb => fb.Status,
                ["currentquantity"] = fb => fb.CurrentQuantity,
            };

            ApplySearch(
                request.SearchTerm,
                [
                    fb => fb.Name,
                    fb => fb.FishTank.Name,
                ]
            );

            ApplyFilter(request.Status, fb => fb.Status == request.Status);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "startdate");

            Query.Select(fb => new FarmingBatchDto
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
