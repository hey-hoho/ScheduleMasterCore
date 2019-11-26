
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Log
{
    public class LogManager
    {
        public static BufferQueue<SystemLogEntity> Queue;

        public static void Init()
        {
            Queue = new BufferQueue<SystemLogEntity>();
            var td = new System.Threading.Thread(() =>
            {
                using (var serviceScope = ConfigurationCache.RootServiceProvider.CreateScope())
                {
                    var db = serviceScope.ServiceProvider.GetRequiredService<SmDbContext>();
                    while (true)
                    {
                        Queue.Read((item, index) =>
                        {
                            db.SystemLogs.Add(item);
                        });
                        db.SaveChanges();
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            });
            td.IsBackground = true;
            td.Start();
        }

        public static void Shutdown()
        {
            Queue.Clear();
        }
    }
}
