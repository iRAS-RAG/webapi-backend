using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Domain.Enums;
using IRasRag.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IRasRag.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        protected IQueryable<T> Query(QueryType type = QueryType.ActiveOnly)
        {
            return type == QueryType.IncludeDeleted
                ? _dbSet.IgnoreQueryFilters()
                : _dbSet;
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryType type = QueryType.ActiveOnly)
        {
            return await Query(type).AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, QueryType type = QueryType.ActiveOnly)
        {
            return predicate == null
                ? await Query(type).CountAsync()
                : await Query(type).CountAsync(predicate);
        }

        public void DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, QueryType type = QueryType.ActiveOnly)
        {
            return await Query(type).Where(predicate).ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, QueryType type = QueryType.ActiveOnly)
        {
            return await Query(type).FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync(QueryType type = QueryType.ActiveOnly)
        {
            return await Query(type).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id, QueryType type = QueryType.ActiveOnly)
        {
            return await Query(type).FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<PaginatedResult<T>> GetPaginatedResult(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public void UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
