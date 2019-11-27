
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
        SmDbContext _db;
        Core.Interface.IScheduleService service;


        public Task Execute(IJobExecutionContext context)
        {
            Guid sid = Guid.Parse(context.JobDetail.Key.Name);

            var node = "1";// ConfigurationManager.AppSettings.Get("HostIdentity");
            bool getLocked = false;
            using (var serviceScope = Core.ConfigurationCache.RootServiceProvider.CreateScope())
            {
                _db = serviceScope.ServiceProvider.GetRequiredService<SmDbContext>();
                service = serviceScope.ServiceProvider.GetRequiredService<Core.Interface.IScheduleService>();
                try
                {
                    getLocked = _db.Database.ExecuteSqlRaw($"INSERT INTO schedulelocks(ScheduleId,Status) values('{sid.ToString()}',1)") > 0;
                }
                catch (Exception)
                {
                    getLocked = _db.Database.ExecuteSqlRaw($"UPDATE schedulelocks SET Status=1 WHERE ScheduleId='{sid.ToString()}' and Status=0") > 0;
                }
                if (getLocked)
                {
                    IJobDetail job = context.JobDetail;
                    try
                    {
                        LogHelper.Info($"节点{node}抢锁成功！准备执行任务....", sid);
                        var instance = job.JobDataMap["instance"] as TaskBase;
                        if (instance != null)
                        {
                            Guid traceId = service.AddRunTrace(sid);
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Restart();
                            TaskContext tctx = new TaskContext(instance);
                            tctx.Node = node;
                            tctx.TaskId = sid;

                            var param = job.JobDataMap["params"];
                            if (param != null)
                            {
                                tctx.CustomParamsJson = param.ToString();
                            }

                            try
                            {
                                instance.InnerRun(tctx);
                                stopwatch.Stop();
                                service.UpdateRunTrace(traceId, stopwatch.Elapsed.TotalSeconds, ScheduleRunResult.Success);
                                LogHelper.Info($"任务[{job.JobDataMap["name"]}]运行成功！用时{stopwatch.Elapsed.Milliseconds.ToString()}ms", sid);
                                //保存运行结果用于子任务触发
                                context.Result = tctx.Result;
                            }
                            catch (RunConflictException conflict)
                            {
                                stopwatch.Stop();
                                service.UpdateRunTrace(traceId, stopwatch.Elapsed.TotalSeconds, ScheduleRunResult.Conflict);
                                throw conflict;
                            }
                            catch (Exception e)
                            {
                                stopwatch.Stop();
                                service.UpdateRunTrace(traceId, stopwatch.Elapsed.TotalSeconds, ScheduleRunResult.Failed);
                                throw new BusinessRunException(e);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        LogHelper.Error($"任务\"{job.JobDataMap["name"]}\"运行失败！", exp, sid);
                        //这里抛出的异常会在JobListener的JobWasExecuted事件中接住
                        //如果吃掉异常会导致程序误以为本次任务执行成功
                        throw new JobExecutionException(exp);
                    }
                    finally
                    {
                        //为了避免各节点之间的时间差，延迟1秒释放锁
                        System.Threading.Thread.Sleep(1000);
                        _db.Database.ExecuteSqlRaw($"UPDATE schedulelocks SET Status=0 WHERE ScheduleId='{sid.ToString()}'");
                    }
                }
                else
                {
                    LogHelper.Info($"节点{node}抢锁失败！", sid);
                    throw new JobExecutionException("lock_failed");
                }
            }
            return Task.FromResult(0);
        }
    }
}