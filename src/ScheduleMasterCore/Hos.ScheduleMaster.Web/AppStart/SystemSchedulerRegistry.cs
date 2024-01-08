using FluentScheduler;
using Hos.ScheduleMaster.Core;

namespace Hos.ScheduleMaster.Web.AppStart
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
                var service = scope.ServiceProvider.GetService<Core.Interface.INodeService>();
                var provider = new AutowiredServiceProvider();
                provider.PropertyActivate(service, scope.ServiceProvider);
                service.WorkerHealthCheck();
            }
        }
    }
}
