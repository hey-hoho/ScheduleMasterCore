
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                using (var scope = new ScopeDbContext())
                {
                    var db = scope.GetDbContext();
                    db.ChangeTracker.AutoDetectChangesEnabled = false;
                    while (true)
                    {
                        Queue.Read((item, index) =>
                        {
                            item.Node = ConfigurationCache.NodeSetting?.IdentityName;
                            db.SystemLogs.Add(item);
                        });
                        if (db.ChangeTracker.HasChanges())
                        {
                            db.SaveChanges();
                        }
                        foreach (var item in db.ChangeTracker.Entries())
                        {
                            item.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                        }
                        System.Threading.Thread.Sleep(5000);
                    }
                }
            });
            td.IsBackground = true;
            td.Start();
        }

        public static void Shutdown()
        {
            if (Queue != null) { Queue.Clear(); }
        }
    }
}
