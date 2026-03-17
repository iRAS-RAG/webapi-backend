using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.MortalityLogSpecifications
{
    public class MortalityLogDtoListSpec : BaseListSpec<MortalityLog, MortalityLogDto>
    {
        public MortalityLogDtoListSpec(MortalityLogListRequest request)
        {
            Query.AsNoTracking();

            var sortMap = new Dictionary<string, Expression<Func<MortalityLog, object?>>>
            {
                ["date"] = ml => ml.Date,
                ["quantity"] = ml => ml.Quantity,
                ["createdat"] = ml => ml.CreatedAt,
            };

            ApplyFilter(request.BatchId, ml => ml.BatchId == request.BatchId);

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "date");

            Query.Select(ml => new MortalityLogDto
            {
                Id = ml.Id,
                BatchId = ml.BatchId,
                BatchName = ml.Batch.Name,
                UserId = ml.UserId,
                UserEmail = ml.User.Email,
                Quantity = ml.Quantity,
                Date = ml.Date,
                CreatedAt = ml.CreatedAt,
                ModifiedAt = ml.ModifiedAt,
            });
        }
    }
}
