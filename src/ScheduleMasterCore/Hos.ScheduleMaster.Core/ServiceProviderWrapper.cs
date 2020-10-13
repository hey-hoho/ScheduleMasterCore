using System;
using System.Collections.Generic;
using System.Text;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Hos.ScheduleMaster.Core
{
    public class ServiceProviderWrapper : IDisposable
    {
        protected IServiceScope scope;
        protected IServiceProvider provider;

        public ServiceProviderWrapper()
        {
            scope = ConfigurationCache.RootServiceProvider.CreateScope();
            provider = scope.ServiceProvider;
        }

        public T GetService<T>()
        {
            return provider.GetRequiredService<T>();
        }

        public virtual void Dispose()
        {
            scope.Dispose();
        }
    }

    public class ScopeDbContext : ServiceProviderWrapper
    {
        private SmDbContext dbContext;

        public ScopeDbContext()
        {
            dbContext = scope.ServiceProvider.GetRequiredService<SmDbContext>();
        }

        public SmDbContext GetDbContext()
        {
            return dbContext;
        }

        public override void Dispose()
        {
            base.Dispose();
            dbContext.Dispose();
        }
    }
}
