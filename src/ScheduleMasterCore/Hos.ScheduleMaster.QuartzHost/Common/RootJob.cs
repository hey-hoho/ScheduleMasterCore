
using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.QuartzHost.HosSchedule;

namespace Hos.ScheduleMaster.QuartzHost.Common
{
    /// <summary>
    /// 这个是quartz直接调用的公共job
    /// by hoho
    /// </summary>
    //禁止多实例并发执行
    [DisallowConcurrentExecution]
    public abstract class RootJob : IJob
    {
        Guid _sid;
        SmDbContext _db;
        string node = ConfigurationCache.NodeSetting.IdentityName;

        public async Task Execute(IJobExecutionContext context)
        {
            _sid = Guid.Parse(context.JobDetail.Key.Name);

            using (var scope = new ScopeDbContext())
            {
                _db = scope.GetDbContext();
                var locker = scope.GetService<HosLock.IHosLock>();
                if (locker.TryGetLock(context.JobDetail.Key.Name))
                {
                    await InnerRun(context);
                }
                else
                {
                    throw new JobExecutionException("lock_failed");
                }
            }
        }

        private async Task InnerRun(IJobExecutionContext context)
        {
            IJobDetail job = context.JobDetail;
            if (job.JobDataMap["instance"] is IHosSchedule instance)
            {
                Guid traceId = await CreateRunTrace();
                Stopwatch stopwatch = new Stopwatch();
                TaskContext tctx = new TaskContext(instance.RunnableInstance);
                tctx.Node = node;
                tctx.TraceId = traceId;
                tctx.ParamsDict = instance.CustomParams;
                if (context.MergedJobDataMap["PreviousResult"] is object prev)
                {
                    tctx.PreviousResult = prev;
                }
                try
                {
                    stopwatch.Restart();
                    //执行
                    OnExecuting(tctx);
                    stopwatch.Stop();
                    //更新执行结果
                    await UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Success);
                    LogHelper.Info($"任务[{instance.Main.Title}]运行成功！用时{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 3).ToString()}ms", _sid, traceId);
                    //保存运行结果用于子任务触发
                    context.Result = tctx.Result;
                }
                catch (RunConflictException conflict)
                {
                    stopwatch.Stop();
                    await UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Conflict);
                    throw conflict;
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    await UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Failed);
                    LogHelper.Error($"任务\"{instance.Main.Title}\"运行失败！", e, _sid, traceId);
                    //这里抛出的异常会在JobListener的JobWasExecuted事件中接住
                    //如果吃掉异常会导致程序误以为本次任务执行成功
                    throw new BusinessRunException(e);
                }
                finally
                {
                    OnExecuted(tctx);
                }
            }
        }

        public abstract void OnExecuting(TaskContext context);

        public abstract void OnExecuted(TaskContext context);

        private async Task<Guid> CreateRunTrace()
        {
            ScheduleTraceEntity entity = new ScheduleTraceEntity();
            entity.TraceId = Guid.NewGuid();
            entity.ScheduleId = _sid;
            entity.Node = node;
            entity.StartTime = DateTime.Now;
            entity.Result = (int)ScheduleRunResult.Null;
            _db.ScheduleTraces.Add(entity);
            if (await _db.SaveChangesAsync() > 0)
            {
                return entity.TraceId;
            }
            return Guid.Empty;
        }

        private async Task UpdateRunTrace(Guid traceId, double elapsed, ScheduleRunResult result)
        {
            if (traceId == Guid.Empty) return;
            await _db.Database.ExecuteSqlRawAsync($"update scheduletraces set result={(int)result},elapsedtime={elapsed},endtime=now() where traceid='{traceId}'");
        }
    }
}