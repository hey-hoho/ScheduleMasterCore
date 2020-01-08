
using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Hos.ScheduleMaster.Core;

namespace Hos.ScheduleMaster.QuartzHost.Common
{
    /// <summary>
    /// 这个是quartz直接调用的公共job
    /// by hoho
    /// </summary>
    //禁止多实例并发执行
    [DisallowConcurrentExecution]
    public class RootJob : IJob
    {
        Guid _sid;
        SmDbContext _db;
        string node = ConfigurationCache.NodeSetting.IdentityName;

        public Task Execute(IJobExecutionContext context)
        {
            _sid = Guid.Parse(context.JobDetail.Key.Name);

            bool getLocked = false;
            using (var scope = new ScopeDbContext())
            {
                _db = scope.GetDbContext();
                if (!_db.Schedules.Any(x => x.Id == _sid && x.Status == (int)ScheduleStatus.Running))
                {
                    LogHelper.Warn("不存在或没有启动的任务", _sid);
                    throw new JobExecutionException("不存在或没有启动的任务");
                }
                try
                {
                    getLocked = _db.Database.ExecuteSqlRaw($"INSERT INTO schedulelocks(ScheduleId,Status) values('{_sid.ToString()}',1)") > 0;
                }
                catch (Exception)
                {
                    getLocked = _db.Database.ExecuteSqlRaw($"UPDATE schedulelocks SET Status=1 WHERE ScheduleId='{_sid.ToString()}' and Status=0") > 0;
                }
                if (getLocked)
                {
                    LogHelper.Info($"节点{node}抢锁成功！准备执行任务....", _sid);
                    IJobDetail job = context.JobDetail;
                    try
                    {
                        if (job.JobDataMap["instance"] is TaskBase instance)
                        {
                            Guid traceId = GreateRunTrace();
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Restart();
                            TaskContext tctx = new TaskContext(instance);
                            tctx.Node = node;
                            tctx.TaskId = _sid;
                            tctx.TraceId = traceId;
                            tctx.CustomParamsJson = job.JobDataMap["params"]?.ToString();
                            if (context.MergedJobDataMap["PreviousResult"] is object prev)
                            {
                                tctx.PreviousResult = prev;
                            }
                            try
                            {
                                instance.InnerRun(tctx);
                                stopwatch.Stop();
                                UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Success);
                                LogHelper.Info($"任务[{job.JobDataMap["name"]}]运行成功！用时{stopwatch.Elapsed.Milliseconds.ToString()}ms", _sid, traceId);
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
                                LogHelper.Error($"任务\"{job.JobDataMap["name"]}\"运行失败！", e, _sid, traceId);
                                throw new BusinessRunException(e);
                            }
                        }
                    }
                    //catch (Exception exp)
                    //{
                    //    //这里抛出的异常会在JobListener的JobWasExecuted事件中接住
                    //    //如果吃掉异常会导致程序误以为本次任务执行成功
                    //    throw new JobExecutionException(exp);
                    //}
                    finally
                    {
                        //为了避免各节点之间的时间差，延迟1秒释放锁
                        System.Threading.Thread.Sleep(1000);
                        _db.Database.ExecuteSqlRaw($"UPDATE schedulelocks SET Status=0 WHERE ScheduleId='{_sid.ToString()}'");
                    }
                }
                else
                {
                    //LogHelper.Info($"节点{node}抢锁失败！", _sid);
                    throw new JobExecutionException("lock_failed");
                }
            }
            return Task.FromResult(0);
        }

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
            _db.Database.ExecuteSqlRaw($"update scheduletraces set result={(int)result},elapsedtime={elapsed},endtime='{DateTime.Now}' where traceid='{traceId}'");
        }
    }
}