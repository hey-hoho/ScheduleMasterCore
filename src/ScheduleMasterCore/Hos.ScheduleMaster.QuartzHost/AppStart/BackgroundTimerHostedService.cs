using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.AppStart
{
    public abstract class BackgroundTimerHostedService : IHostedService, IDisposable
    {
        private readonly Timer _timer;
        private readonly TimeSpan _period;
        protected readonly ILogger Logger;

        protected BackgroundTimerHostedService(TimeSpan period, ILogger logger)
        {
            Logger = logger;
            _period = period;
            _timer = new Timer(Execute, null, Timeout.Infinite, 0);
        }

        public void Execute(object state = null)
        {
            try
            {
                //Logger.LogInformation("Begin execute service");
                ExecuteAsync().Wait();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Execute exception");
            }
            finally
            {
                //Logger.LogInformation("Execute finished");
            }
        }

        protected abstract Task ExecuteAsync();

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Logger.LogInformation("Service is starting.");
            _timer.Change(TimeSpan.FromSeconds(new Random().Next(10)), _period);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Logger.LogInformation("Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }

    public class ConfigurationRefreshService : BackgroundTimerHostedService
    {
        ILogger<ConfigurationRefreshService> _logger;
        public ConfigurationRefreshService(ILogger<ConfigurationRefreshService> logger) : base(TimeSpan.FromMinutes(10), logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync()
        {
            Core.ConfigurationCache.Reload();
            _logger.LogInformation($"ConfigurationRefresh Finished.");
            return Task.CompletedTask;
        }
    }

    public class DelayedTaskConsumerService : BackgroundTimerHostedService
    {
        ILogger<DelayedTaskConsumerService> _logger;
        public DelayedTaskConsumerService(ILogger<DelayedTaskConsumerService> logger) : base(TimeSpan.FromSeconds(1), logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync()
        {
            if (DelayedTask.DelayPlanManager.IsEnabled)
            {
                DelayedTask.DelayPlanManager.Read();
            }
            else
            {
                _logger.LogWarning($"DelayPlanManager Is Unenabled.");
            }
            return Task.CompletedTask;
        }
    }
}
