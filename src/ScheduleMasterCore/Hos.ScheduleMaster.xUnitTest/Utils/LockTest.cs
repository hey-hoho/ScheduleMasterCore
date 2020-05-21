using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Hos.ScheduleMaster.xUnitTest.Utils
{
    public class LockTest
    {
        [Fact]
        public void DatabaseLock()
        {
            IServiceCollection services = new ServiceCollection();
            //EF数据库上下文
            services.AddDbContext<SmDbContext>(option => option.UseMySql("Data Source=192.168.8.27;Database=schedule_master;User ID=root;Password=123456;pooling=true;CharSet=utf8;port=3306;sslmode=none;TreatTinyAsBoolean=true"), ServiceLifetime.Transient);
            services.AddTransient<QuartzHost.HosLock.IHosLock, QuartzHost.HosLock.DatabaseLock>();
            var serviceProvider = services.BuildServiceProvider();

            Core.ConfigurationCache.SetNode(new ServerNodeEntity { NodeName = "xUnitTest", Host = "localhost:996" });

            int stock = 1;
            Task[] tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                tasks[i] = new Task(() =>
                {
                    using (var locker = serviceProvider.GetService<QuartzHost.HosLock.IHosLock>())
                    {
                        if (locker.TryGetLock("4f322d68-8d21-44f2-9f76-60ed7a144e25"))
                        {
                            stock--;
                        }
                    }
                }, TaskCreationOptions.LongRunning);
                tasks[i].Start();
            }
            Task.WaitAll(tasks);

            Assert.Equal(0, stock);
        }
    }
}
