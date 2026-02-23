using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
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
                ["amount"] = fl => fl.Amount
            };

            ApplyFilter(request.CreatedDate, fl => fl.CreatedDate == request.CreatedDate);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "createddate");

            Query.Select(fl => new FeedingLogDto
            {
                Id = fl.Id,
                FarmingBatchId = fl.FarmingBatchId,
                Amount = fl.Amount,
                CreatedDate = fl.CreatedDate,
                CreatedAt = fl.CreatedAt,
                ModifiedAt = fl.ModifiedAt,
            });
        }
    }
}
