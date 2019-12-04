using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.QuartzHost.Common;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.AppStart
{
    /// <summary>
    /// 清理那些已经停止但是quartz里面还在运行的任务
    /// </summary>
    public class TaskClearJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = new ScopeDbContext())
            {
                var _db = scope.GetDbContext();
                var stoppedList = _db.Schedules.Where(x => x.Status == (int)ScheduleStatus.Stop).Select(x => x.Id).ToList();
                foreach (var sid in stoppedList)
                {
                    JobKey jk = new JobKey(sid.ToString().ToLower());
                    if (await context.Scheduler.CheckExists(jk))
                    {
                        await QuartzManager.Stop(sid);
                    }
                }
            }
        }
    }
}
