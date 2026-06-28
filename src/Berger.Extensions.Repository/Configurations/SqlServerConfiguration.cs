using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Berger.Extensions.Repository;

public interface IApplicationContext
{
    Guid ApplicationId { get; }
    void SetApplication(Guid applicationId);
}

public sealed class ApplicationContext : IApplicationContext
{
    public Guid ApplicationId { get; private set; } = Guid.Empty;
    public void SetApplication(Guid applicationId) => ApplicationId = applicationId;
}

public static class SqlServerConfiguration
{
    public static IServiceCollection ConfigureDbContext<TContext>(this IServiceCollection services, IConfiguration configuration, string pattern = Patterns.AzureSqlServer, bool tracking = true) where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connection = configuration.GetSection(pattern).Value ?? configuration.GetConnectionString(pattern) ?? configuration.GetConnectionString(pattern.Replace("ConnectionStrings:", string.Empty));

        if (string.IsNullOrWhiteSpace(connection)) throw new FileNotFoundException(Errors.ConfigNotFound);

        services.AddScoped<IApplicationContext, ApplicationContext>();
        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(connection, sql => sql.EnableRetryOnFailure());

            if (!tracking) options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddRepositoryPattern<TContext>();

        return services;
    }

    public static IServiceCollection ConfigureDbContextFactory<TContext>(this IServiceCollection services, IConfiguration configuration, string pattern = Patterns.AzureSqlServer, ServiceLifetime lifetime = ServiceLifetime.Scoped) where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connection = configuration.GetSection(pattern).Value ?? configuration.GetConnectionString(pattern) ?? configuration.GetConnectionString(pattern.Replace("ConnectionStrings:", string.Empty));

        if (string.IsNullOrWhiteSpace(connection)) throw new FileNotFoundException(Errors.ConfigNotFound);

        services.AddDbContextFactory<TContext>(options =>
        {
            options.UseSqlServer(connection, sql => sql.EnableRetryOnFailure());
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.ConfigureWarnings(builder =>
            {
                builder.Ignore(RelationalEventId.BoolWithDefaultWarning);
                builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
            });
        }, lifetime);

        return services;
    }
}
