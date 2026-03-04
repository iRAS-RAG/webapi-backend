using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.FeedingLogSpecifications
{
    public class FeedingLogDtoListSpec : BaseListSpec<FeedingLog, FeedingLogDto>
    {
        public FeedingLogDtoListSpec(FeedingLogListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<FeedingLog, object?>>>
            {
                ["createddate"] = fl => fl.CreatedDate,
                ["amount"] = fl => fl.Amount,
            };

            ApplyFilter(request.CreatedDate, fl => fl.CreatedDate == request.CreatedDate);
            ApplyFilter(request.FarmingBatchId, fl => fl.FarmingBatchId == request.FarmingBatchId);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "createddate");

            Query.Select(fl => new FeedingLogDto
            {
                Id = fl.Id,
                FarmingBatchId = fl.FarmingBatchId,
                FarmingBatchName = fl.FarmingBatch.Name,
                Amount = fl.Amount,
                CreatedDate = fl.CreatedDate,
                CreatedAt = fl.CreatedAt,
                ModifiedAt = fl.ModifiedAt,
            });
        }
    }
}
