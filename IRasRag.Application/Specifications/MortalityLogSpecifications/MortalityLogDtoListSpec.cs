using System.Linq.Expressions;
using IRasRag.Application.DTOs;
using IRasRag.Application.Specifications.Base;
using Ardalis.Specification;
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
                ["createdat"] = ml => ml.CreatedAt
            };

            ApplySort(request.SortBy, request.SortDir, sortMap, defaultSortKey: "date");

            Query.Select(ml => new MortalityLogDto
            {
                Id = ml.Id,
                BatchId = ml.BatchId,
                Quantity = ml.Quantity,
                Date = ml.Date,
                CreatedAt = ml.CreatedAt,
                ModifiedAt = ml.ModifiedAt,
            });
        }
    }
}
