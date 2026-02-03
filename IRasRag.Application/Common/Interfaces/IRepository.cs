using System.Linq.Expressions;
using Ardalis.Specification;
using IRasRag.Application.Common.Models;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        #region Base Query Methods (Expression-based)

        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            QueryType type = QueryType.ActiveOnly
        );
        Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            QueryType type = QueryType.ActiveOnly
        );
        Task<IEnumerable<T>> GetAllAsync(QueryType type = QueryType.ActiveOnly);
        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            QueryType type = QueryType.ActiveOnly
        );
        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            QueryType type = QueryType.ActiveOnly
        );
        Task<PaginatedResult<T>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            QueryType type = QueryType.ActiveOnly
        );

        #endregion

        #region Specification Methods

        // Non-projected specification (returns T)
        Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, QueryType type = QueryType.ActiveOnly);

        // Projected specification (returns TResult)
        Task<TResult?> FirstOrDefaultAsync<TResult>(
            ISpecification<T, TResult> spec,
            QueryType type = QueryType.ActiveOnly
        );

        // Non-projected specification (returns IEnumerable<T>)
        Task<IEnumerable<T>> ListAsync(
            ISpecification<T> spec,
            QueryType type = QueryType.ActiveOnly
        );

        // Projected specification (returns IEnumerable<TResult>)
        Task<IEnumerable<TResult>> ListAsync<TResult>(
            ISpecification<T, TResult> spec,
            QueryType type = QueryType.ActiveOnly
        );

        // Non-projected specification with pagination
        Task<PaginatedResult<T>> GetPaginatedAsync(
            ISpecification<T> spec,
            int pageNumber,
            int pageSize,
            QueryType type = QueryType.ActiveOnly
        );

        // Projected specification with pagination
        Task<PaginatedResult<TResult>> GetPaginatedAsync<TResult>(
            ISpecification<T, TResult> spec,
            int pageNumber,
            int pageSize,
            QueryType type = QueryType.ActiveOnly
        );

        // Count with specification
        Task<int> CountAsync(ISpecification<T> spec, QueryType type = QueryType.ActiveOnly);

        // Any with specification
        Task<bool> AnyAsync(ISpecification<T> spec, QueryType type = QueryType.ActiveOnly);

        #endregion

        #region Basic CRUD Operations

        Task<T?> GetByIdAsync(Guid id, QueryType type = QueryType.ActiveOnly);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        #endregion
    }
}
