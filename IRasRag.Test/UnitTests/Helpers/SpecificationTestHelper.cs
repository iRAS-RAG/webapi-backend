using Ardalis.Specification;
using IRasRag.Application.Common.Models.Pagination;

namespace IRasRag.Test.UnitTests.Helpers
{
    public static class SpecificationTestHelper
    {
        // Applies an Ardalis specification to an in-memory collection
        // including filtering, sorting, projection, and manual paging.
        // This is used in unit tests to simulate repository behavior.
        public static PagedResult<TResult> ApplySpecificationWithPaging<TEntity, TResult>(
            IEnumerable<TEntity> source,
            ISpecification<TEntity, TResult> specification,
            int pageNumber,
            int pageSize
        )
            where TEntity : class
        {
            // Convert source to IQueryable for LINQ execution
            IQueryable<TEntity> query = source.AsQueryable();

            // Apply filtering (Where expressions)
            foreach (var whereExpression in specification.WhereExpressions)
            {
                query = query.Where(whereExpression.Filter);
            }

            IOrderedQueryable<TEntity>? orderedQuery = null;

            // Apply sorting (OrderBy / ThenBy chain)
            foreach (var orderExpression in specification.OrderExpressions)
            {
                orderedQuery = orderExpression.OrderType switch
                {
                    OrderTypeEnum.OrderBy => query.OrderBy(orderExpression.KeySelector),
                    OrderTypeEnum.OrderByDescending => query.OrderByDescending(
                        orderExpression.KeySelector
                    ),
                    OrderTypeEnum.ThenBy =>
                        orderedQuery is null
                            ? query.OrderBy(orderExpression.KeySelector)
                            : orderedQuery.ThenBy(orderExpression.KeySelector),
                    OrderTypeEnum.ThenByDescending =>
                        orderedQuery is null
                            ? query.OrderByDescending(orderExpression.KeySelector)
                            : orderedQuery.ThenByDescending(orderExpression.KeySelector),
                    _ => orderedQuery,
                };
            }

            // Replace original query with ordered query if sorting was applied
            if (orderedQuery is not null)
            {
                query = orderedQuery;
            }

            // Ensure the specification defines a projection
            if (specification.Selector is null)
            {
                throw new InvalidOperationException("Projected specification must define Selector.");
            }

            // Apply projection
            var projectedQuery = query.Select(specification.Selector);

            // Calculate total items before paging
            var totalItems = projectedQuery.Count();

            // Apply manual pagination
            var items = projectedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<TResult>
            {
                Items = items,
                TotalItems = totalItems
            };
        }
    }
}