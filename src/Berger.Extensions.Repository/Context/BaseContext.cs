using Microsoft.EntityFrameworkCore;

namespace Berger.Extensions.Repository;

public abstract class BaseContext<TContext> : DbContext where TContext : DbContext
{
    protected BaseContext(DbContextOptions<TContext> options) : base(options)
    {
        Database.SetCommandTimeout(1000);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<string>().AreUnicode(false).HaveMaxLength(4000);
        builder.Properties<decimal>().HavePrecision(28, 6);
        builder.Properties<double>().HavePrecision(28, 6);
        builder.Properties<DateTime>().HavePrecision(3);
        builder.Properties<DateTimeOffset>().HavePrecision(3);
        builder.Properties<List<string>>().HaveConversion<StringListConverter>().AreUnicode(false).HaveMaxLength(4000);

        base.ConfigureConventions(builder);
    }
}
