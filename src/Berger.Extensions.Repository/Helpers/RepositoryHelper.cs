using Microsoft.EntityFrameworkCore;

namespace Berger.Extensions.Repository;

public static class RepositoryHelper
{
    public static void SoftDelete<TEntity>(this DbContext context, TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(entity);

        var entry = context.Entry(entity);
        var propertyName = entry.Properties.Any(x => x.Metadata.Name == Values.IsDeleted)
            ? Values.IsDeleted
            : entry.Properties.Any(x => x.Metadata.Name == Values.Deleted)
                ? Values.Deleted
                : null;

        if (propertyName is null)
        {
            entry.State = EntityState.Deleted;
            return;
        }

        entry.CurrentValues[propertyName] = true;
        entry.Property(propertyName).IsModified = true;
    }

    public static void Detach<TEntity>(this DbContext context, TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(entity);
        context.Entry(entity).State = EntityState.Detached;
    }

    public static void DetachRange<TEntity>(this DbContext context, IEnumerable<TEntity> entities) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(entities);
        foreach (var entity in entities) context.Entry(entity).State = EntityState.Detached;
    }
}
