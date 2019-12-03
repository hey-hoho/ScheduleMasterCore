using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Hos.ScheduleMaster.Web.Infrastructure
{
    public class SystemSchedulerRegistry : Registry
    {
        public SystemSchedulerRegistry()
        {
            NonReentrantAsDefault();

            //对运行节点每分钟一次心跳监测
            Schedule<WorkerCheckJob>().ToRunEvery(1).Minutes();
        }
    }

    internal class WorkerCheckJob : IJob
    {
        /// <summary>
        /// 执行计划
        /// </summary>
        public void Execute()
        {
            using (var scope = ConfigurationCache.RootServiceProvider.CreateScope())
            {
                Core.Services.ScheduleService service = new Core.Services.ScheduleService();
                AutowiredServiceProvider provider = new AutowiredServiceProvider();
                provider.PropertyActivate(service, scope.ServiceProvider);
                service.NodeCheck();
            }
        }
    }
}
