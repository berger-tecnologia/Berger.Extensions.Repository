using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Berger.Extensions.Repository
{
    public sealed class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(TContext context, IServiceProvider serviceProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
            => _serviceProvider.GetService<IRepository<TEntity>>() ?? new Repository<TEntity>(_context);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }

    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryPattern<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();

            return services;
        }
    }
}