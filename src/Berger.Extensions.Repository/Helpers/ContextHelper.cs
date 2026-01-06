using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Berger.Extensions.Repository
{
    public static class ContextHelper
    {
        public static T GetContext<T>(this IServiceProvider provider) where T : DbContext
        {
            return provider.GetContext<T>();
        }
        public static void Reset<T>(this IServiceProvider provider) where T : DbContext
        {
            var _context = GetContext<T>(provider);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
        public static ServiceProvider CreateContext<T>(this IConfiguration builder, string pattern = Patterns.AzureSqlServer) where T : DbContext
        {
            // Service Configuration
            var services = new ServiceCollection();

            // IConfiguration
            services.AddSingleton<IConfiguration>(builder);

            // Database Configuration
            services.ConfigureDbContext<T>(builder, pattern);

            // Service Building
            return services.BuildServiceProvider();
        }
    }
}