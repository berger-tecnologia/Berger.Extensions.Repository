using System.Linq.Expressions;

namespace Berger.Extensions.Repository
{
    public interface IReadRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query(bool tracking = false);
        IQueryable<TEntity> Query(ISpecification<TEntity> specification);
        Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default) where TKey : notnull;
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<PagedResult<TEntity>> PageAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    }
}