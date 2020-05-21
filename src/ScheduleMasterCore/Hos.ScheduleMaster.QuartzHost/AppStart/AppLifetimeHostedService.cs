using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.AppStart
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

            string pluginBasePath = ConfigurationCache.PluginPathPrefix.ToPhysicalPath();
            if (!System.IO.Directory.Exists(pluginBasePath))
            {
                System.IO.Directory.CreateDirectory(pluginBasePath);
            }

            //加载全局缓存
            ConfigurationCache.Reload();
            //初始化日志管理器
            Core.Log.LogManager.Init();
            //判断是否要自动根据配置文件注册节点信息
            if (AppCommandResolver.IsAutoRegister())
            {
                _logger.LogInformation("enabled auto register...");
                //设置节点信息
                ConfigurationCache.SetNode(_configuration);
                //初始化Quartz
                Common.QuartzManager.InitScheduler().Wait();
                //初始化延时队列容器
                DelayedTask.DelayPlanManager.Init();
            }
        }

        private void OnStopping()
        {
            _logger.LogInformation("Hosted service OnStopping");

            Common.QuartzManager.Shutdown(true).Wait();
            DelayedTask.DelayPlanManager.Clear();
            Core.Log.LogManager.Shutdown();
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
