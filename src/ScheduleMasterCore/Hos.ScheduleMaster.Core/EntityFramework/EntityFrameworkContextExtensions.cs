using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EntityFrameworkContextExtensions
    {
        public static void AddScheduleMasterDb(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigurationCache.DbConnector = new DbConnector
            {
                Provider = (DbProvider)Enum.Parse(typeof(DbProvider), configuration["DbConnector:Provider"] ?? "mysql", true),
                ConnectionString = configuration["DbConnector:ConnectionString"],
                Version = configuration["DbConnector:Version"],
            };

            services.AddDbContext<SmDbContext>();
            using (var ctx = new SmDbContext())
            {
                ctx.Database.EnsureCreated();
            }
        }
    }
}
