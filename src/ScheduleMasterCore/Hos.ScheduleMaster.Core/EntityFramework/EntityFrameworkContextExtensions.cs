using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EntityFrameworkContextExtensions
    {
        public static void AddScheduleMasterDb(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigurationCache.DbConnector = new DbConnector
            {
                Provider = (DbProvider)Enum.Parse(typeof(DbProvider), configuration["DbConnector:Provider"] ?? "mysql", true),
                ConnectionString = configuration["DbConnector:ConnectionString"]
            };

            services.AddDbContext<SmDbContext>();
            using (var ctx = new SmDbContext())
            {
                ctx.Database.EnsureCreated();
            }
        }
    }
}
