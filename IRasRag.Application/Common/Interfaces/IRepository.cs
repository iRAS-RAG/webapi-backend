using System.Linq.Expressions;
using IRasRag.Application.Common.Models;
using IRasRag.Domain.Enums;

namespace IRasRag.Application.Common.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        Task<T?> GetByIdAsync(Guid id, QueryType type = QueryType.ActiveOnly);
        Task<IEnumerable<T>> GetAllAsync(QueryType type = QueryType.ActiveOnly);
        Task<PaginatedResult<T>> GetPaginatedResult(
            IQueryable<T> query,
            int pageNumber,
            int pageSize
        );
        Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            QueryType type = QueryType.ActiveOnly
        );
        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            QueryType type = QueryType.ActiveOnly
        );
        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            QueryType type = QueryType.ActiveOnly
        );
        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            QueryType type = QueryType.ActiveOnly
        );

        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task<int> SaveChangesAsync();
    }
}
