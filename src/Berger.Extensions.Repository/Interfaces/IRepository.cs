using System.Linq.Expressions;

namespace Berger.Extensions.Repository
{
    public interface IRepository<TEntity> : IReadRepository<TEntity> where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task RemoveAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task SoftDeleteAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<int> ExecuteDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        //Task<int> ExecuteUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}