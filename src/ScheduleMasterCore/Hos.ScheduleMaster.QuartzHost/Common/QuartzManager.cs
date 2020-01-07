
using Hos.ScheduleMaster.Core.EntityFramework;
using Hos.ScheduleMaster.Core.Log;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Base;
using System.Threading.Tasks;
using System.Threading;
using Hos.ScheduleMaster.Core.Common;
using Microsoft.EntityFrameworkCore;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Dto;

namespace Hos.ScheduleMaster.QuartzHost.Common
{
    public class QuartzManager
    {
        /// <summary>
        /// worker访问秘钥
        /// </summary>
        public static string AccessSecret { get; private set; }

        private QuartzManager() { }

        /// <summary>
        /// 调度器实例
        /// </summary>
        private static IScheduler _scheduler = null;

        /// <summary>
        /// 初始化调度系统
        /// </summary>
        public static async Task InitScheduler()
        {
            try
            {
                if (_scheduler == null)
                {
                    NameValueCollection properties = new NameValueCollection();
                    properties["quartz.scheduler.instanceName"] = "Hos.SchefuleMaster";
                    properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                    properties["quartz.threadPool.threadCount"] = "50";
                    properties["quartz.threadPool.threadPriority"] = "Normal";
                    ISchedulerFactory factory = new StdSchedulerFactory(properties);
                    _scheduler = await factory.GetScheduler();
                }
                await _scheduler.Start();
                await _scheduler.Clear();
                MarkNode(true);
                LogHelper.Info("任务调度平台初始化成功！");
            }
            catch (Exception ex)
            {
                LogHelper.Error("任务调度平台初始化失败！", ex);
            }
        }

        /// <summary>
        /// 更新节点状态
        /// </summary>
        /// <param name="isStarted"></param>
        /// <param name="isOnStop"></param>
        private static void MarkNode(bool isStarted, bool isOnStop = false)
        {
            var setting = ConfigurationCache.NodeSetting;
            using (var scope = new Core.ScopeDbContext())
            {
                bool isCreate = false;
                var db = scope.GetDbContext();
                var node = db.ServerNodes.FirstOrDefault(x => x.NodeName == setting.IdentityName);
                if (isStarted)
                {
                    if (node == null)
                    {
                        isCreate = true;
                        node = new ServerNodeEntity();
                    }
                    node.NodeName = setting.IdentityName;
                    node.NodeType = setting.Role;
                    node.MachineName = Environment.MachineName;
                    node.AccessProtocol = setting.Protocol;
                    node.Host = $"{setting.IP}:{setting.Port}";
                    node.Priority = setting.Priority;
                    node.Status = 2;
                    node.AccessSecret = Guid.NewGuid().ToString("n");
                    if (isCreate) db.ServerNodes.Add(node);
                }
                else
                {
                    if (node != null)
                    {
                        node.Status = isOnStop ? 0 : 1;
                        if (isOnStop) node.AccessSecret = null;
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    AccessSecret = node.AccessSecret;
                }
            }
        }

        /// <summary>
        /// 关闭调度系统
        /// </summary>
        public static async Task Shutdown(bool isOnStop = false)
        {
            try
            {
                //判断调度是否已经关闭
                if (!_scheduler.IsShutdown)
                {
                    //等待任务运行完成再关闭调度
                    await _scheduler.Shutdown(true);
                    MarkNode(false, isOnStop);
                    LogHelper.Info("任务调度平台已经停止！");
                    _scheduler = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("任务调度平台停止失败！", ex);
            }
        }

        /// <summary>
        /// 启动一个任务，带重试机制
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static async Task<bool> StartWithRetry(ScheduleView view, Action<Guid, DateTime?> callBack)
        {
            PluginLoadContext lc = null;
            try
            {
                lc = AssemblyHelper.LoadAssemblyContext(view.Schedule.Id, view.Schedule.AssemblyName);
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        await Start(view, lc, callBack);
                        return true;
                    }
                    catch (SchedulerException sexp)
                    {
                        LogHelper.Error($"任务启动失败！开始第{i + 1}次重试...", sexp, view.Schedule.Id);
                    }
                }
                //最后一次尝试
                await Start(view, lc, callBack);
                return true;
            }
            catch (SchedulerException sexp)
            {
                AssemblyHelper.UnLoadAssemblyLoadContext(lc);
                LogHelper.Error($"任务所有重试都失败了，已放弃启动！", sexp, view.Schedule.Id);
                return false;
            }
            catch (Exception exp)
            {
                AssemblyHelper.UnLoadAssemblyLoadContext(lc);
                LogHelper.Error($"任务启动失败！", exp, view.Schedule.Id);
                return false;
            }
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static async Task<bool> Pause(Guid sid)
        {
            try
            {
                JobKey jk = new JobKey(sid.ToString().ToLower());
                if (await _scheduler.CheckExists(jk))
                {
                    //任务已经存在则暂停任务
                    await _scheduler.PauseJob(jk);
                    var jobDetail = await _scheduler.GetJobDetail(jk);
                    if (jobDetail.JobType.GetInterface("IInterruptableJob") != null)
                    {
                        await _scheduler.Interrupt(jk);
                    }
                    LogHelper.Warn($"任务已经暂停运行！", sid);
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"任务暂停运行失败！", exp, sid);
                return false;
            }
        }

        /// <summary>
        /// 恢复运行
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static async Task<bool> Resume(Guid sid)
        {
            try
            {
                JobKey jk = new JobKey(sid.ToString().ToLower());
                if (await _scheduler.CheckExists(jk))
                {
                    //恢复任务继续执行
                    await _scheduler.ResumeJob(jk);
                    LogHelper.Info($"任务已经恢复运行！", sid);
                    return true;
                }
                return false;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"任务恢复运行失败！", exp, sid);
                return false;
            }
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static async Task<bool> Stop(Guid sid)
        {
            try
            {
                JobKey jk = new JobKey(sid.ToString().ToLower());
                var job = await _scheduler.GetJobDetail(jk);
                if (job != null)
                {
                    var instance = job.JobDataMap["instance"] as TaskBase;
                    //释放资源
                    if (instance != null)
                    {
                        instance.Dispose();
                    }
                    //卸载应用程序域
                    var domain = job.JobDataMap["domain"] as PluginLoadContext;
                    AssemblyHelper.UnLoadAssemblyLoadContext(domain);
                    //删除quartz有关设置
                    var trigger = new TriggerKey(sid.ToString());
                    await _scheduler.PauseTrigger(trigger);
                    await _scheduler.UnscheduleJob(trigger);
                    await _scheduler.DeleteJob(jk);
                    _scheduler.ListenerManager.RemoveJobListener(sid.ToString());
                }
                LogHelper.Info($"任务已经停止运行！", sid);
                return true;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"任务停止失败！", exp, sid);
                return false;
            }
        }

        /// <summary>
        ///立即运行一次任务
        /// </summary>
        /// <param name="sid"></param>
        public static async Task<bool> RunOnce(Guid sid)
        {
            JobKey jk = new JobKey(sid.ToString().ToLower());
            if (await _scheduler.CheckExists(jk))
            {
                await _scheduler.TriggerJob(jk);
                return true;
                //var jobDetail = await _scheduler.GetJobDetail(jk);
                //var instance = jobDetail.JobDataMap["instance"] as TaskBase;
                //try
                //{
                //    if (instance != null)
                //    {
                //        instance.TaskId = taskId;

                //        var param = jobDetail.JobDataMap["params"];
                //        if (param != null)
                //        {
                //            instance.CustomParamsJson = param.ToString();
                //        }
                //        instance.InnerRun();
                //        LogHelper.Info(string.Format("任务[{0}]立即运行成功！", jobDetail.JobDataMap["name"]), taskId);
                //        return true;
                //    }
                //    else
                //    {
                //        LogHelper.Error($"instance=null", taskId);
                //    }
                //}
                //catch (Exception exp)
                //{
                //    LogHelper.Error($"任务[{jobDetail.JobDataMap["name"]}]运行失败！", exp, taskId);
                //}
                //var triggers = _scheduler.GetTriggersOfJob(jk);
                //string taskName = JobKey;
                //if (triggers != null && triggers.Count > 0)
                //{
                //    taskName = triggers[0].Description;
                //}
                //var type = jobDetail.JobType;
                //var instance = type.FastNew();
                //var method = type.GetMethod("Execute");
                //method.Invoke(instance, new object[] { null });
            }
            else
            {
                LogHelper.Error($"_scheduler.CheckExists=false", sid);
            }
            return false;
        }

        /// <summary>
        /// 执行自定义任务类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <param name="cronExp"></param>
        /// <returns></returns>
        public static async Task Start<T>(string identity, string cronExp) where T : IJob
        {
            IJobDetail job = JobBuilder.Create<T>().WithIdentity(identity).Build();
            CronTriggerImpl trigger = new CronTriggerImpl
            {
                CronExpressionString = cronExp,
                Name = identity,
                Key = new TriggerKey(identity)
            };
            trigger.StartTimeUtc = DateTimeOffset.Now;
            await _scheduler.ScheduleJob(job, trigger);
        }

        #region 私有方法

        private static async Task Start(ScheduleView view, PluginLoadContext lc, Action<Guid, DateTime?> callBack)
        {
            //throw new SchedulerException("SchedulerException");

            //在应用程序域中创建实例返回并保存在job中，这是最终调用任务执行的实例
            TaskBase instance = AssemblyHelper.CreateTaskInstance(lc, view.Schedule.Id, view.Schedule.AssemblyName, view.Schedule.ClassName);
            if (instance == null)
            {
                throw new InvalidCastException($"任务实例创建失败，请确认目标任务是否派生自TaskBase类型。程序集：{view.Schedule.AssemblyName}，类型：{view.Schedule.ClassName}");
            }
            // instance.logger = new LogWriter(); ;
            JobDataMap map = new JobDataMap
            {
                new KeyValuePair<string, object> ("domain",lc),
                new KeyValuePair<string, object> ("instance",instance),
                new KeyValuePair<string, object> ("name",view.Schedule.Title),
                new KeyValuePair<string, object> ("params",ConvertParamsJson(view.Schedule.CustomParamsJson)),
                new KeyValuePair<string, object> ("keepers",view.Keepers),
                new KeyValuePair<string, object> ("children",view.Children)
            };
            IJobDetail job = JobBuilder.Create<RootJob>()
                .WithIdentity(view.Schedule.Id.ToString())
                .UsingJobData(map)
                //.UsingJobData("assembly", task.AssemblyName)
                //.UsingJobData("class", task.ClassName)
                .Build();

            //添加触发器
            _scheduler.ListenerManager.AddJobListener(new JobRunListener(view.Schedule.Id.ToString(), callBack),
                KeyMatcher<JobKey>.KeyEquals(new JobKey(view.Schedule.Id.ToString())));

            if (view.Schedule.RunLoop)
            {
                if (!CronExpression.IsValidExpression(view.Schedule.CronExpression))
                {
                    throw new Exception("cron表达式验证失败");
                }
                CronTriggerImpl trigger = new CronTriggerImpl
                {
                    CronExpressionString = view.Schedule.CronExpression,
                    Name = view.Schedule.Title,
                    Key = new TriggerKey(view.Schedule.Id.ToString()),
                    Description = view.Schedule.Remark
                };
                if (view.Schedule.StartDate.HasValue)
                {
                    trigger.StartTimeUtc = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.StartDate.Value);
                }
                if (view.Schedule.EndDate.HasValue)
                {
                    trigger.EndTimeUtc = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.EndDate.Value);
                }
                await _scheduler.ScheduleJob(job, trigger);
            }
            else
            {
                DateTimeOffset start = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
                if (view.Schedule.StartDate.HasValue)
                {
                    start = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.StartDate.Value);
                }
                DateTimeOffset end = start.AddMinutes(1);
                if (view.Schedule.EndDate.HasValue)
                {
                    end = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.EndDate.Value);
                }
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(view.Schedule.Id.ToString())
                    .StartAt(start)
                    .WithSimpleSchedule(x => x
                    .WithRepeatCount(1).WithIntervalInMinutes(1))
                    .EndAt(end)
                    .Build();
                await _scheduler.ScheduleJob(job, trigger);
            }

            LogHelper.Info($"任务[{view.Schedule.Title}]启动成功！", view.Schedule.Id);

            _ = Task.Run(() =>
              {
                  while (true)
                  {
                      var log = instance.ReadLog();
                      if (log != null)
                      {
                          LogManager.Queue.Write(new SystemLogEntity
                          {
                              Category = log.Category,
                              Message = log.Message,
                              ScheduleId = log.ScheduleId,
                              Node = log.Node,
                              StackTrace = log.StackTrace,
                              TraceId = log.TraceId,
                              CreateTime = log.CreateTime
                          });
                      }
                      else
                      {
                          Thread.Sleep(3000);
                      }
                  }
              });
        }

        private static string ConvertParamsJson(string source)
        {
            List<ScheduleParam> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScheduleParam>>(source);
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var item in list)
            {
                result[item.ParamKey] = item.ParamValue;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }
        #endregion

        #region 邮件模板

        public static string GetErrorEmailContent(string sname, Exception ex)
        {
            string EmailTemplate = "<div style=\"background-color:#d0d0d0;text-align:center;padding:40px;font-family:'微软雅黑','黑体', 'Lucida Grande', Verdana, sans-serif;\"><div style=\"width:700px;margin:0 auto;padding:10px;color:#333;background-color:#fff;border:0px solid #aaa;border-radius:5px;-webkit-box-shadow:3px 3px 10px #999;-moz-box-shadow:3px 3px 10px #999;box-shadow:3px 3px 10px #999;font-family:Verdana, sans-serif; \"><style> .mmsgLetterContent p { margin:20px 0; padding:0; } 	.mmsgLetterContent 	{background: url(https://raw.githubusercontent.com/hey-hoho/ScheduleMasterCore/master/src/ScheduleMasterCore/Hos.ScheduleMaster.Web/wwwroot/images/logo-black_xs.png) no-repeat top right; }</style><div class=\"mmsgLetterContent\" style=\"text-align:left;padding:30px;font-size:14px;line-height:1.5;\"><p>你好!</p><p>感谢你使用ScheduleMaster平台。 <br />你参与的任务<strong>$NAME$</strong>在[$TIME$] 运行发生异常，请及时查看处理。</p><p><span style = \"border-radius: 2px;background: linear-gradient(to right,#57b5e3,#c4e6f6) ;padding: 4px 6px 4px 6px;display: inline-block;line-height: 1;color: #fff;text-align: center;white-space: nowrap;vertical-align: baseline;\" > 错误信息 </span ><br /> <strong> $MESSAGE$ </strong></p><p><span style= \"border-radius: 2px ;background: linear-gradient(to right,#d73d32,#f7b5b0) ;padding: 4px 6px 4px 6px;display: inline-block;line-height: 1;color: #fff;text-align: center;white-space: nowrap;vertical-align: baseline;\" > 程序堆栈 </span><br /><span style= \"font-family: Consolas,'Courier New',Courier,FreeMono,monospace;\" >$STACKREACE$</span></p></div></div></div> ";
            return EmailTemplate.Replace("$NAME$", sname).Replace("$TIME$", DateTime.Now.ToString()).Replace("$MESSAGE$", ex.Message).Replace("$STACKREACE$", ex.StackTrace);
        }

        #endregion
    }

    /// <summary>
    /// 任务运行状态监听器
    /// </summary>
    internal class JobRunListener : IJobListener
    {
        public string Name { get; set; }
        private Action<Guid, DateTime?> _callBack;

        public JobRunListener()
        {
        }

        public JobRunListener(string name, Action<Guid, DateTime?> callback)
        {
            _callBack = callback;
            Name = name;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken)
        {
            IJobDetail job = context.JobDetail;

            if (jobException == null)
            {
                var utcDate = context.Trigger.GetNextFireTimeUtc();
                DateTime? nextTime = utcDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value.DateTime, TimeZoneInfo.Local) : new DateTime?();
                _callBack(Guid.Parse(job.Key.Name), nextTime);

                //子任务触发
                Task.Run(async () =>
                 {
                     var children = job.JobDataMap["children"] as Dictionary<Guid, string>;
                     foreach (var item in children)
                     {
                         var jobkey = new JobKey(item.Key.ToString());
                         if (await context.Scheduler.CheckExists(jobkey))
                         {
                             JobDataMap map = new JobDataMap{
                                 new KeyValuePair<string, object>("PreviousResult", context.Result)
                             };
                             //map["PreviousResult"] = context.Result;
                             //var jDetail = await context.Scheduler.GetJobDetail(jobkey);
                             //JobDataMap data = jDetail.JobDataMap;
                             //data["PreviousResult"] = context.Result;
                             //jDetail.GetJobBuilder().UsingJobData(map);
                             //jDetail.JobDataMap.Add("PreviousResult", context.Result);
                             await context.Scheduler.TriggerJob(jobkey, map);
                         }
                     }
                 });
            }
            else if (jobException is BusinessRunException)
            {
                Task.Run(() =>
                 {
                     var name = job.JobDataMap["name"] as string;
                     var users = job.JobDataMap["keepers"] as List<KeyValuePair<string, string>>;
                     if (users != null && users.Any())
                     {
                         MailKitHelper.SendMail(users, $"任务运行异常 — {name}", QuartzManager.GetErrorEmailContent(name, (jobException as BusinessRunException).Detail));
                     }
                 });
            }
            return Task.FromResult(0);
        }
    }

    public class BusinessRunException : JobExecutionException
    {
        public Exception Detail;
        public BusinessRunException(Exception exp)
        {
            Detail = exp;
        }
    }
}