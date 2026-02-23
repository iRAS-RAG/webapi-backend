using System.Linq.Expressions;
using Ardalis.Specification;

namespace IRasRag.Application.Specifications.Base
{
    public abstract class BaseListSpec<TEntity, TResult> : Specification<TEntity, TResult>
    {
        /// <summary>
        /// <para>
        /// Applies sorting to the specification query using a safe mapping
        /// between client-provided sort keys and entity property expressions.
        /// </para>
        ///
        /// <para>
        /// How it works:
        /// </para>
        ///
        /// <para>
        /// If no sortBy is provided
        /// -> Applies the default sort expression defined by defaultSortKey
        /// </para>
        ///
        /// <para>
        /// If sortBy is provided
        /// -> Converts it to lowercase
        /// -> Looks up the corresponding expression in sortMap
        /// -> Throws an exception if the field is not allowed
        /// </para>
        ///
        /// <para>
        /// Applies OrderBy or OrderByDescending depending on sortDir.
        /// </para>
        /// <para>
        /// Example: ApplySorting(request.SortBy, request.SortDir, sortMap, "createdat");
        /// </para>
        /// </summary>
        protected void ApplySort(
            string? sortBy,
            string sortDir,
            IDictionary<string, Expression<Func<TEntity, object?>>> sortMap,
            string defaultSortKey
        )
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                Query.OrderBy(sortMap[defaultSortKey]);
                return;
            }

            if (!sortMap.TryGetValue(sortBy.ToLowerInvariant(), out var expression))
                throw new ArgumentException($"Invalid sort field: {sortBy}");

            if (sortDir == "desc")
                Query.OrderByDescending(expression);
            else
                Query.OrderBy(expression);
        }

        /// <summary>
        /// Applies a dynamic OR-based search filter across multiple string fields.
        ///
        /// Goal:
        /// Build a predicate equivalent to:
        ///
        ///     e => e.Field1.Contains(term)
        ///       || e.Field2.Contains(term)
        ///
        /// while keeping the entire expression translatable by EF Core.
        ///
        /// How it works:
        ///
        /// 1. Guard clause:
        ///    - If searchTerm is null/empty, no filtering is applied.
        ///
        /// 2. Normalize the search term:
        ///    - Trims whitespace.
        ///    - Converts to lowercase.
        ///
        /// 3. Create a shared ParameterExpression:
        ///    - All generated field expressions must use the same parameter instance.
        ///    - This ensures the final expression tree is valid and composable.
        ///
        /// 4. For each provided search field:
        ///    a. Replace its original parameter with the shared one.
        ///    b. Build:
        ///         field != null && field.ToLower().Contains(term)
        ///    c. Combine each clause using Expression.OrElse.
        ///
        /// 5. Wrap the combined expression in:
        ///        Expression<Func<TEntity, bool>>
        ///    and apply it via Query.Where().
        ///
        /// Why parameter replacement is necessary:
        /// Each incoming field expression has its own parameter (e.g. x => x.Name).
        /// To combine them into a single lambda (e => ... || ...),
        /// all expressions must share the same parameter instance.
        /// </summary>
        protected void ApplySearch(
            string? searchTerm,
            IEnumerable<Expression<Func<TEntity, string?>>> searchFields
        )
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return;

            var term = searchTerm.Trim().ToLower();
            var parameter = Expression.Parameter(typeof(TEntity), "e"); // representing " e => "
            var containsMethod = typeof(string).GetMethod(
                nameof(string.Contains),
                [typeof(string)]
            )!; // string.Contains(string)
            var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!; // string.ToLower()
            var termExpression = Expression.Constant(term);

            Expression? combined = null;

            foreach (var field in searchFields)
            {
                // Reuse the same parameter across all field expressions
                // e.g. convert x => x.Name to e => e.Name
                var body = ExpressionReplacer.Replace(field.Body, field.Parameters[0], parameter);

                // field != null && field.ToLower().Contains(term)
                var notNull = Expression.NotEqual(body, Expression.Constant(null, typeof(string)));
                var toLower = Expression.Call(body, toLowerMethod);
                var contains = Expression.Call(toLower, containsMethod, termExpression);
                var clause = Expression.AndAlso(notNull, contains);
                combined = combined is null ? clause : Expression.OrElse(combined, clause);
            }

            if (combined is null)
                return;

            var lambda = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
            Query.Where(lambda);
        }

        /// <summary>
        /// <para>
        /// Applies a conditional filter only when the provided value
        /// is not considered empty.
        /// </para>
        ///
        /// <para>
        /// Supported empty checks: null, empty or whitespace string, empty collections.
        /// </para>
        ///
        /// <para>
        /// Purpose: Avoids writing repetitive conditional statements before calling Query.Where().
        /// </para>
        ///
        /// <para>
        /// Behavior:
        /// </para>
        ///
        /// <para>
        /// If value is empty
        /// -> No filter is applied
        /// </para>
        ///
        /// <para>
        /// If value contains meaningful content
        /// -> The filter expression is added to the query
        /// </para>
        ///
        /// <para>
        /// Example: ApplyFilter(request.Status, e => e.Status == request.Status)
        /// </para>
        /// </summary>
        protected void ApplyFilter<TValue>(TValue? value, Expression<Func<TEntity, bool>> filter)
        {
            var isEmpty =
                value is null
                || (value is string s && string.IsNullOrWhiteSpace(s))
                || (value is IEnumerable<object> e && !e.Any());

            if (!isEmpty)
                Query.Where(filter);
        }

        /// <summary>
        /// <para>
        /// Utility class used internally by ApplySearch to replace the parameter
        /// of one expression with another.
        /// </para>
        ///
        /// <para>
        /// Why this is needed:
        /// </para>
        ///
        /// <para>
        /// When combining multiple expression trees, they must share
        /// the same ParameterExpression instance.
        /// </para>
        ///
        /// <para>
        /// Example:
        /// x => x.Name,
        /// y => y.Description
        /// </para>
        ///
        /// <para>
        /// These cannot be combined directly because x and y are different parameters.
        /// </para>
        ///
        /// <para>
        /// This helper replaces the original parameter with a shared one,
        /// ensuring the final combined lambda expression is structurally valid.
        /// </para>
        /// </summary>
        internal static class ExpressionReplacer
        {
            public static Expression Replace(
                Expression body,
                ParameterExpression oldParam,
                ParameterExpression newParam
            ) => new ReplaceVisitor(oldParam, newParam).Visit(body);

            private sealed class ReplaceVisitor(
                ParameterExpression oldParam,
                ParameterExpression newParam
            ) : ExpressionVisitor
            {
                protected override Expression VisitParameter(ParameterExpression node) =>
                    node == oldParam ? newParam : base.VisitParameter(node);
            }
        }
    }
}
