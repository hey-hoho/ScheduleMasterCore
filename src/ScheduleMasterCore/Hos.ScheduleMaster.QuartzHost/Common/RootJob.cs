
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
using Hos.ScheduleMaster.Core.Common;

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
        RunTracer _tracer;
        string node = ConfigurationCache.NodeSetting.IdentityName;

        public async Task Execute(IJobExecutionContext context)
        {
            _sid = Guid.Parse(context.JobDetail.Key.Name);

            using (var scope = new ScopeDbContext())
            {
                _tracer = scope.GetService<RunTracer>();
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
                Guid traceId = Guid.NewGuid();

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
                    await _tracer.Begin(traceId, context.JobDetail.Key.Name);

                    //执行
                    OnExecuting(tctx);

                    double elapsed = await _tracer.Complete(ScheduleRunResult.Success);

                    LogHelper.Info($"任务[{instance.Main.Title}]运行成功！用时{elapsed.ToString()}ms", _sid, traceId);
                    //保存运行结果用于子任务触发
                    context.Result = tctx.Result;
                }
                catch (RunConflictException conflict)
                {
                    await _tracer.Complete(ScheduleRunResult.Conflict);
                    throw conflict;
                }
                catch (Exception e)
                {
                    await _tracer.Complete(ScheduleRunResult.Failed);

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

    }
}