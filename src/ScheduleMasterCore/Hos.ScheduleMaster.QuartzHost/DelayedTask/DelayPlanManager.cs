using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Log;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.DelayedTask
{
    public class DelayPlanManager
    {
        private static DelayQueue<NotifyPlan> _queue;

        /// <summary>
        /// 队列实例是已启用
        /// </summary>
        public static bool IsEnabled => _queue != null;

        /// <summary>
        /// 队列初始化
        /// </summary>
        public static void Init()
        {
            _queue = new DelayQueue<NotifyPlan>(60);
            //把归属本节点的就绪任务恢复
            ReadyRecovery();
        }

        /// <summary>
        /// 队列清空
        /// </summary>
        public static void Clear()
        {
            if (_queue != null)
            {
                _queue.Clear();
                _queue = null;
            }
        }

        /// <summary>
        /// 根据任务id插入
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static async Task<bool> InsertById(Guid sid)
        {
            bool success = false;
            using (var scope = new Core.ScopeDbContext())
            {
                var entity = await scope.GetDbContext().ScheduleDelayeds.FirstOrDefaultAsync(x => x.Id == sid && x.Status != (int)ScheduleDelayStatus.Ready);
                if (entity != null)
                {
                    return InsertByEntity(entity);
                }
            }
            return success;
        }

        /// <summary>
        /// 根据任务实体插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static bool InsertByEntity(ScheduleDelayedEntity entity)
        {
            bool success = false;
            NotifyPlan plan = new NotifyPlan()
            {
                Key = entity.Id.ToString(),
                NotifyUrl = entity.NotifyUrl,
                NotifyDataType = entity.NotifyDataType,
                NotifyBody = entity.NotifyBody,
                Callback = NotifyExecutedEvent
            };
            string pattern = ConfigurationCache.GetField<string>("DelayTask_DelayPattern");
            if (pattern.ToLower() == "absolute")
            {
                success = Insert(plan, entity.DelayAbsoluteTime);
            }
            else
            {
                success = Insert(plan, entity.DelayTimeSpan);
            }
            return success;
        }

        /// <summary>
        /// 任务执行事件回调
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static async Task NotifyExecutedEvent(DelayQueueSlot slot)
        {
            if (slot is NotifyPlan plan)
            {
                using (var scope = new ScopeDbContext())
                {
                    var db = scope.GetDbContext();
                    string node = ConfigurationCache.NodeSetting.IdentityName;
                    bool getLocked = await db.Database.ExecuteSqlRawAsync($"update schedulelocks set status=1,lockedtime=now(),lockednode='{node}' where scheduleid='{slot.Key}' and status=0") > 0;
                    if (getLocked)
                    {
                        Guid traceId = Guid.NewGuid();
                        try
                        {
                            var httpClient = scope.GetService<IHttpClientFactory>().CreateClient();
                            //创建一条trace
                            await db.Database.ExecuteSqlRawAsync($"insert into scheduletraces values('{traceId.ToString()}','{slot.Key}','{node}',now(),'0001-01-01',0,0)");
                            //执行回调
                            await NotifyRequest(httpClient, db, plan, traceId);
                        }
                        finally
                        {
                            System.Threading.Thread.Sleep(1000);
                            await db.Database.ExecuteSqlRawAsync($"update schedulelocks set status=0,lockedtime=null,lockednode=null where scheduleid='{slot.Key}'");
                        }
                    }
                    else
                    {
                        //竞争失败就移除
                        Remove(plan.Key);
                    }
                }
            }
        }

        private static async Task NotifyRequest(HttpClient httpClient, SmDbContext db, NotifyPlan plan, Guid traceId)
        {
            Guid sid = Guid.Parse(plan.Key);
            var entity = await db.ScheduleDelayeds.FirstOrDefaultAsync(x => x.Id == sid);
            if (entity == null) return;
            entity.ExecuteTime = DateTime.Now;

            string updateTraceSql = "update scheduletraces set result={0},elapsedtime={1},endtime=now() where traceid='{2}'";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Restart();

                HttpContent reqContent = new StringContent(plan.NotifyBody, System.Text.Encoding.UTF8, "application/json");
                if (plan.NotifyDataType == "application/x-www-form-urlencoded")
                {
                    //任务创建是要确保参数是键值对的json格式
                    reqContent = new FormUrlEncodedContent(Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(plan.NotifyBody));
                }

                LogHelper.Info($"即将请求：{entity.NotifyUrl}", sid, traceId);
                var response = await httpClient.PostAsync(plan.NotifyUrl, reqContent);
                var content = await response.Content.ReadAsStringAsync();
                LogHelper.Info($"请求结束，响应码：{response.StatusCode.GetHashCode().ToString()}，响应内容：{(response.Content.Headers.GetValues("Content-Type").Any(x => x.Contains("text/html")) ? "html文档" : content)}", sid, traceId);

                if (response.IsSuccessStatusCode && content.Contains("success"))
                {
                    stopwatch.Stop();
                    string elapsed = Math.Round(stopwatch.Elapsed.TotalSeconds, 3).ToString();
                    db.Database.ExecuteSqlRaw(string.Format(updateTraceSql, (int)ScheduleRunResult.Success, elapsed, traceId));
                    //更新结果字段
                    entity.FinishTime = DateTime.Now;
                    entity.Status = (int)ScheduleDelayStatus.Successed;
                    //更新日志
                    LogHelper.Info($"延时任务[{entity.Topic}:{entity.ContentKey}]执行成功。", sid, traceId);
                }
                else
                {
                    throw new HttpRequestException("异常的返回结果。");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                //更新日志
                db.Database.ExecuteSqlRaw(string.Format(updateTraceSql, (int)ScheduleRunResult.Failed, Math.Round(stopwatch.Elapsed.TotalSeconds, 3).ToString(), traceId));
                //失败重试策略
                int maxRetry = ConfigurationCache.GetField<int>("DelayTask_RetryTimes");
                if (entity.FailedRetrys < (maxRetry > 0 ? maxRetry : 3))
                {
                    //更新结果字段
                    entity.FailedRetrys++;
                    //计算下次延时间隔
                    int timespan = ConfigurationCache.GetField<int>("DelayTask_RetrySpans");
                    int delay = (timespan > 0 ? timespan : 10) * entity.FailedRetrys;
                    //重新进入延时队列
                    Insert(plan, delay);
                    //更新日志
                    LogHelper.Error($"延时任务[{entity.Topic}:{entity.ContentKey}]执行失败，将在{delay.ToString()}秒后开始第{entity.FailedRetrys.ToString()}次重试。", ex, sid, traceId);
                }
                else
                {
                    entity.Status = (int)ScheduleDelayStatus.Failed;
                    entity.Remark = $"重试{entity.FailedRetrys}次后失败结束";
                    //更新日志
                    LogHelper.Error($"延时任务[{entity.Topic}:{entity.ContentKey}]重试{entity.FailedRetrys}次后失败结束。", ex, sid, traceId);
                    //邮件通知
                    var user = await db.SystemUsers.FirstOrDefaultAsync(x => x.UserName == entity.CreateUserName && !string.IsNullOrEmpty(x.Email));
                    if (user != null)
                    {
                        var keeper = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(user.RealName, user.Email) };
                        MailKitHelper.SendMail(keeper, $"延时任务异常 — {entity.Topic}:{entity.ContentKey}",
                                Common.QuartzManager.GetErrorEmailContent($"{entity.Topic}:{entity.ContentKey}", ex)
                            );
                    }
                }
                // .....
                // 其实这个重试策略稍微有点问题，只能在抢锁成功的节点上进行重试，如果遭遇单点故障会导致任务丢失
                // 严格来说应该通知到master让其对所有节点执行重试策略，但考虑到master也会有单点问题，综合考虑后还是放到当前worker中重试，若worker节点异常可以在控制台中人工干预进行重置或立即执行
            }
            db.Update(entity);
            await db.SaveChangesAsync();
        }


        /// <summary>
        /// 节点恢复就绪任务
        /// </summary>
        private static void ReadyRecovery()
        {
            using (var scope = new Core.ScopeDbContext())
            {
                var db = scope.GetDbContext();
                List<ScheduleDelayedEntity> list = (from t in db.ScheduleDelayeds
                                                    join e in db.ScheduleExecutors on t.Id equals e.ScheduleId
                                                    where e.WorkerName == ConfigurationCache.NodeSetting.IdentityName && t.Status == (int)ScheduleDelayStatus.Ready
                                                    select t).ToList();
                list.AsParallel().ForAll(t =>
                {
                    InsertByEntity(t);
                });
            }
        }

        /// <summary>
        /// 按相对时间插入任务
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="delaySeconds"></param>
        /// <returns></returns>
        public static bool Insert(NotifyPlan plan, int delaySeconds)
        {
            return _queue.Insert(plan, delaySeconds);
        }

        /// <summary>
        /// 按绝对时间插入任务
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool Insert(NotifyPlan plan, DateTime time)
        {
            //System.Diagnostics.Debug.WriteLine($"插入时间{DateTime.Now}");
            var seconds = Math.Round((time - DateTime.Now).TotalSeconds, 0, MidpointRounding.AwayFromZero);
            if (seconds <= 0) return false;
            //System.Diagnostics.Debug.WriteLine($"时间差{seconds}");
            return _queue.Insert(plan, (int)seconds);
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            return _queue.Remove(key);
        }

        /// <summary>
        /// 从队列消费一个槽点数据
        /// </summary>
        public static void Read()
        {
            _queue.Read();
        }
    }

    public class NotifyPlan : DelayQueueSlot
    {
        public string NotifyUrl { get; set; }

        public string NotifyDataType { get; set; }

        public string NotifyBody { get; set; }
    }
}
