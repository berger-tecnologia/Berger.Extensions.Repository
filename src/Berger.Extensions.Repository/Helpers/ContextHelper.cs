using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Berger.Extensions.Repository;

public static class ContextHelper
{
    public static TContext GetContext<TContext>(this IServiceProvider provider) where TContext : DbContext
        => provider.GetRequiredService<TContext>();

    public static async Task ResetAsync<TContext>(this IServiceProvider provider, CancellationToken cancellationToken = default) where TContext : DbContext
    {
        var context = provider.GetContext<TContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }

    public static ServiceProvider CreateContext<TContext>(this IConfiguration configuration, string pattern = Patterns.AzureSqlServer) where TContext : DbContext
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.ConfigureDbContext<TContext>(configuration, pattern);
        services.AddRepositoryPattern<TContext>();
        return services.BuildServiceProvider();
    }
}
