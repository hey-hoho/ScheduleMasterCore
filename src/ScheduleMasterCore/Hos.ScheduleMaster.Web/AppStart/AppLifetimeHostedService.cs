using Hos.ScheduleMaster.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.AppStart
{
    public class AppLifetimeHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;

        private readonly IConfiguration _configuration;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<AppLifetimeHostedService> _logger;

        public AppLifetimeHostedService(
            IHostApplicationLifetime appLifetime,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<AppLifetimeHostedService> logger)
        {
            _appLifetime = appLifetime;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("Hosted service OnStarted");

            //读取节点配置信息
            ConfigurationCache.SetNode(_configuration);
            //注册节点
            AppStart.NodeRegistry.Register();
            //加载缓存
            ConfigurationCache.Reload();
            //初始化日志管理器
            Core.Log.LogManager.Init();

            //初始化系统任务
            FluentScheduler.JobManager.Initialize(new AppStart.SystemSchedulerRegistry());
            FluentScheduler.JobManager.JobException += info => Core.Log.LogHelper.Error("An error just happened with a FluentScheduler job", info.Exception);
            //任务恢复
            using (var scope = _serviceProvider.CreateScope())
            {
                Core.Services.ScheduleService service = new Core.Services.ScheduleService();
                AutowiredServiceProvider provider = new AutowiredServiceProvider();
                provider.PropertyActivate(service, scope.ServiceProvider);
                service.RunningRecovery();
            }
        }

        private void OnStopping()
        {
            _logger.LogInformation("Hosted service OnStopping");

            Core.Log.LogManager.Shutdown();
            AppStart.NodeRegistry.Shutdown();

        }

        private void OnStopped()
        {
            _logger.LogInformation("Hosted service OnStopped");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted service StopAsync");
            return Task.CompletedTask;
        }
    }
}
