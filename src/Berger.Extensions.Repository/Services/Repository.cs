using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Berger.Extensions.Repository;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> Set;

    public Repository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Set = Context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query(bool tracking = false)
        => tracking ? Set : Set.AsNoTracking();

    public IQueryable<TEntity> Query(ISpecification<TEntity> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        IQueryable<TEntity> query = specification.AsNoTracking ? Set.AsNoTracking() : Set;

        if (specification.IgnoreQueryFilters) query = query.IgnoreQueryFilters();
        if (specification.Criteria is not null) query = query.Where(specification.Criteria);

        foreach (var include in specification.Includes)
            query = query.Include(include);

        if (specification.OrderBy is not null) query = specification.OrderBy(query);
        if (specification.Skip.HasValue) query = query.Skip(specification.Skip.Value);
        if (specification.Take.HasValue) query = query.Take(specification.Take.Value);

        return query;
    }

    public async Task<TEntity?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default) where TKey : notnull
        => await Set.FindAsync(new object[] { id }, cancellationToken);

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await Query().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        => await Query().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await Query().Where(predicate).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => await Query(specification).ToListAsync(cancellationToken);

    public async Task<PagedResult<TEntity>> PageAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        IQueryable<TEntity> query = Query();
        if (predicate is not null) query = query.Where(predicate);

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<TEntity>(items, page, pageSize, total);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await Query().AnyAsync(predicate, cancellationToken);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null
            ? await Query().CountAsync(cancellationToken)
            : await Query().CountAsync(predicate, cancellationToken);

    public async Task<TEntity> AddAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await Set.AddAsync(entity, cancellationToken);
        if (saveChanges) await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await Set.AddRangeAsync(entities, cancellationToken);
        if (saveChanges) await SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Update(entity);
        if (saveChanges) await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task RemoveAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Remove(entity);
        if (saveChanges) await SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        Set.RemoveRange(entities);
        if (saveChanges) await SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Context.SoftDelete(entity);
        if (saveChanges) await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ExecuteDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await Set.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    //public async Task<int> ExecuteUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls, CancellationToken cancellationToken = default)
    //    => await Set.Where(predicate).ExecuteUpdateAsync(setPropertyCalls, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}

public sealed class Repository<TEntity, TContext> : Repository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    public Repository(TContext context) : base(context) { }
}
