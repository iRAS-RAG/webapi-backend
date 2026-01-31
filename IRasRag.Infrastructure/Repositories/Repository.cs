using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using IRasRag.Application.Common.Interfaces;
using IRasRag.Application.Common.Models;
using IRasRag.Domain.Enums;
using IRasRag.Infrastructure.Persistence;
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

        protected IQueryable<T> GetQueryable(QueryType type = QueryType.ActiveOnly)
        {
            return type == QueryType.IncludeDeleted
                ? _dbSet.IgnoreQueryFilters()
                : _dbSet;
        }

        #region Base Query Methods (Expression-based)

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, QueryType type = QueryType.ActiveOnly)
        {
            return await GetQueryable(type).FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, QueryType type = QueryType.ActiveOnly)
        {
            return await GetQueryable(type).Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(QueryType type = QueryType.ActiveOnly)
        {
            return await GetQueryable(type).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryType type = QueryType.ActiveOnly)
        {
            return await GetQueryable(type).AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, QueryType type = QueryType.ActiveOnly)
        {
            return predicate == null
                ? await GetQueryable(type).CountAsync()
                : await GetQueryable(type).CountAsync(predicate);
        }

        public async Task<PaginatedResult<T>> GetPaginatedAsync(int pageNumber, int pageSize, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
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

        #endregion

        #region Specification Methods

        // Non-projected specification (returns T)
        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            return await SpecificationEvaluator.Default.GetQuery(query, spec).FirstOrDefaultAsync();
        }

        // Projected specification (returns TResult)
        public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> spec, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            return await SpecificationEvaluator.Default.GetQuery(query, spec).FirstOrDefaultAsync();
        }

        // Non-projected specification (returns IEnumerable<T>)
        public async Task<IEnumerable<T>> ListAsync(ISpecification<T> spec, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            return await SpecificationEvaluator.Default.GetQuery(query, spec).ToListAsync();
        }

        // Projected specification (returns IEnumerable<TResult>)
        public async Task<IEnumerable<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            return await SpecificationEvaluator.Default.GetQuery(query, spec).ToListAsync();
        }

        // Non-projected specification with pagination
        public async Task<PaginatedResult<T>> GetPaginatedAsync(ISpecification<T> spec, int pageNumber, int pageSize, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            var specQuery = SpecificationEvaluator.Default.GetQuery(query, spec);

            var count = await specQuery.CountAsync();
            var items = await specQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Projected specification with pagination
        public async Task<PaginatedResult<TResult>> GetPaginatedAsync<TResult>(ISpecification<T, TResult> spec, int pageNumber, int pageSize, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            var specQuery = SpecificationEvaluator.Default.GetQuery(query, spec);

            var count = await specQuery.CountAsync();
            var items = await specQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<TResult>
            {
                Items = items,
                TotalCount = count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Count with specification
        public async Task<int> CountAsync(ISpecification<T> spec, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            return await SpecificationEvaluator.Default.GetQuery(query, spec).CountAsync();
        }

        // Any with specification
        public async Task<bool> AnyAsync(ISpecification<T> spec, QueryType type = QueryType.ActiveOnly)
        {
            var query = GetQueryable(type);
            return await SpecificationEvaluator.Default.GetQuery(query, spec).AnyAsync();
        }

        #endregion

        #region Basic CRUD Operations

        public async Task<T?> GetByIdAsync(Guid id, QueryType type = QueryType.ActiveOnly)
        {
            return await GetQueryable(type).FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion
    }
}