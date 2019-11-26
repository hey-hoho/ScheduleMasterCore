
using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
        public Task Execute(IJobExecutionContext context)
        {
            Guid taskId = Guid.Parse(context.JobDetail.Key.Name);

            var node = ConfigurationManager.AppSettings.Get("HostIdentity");
            bool getLocked = false;
            try
            {
                //getLocked = SQLHelper.ExecuteNonQuery($"INSERT INTO TaskLocks(TaskId,Status) values('{taskId.ToString()}',1)") > 0;
            }
            catch (Exception)
            {
                //getLocked = SQLHelper.ExecuteNonQuery($"UPDATE TaskLocks SET Status=1 WHERE TaskId='{taskId.ToString()}' and Status=0") > 0;
            }
            if (getLocked)
            {
                IJobDetail job = context.JobDetail;
                try
                {
                    LogHelper.Info($"节点{node}抢锁成功！准备执行任务....", taskId);
                    var instance = job.JobDataMap["instance"] as TaskBase;
                    if (instance != null)
                    {
                        TaskContext tctx = new TaskContext(instance);
                        tctx.Node = node;
                        tctx.TaskId = taskId;

                        var param = job.JobDataMap["params"];
                        if (param != null)
                        {
                            tctx.CustomParamsJson = param.ToString();
                        }

                        instance.InnerRun(tctx);
                        LogHelper.Info($"任务\"{job.JobDataMap["name"]}\"运行成功！", taskId);
                    }
                }
                catch (Exception exp)
                {
                    LogHelper.Error($"任务\"{job.JobDataMap["name"]}\"运行失败！", exp, taskId);
                    //这里抛出的异常会在JobListener的JobWasExecuted事件中接住
                    //如果吃掉异常会导致程序误以为本次任务执行成功
                    throw new JobExecutionException(exp);
                }
                finally
                {
                    //为了避免各节点之间的时间差，延迟1秒释放锁
                    System.Threading.Thread.Sleep(1000);
                    //SQLHelper.ExecuteNonQuery($"UPDATE TaskLocks SET Status=0 WHERE TaskId='{taskId.ToString()}'");
                }
            }
            else
            {
                LogHelper.Info($"节点{node}抢锁失败！", taskId);
                throw new JobExecutionException("lock_failed");
            }
            return Task.FromResult(0);
        }
    }
}