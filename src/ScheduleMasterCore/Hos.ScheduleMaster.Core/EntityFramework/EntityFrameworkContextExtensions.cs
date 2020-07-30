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
        public static void AddScheduleMasterDb(this IServiceCollection services)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            if (service.ImplementationInstance == null)
            {
                configuration = (IConfiguration)service.ImplementationFactory(null);
            }
            ConfigurationHelper.Config = configuration;

            services.AddScoped<ISqlContext, SmDbContext>();
            services.AddScoped<SmDbContext>();
            using (var ctx = new SmDbContext())
            {
                ctx.InitTables();
            }
        }
    }
}
