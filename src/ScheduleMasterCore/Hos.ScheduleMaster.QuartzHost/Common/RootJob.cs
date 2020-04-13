
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

        public Task Execute(IJobExecutionContext context)
        {
            _sid = Guid.Parse(context.JobDetail.Key.Name);

            using (var scope = new ScopeDbContext())
            {
                _db = scope.GetDbContext();
                bool getLocked = _db.Database.ExecuteSqlRaw($"update schedulelocks set status=1,lockedtime=now(),lockednode='{node}' where scheduleid='{_sid.ToString()}' and status=0") > 0;
                if (getLocked)
                {
                    LogHelper.Info($"节点{node}抢锁成功！准备执行任务....", _sid);
                    IJobDetail job = context.JobDetail;
                    try
                    {
                        if (job.JobDataMap["instance"] is IHosSchedule instance)
                        {
                            Guid traceId = GreateRunTrace();
                            Stopwatch stopwatch = new Stopwatch();
                            TaskContext tctx = new TaskContext(instance.RunnableInstance);
                            tctx.Node = node;
                            tctx.TaskId = _sid;
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
                                UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Success);
                                LogHelper.Info($"任务[{instance.Main.Title}]运行成功！用时{Math.Round(stopwatch.Elapsed.TotalMilliseconds, 3).ToString()}ms", _sid, traceId);
                                //保存运行结果用于子任务触发
                                context.Result = tctx.Result;
                            }
                            catch (RunConflictException conflict)
                            {
                                stopwatch.Stop();
                                UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Conflict);
                                throw conflict;
                            }
                            catch (Exception e)
                            {
                                stopwatch.Stop();
                                UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Failed);
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
                    finally
                    {
                        //为了避免各节点之间的时间差，延迟1秒释放锁
                        System.Threading.Thread.Sleep(1000);
                        _db.Database.ExecuteSqlRaw($"update schedulelocks set status=0,lockedtime=null,lockednode=null where scheduleid='{_sid.ToString()}'");
                    }
                }
                else
                {
                    //LogHelper.Info($"节点{node}抢锁失败！", _sid);
                    throw new JobExecutionException("lock_failed");
                }
            }
            return Task.CompletedTask;
        }

        public abstract void OnExecuting(TaskContext context);

        public abstract void OnExecuted(TaskContext context);

        private Guid GreateRunTrace()
        {
            ScheduleTraceEntity entity = new ScheduleTraceEntity();
            entity.TraceId = Guid.NewGuid();
            entity.ScheduleId = _sid;
            entity.Node = node;
            entity.StartTime = DateTime.Now;
            entity.Result = (int)ScheduleRunResult.Null;
            _db.ScheduleTraces.Add(entity);
            if (_db.SaveChanges() > 0)
            {
                return entity.TraceId;
            }
            return Guid.Empty;
        }

        private void UpdateRunTrace(Guid traceId, double elapsed, ScheduleRunResult result)
        {
            if (traceId == Guid.Empty) return;
            _db.Database.ExecuteSqlRaw($"update scheduletraces set result={(int)result},elapsedtime={elapsed},endtime=now() where traceid='{traceId}'");
        }
    }
}