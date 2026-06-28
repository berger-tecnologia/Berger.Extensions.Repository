using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Berger.Extensions.Repository;

public static class SeedHelper
{
    public static async Task ExecuteSqlFileAsync(this DbContext context, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!File.Exists(path)) throw new FileNotFoundException(path);

        var script = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);
        await context.Database.ExecuteSqlRawAsync(script, cancellationToken);
    }

    public static async Task ExecuteSqlBatchFileAsync(this DbContext context, string path, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public static async Task ExecuteSqlLinesAsync(this DbContext context, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!File.Exists(path)) throw new FileNotFoundException(path);

        await foreach (var line in File.ReadLinesAsync(path, Encoding.UTF8, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            await context.Database.ExecuteSqlRawAsync(line, cancellationToken);
        }
    }

    [Obsolete("Use ExecuteSqlFileAsync instead.")]
    public static void ExecuteSqlRaw(this DbContext context, string path)
        => context.ExecuteSqlFileAsync(path).GetAwaiter().GetResult();

    [Obsolete("Use ExecuteSqlBatchFileAsync instead.")]
    public static void ExecuteSqlRawBatch(this DbContext context, string path)
        => context.ExecuteSqlBatchFileAsync(path).GetAwaiter().GetResult();
}
