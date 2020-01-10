using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace Hos.ScheduleMaster.Core.Services
{
    [ServiceMapTo(typeof(IScheduleService))]
    public class ScheduleService : BaseService, IScheduleService
    {
        //private readonly System.Net.Http.IHttpClientFactory _clientFactory;

        /// <summary>
        /// 查询所有未删除的任务
        /// </summary>
        /// <returns></returns>
        public List<ScheduleEntity> QueryAll()
        {
            return _repositoryFactory.Schedules.Where(m => m.Status != (int)ScheduleStatus.Deleted).AsNoTracking().ToList();
        }

        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<ScheduleEntity> QueryPager(ListPager<ScheduleEntity> pager)
        {
            return _repositoryFactory.Schedules.WherePager(pager, m => m.Status != (int)ScheduleStatus.Deleted, m => m.CreateTime, false);
        }


        /// <summary>
        /// id查询任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ScheduleEntity QueryById(Guid sid)
        {
            return _repositoryFactory.Schedules.FirstOrDefault(m => m.Id == sid);
        }

        /// <summary>
        /// 查询任务详细信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ScheduleView QueryScheduleView(Guid sid)
        {
            ScheduleView view = new ScheduleView()
            {
                Schedule = QueryById(sid)
            };
            if (view.Schedule != null)
            {
                view.Keepers = (from t in _repositoryFactory.ScheduleKeepers.Table
                                join u in _repositoryFactory.SystemUsers.Table on t.UserId equals u.Id
                                where t.ScheduleId == sid
                                select new KeyValuePair<string, string>(u.UserName, u.RealName)
                        ).ToList();
                view.Children = (from c in _repositoryFactory.ScheduleReferences.Table
                                 join t in _repositoryFactory.Schedules.Table on c.ChildId equals t.Id
                                 where c.ScheduleId == sid && c.ChildId != sid
                                 select new { t.Id, t.Title }
                                ).ToDictionary(x => x.Id, x => x.Title);
            }
            return view;
        }

        /// <summary>
        /// 查看指定用户的监护任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="takeSize"></param>
        /// <returns></returns>
        public List<ScheduleEntity> QueryUserSchedule(int userId, int takeSize)
        {
            var keeper = _repositoryFactory.ScheduleKeepers.Table;
            var schedule = _repositoryFactory.Schedules.Table;
            var query = (from s in schedule
                         join k in keeper on s.Id equals k.ScheduleId
                         where k.UserId == userId
                         orderby s.CreateTime descending
                         select s).Take(takeSize);
            return query.AsNoTracking().ToList();
        }

        /// <summary>
        /// 查询指定状态的任务数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int QueryScheduleCount(int? status)
        {
            var query = _repositoryFactory.Schedules.Where(x => x.Status != (int)ScheduleStatus.Deleted);
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            return query.Count();
        }

        /// <summary>
        /// 查询指定worker状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int QueryWorkerCount(int? status)
        {
            var query = _repositoryFactory.ServerNodes.Where(x => x.NodeType == "worker");
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            return query.Count();
        }

        /// <summary>
        /// 查询指定运行状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int QueryTraceCount(int? status)
        {
            var query = _repositoryFactory.ScheduleTraces.Where(x => x.ScheduleId != null && x.ScheduleId != Guid.Empty);
            if (status.HasValue)
            {
                query = query.Where(x => x.Result == status.Value);
            }
            return query.Count();
        }

        /// <summary>
        /// 查询运行情况周报表
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<KeyValuePair<long, int>> QueryTraceWeeklyReport(int? status)
        {
            var query = _repositoryFactory.ScheduleTraces.Where(x => x.ScheduleId != null && x.ScheduleId != Guid.Empty && x.StartTime >= DateTime.Now.AddDays(-9));
            if (status.HasValue)
            {
                query = query.Where(x => x.Result == status.Value);
            }
            var list = query.GroupBy(x => x.StartTime.Date).Select(x => new KeyValuePair<long, int>(x.Key.ToTimeStamp(), x.Count())).ToList();
            List<KeyValuePair<long, int>> result = new List<KeyValuePair<long, int>>();
            for (int i = 9; i >= 0; i--)
            {
                long day = DateTime.Today.AddDays(-i).ToTimeStamp();
                int cnt = 0;
                var dl = list.Any(x => x.Key == day);
                if (list.Any(x => x.Key == day)) cnt = list.FirstOrDefault(x => x.Key == day).Value;
                result.Add(new KeyValuePair<long, int>(day, cnt));
            }
            return result;
        }

        /// <summary>
        /// 查询任务的监护人
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<ScheduleKeeperEntity> QueryScheduleKeepers(Guid sid)
        {
            return _repositoryFactory.ScheduleKeepers.Where(x => x.ScheduleId == sid).AsNoTracking().ToList();
        }

        /// <summary>
        /// 查询任务的子级任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<ScheduleReferenceEntity> QueryScheduleReferences(Guid sid)
        {
            return _repositoryFactory.ScheduleReferences.Where(x => x.ScheduleId == sid).AsNoTracking().ToList();
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keepers"></param>
        /// <param name="nexts"></param>
        /// <returns></returns>
        public ServiceResponseMessage Add(ScheduleEntity model, List<int> keepers, List<Guid> nexts)
        {
            model.CreateTime = DateTime.Now;
            var user = _repositoryFactory.SystemUsers.FirstOrDefault(x => x.UserName == model.CreateUserName);
            if (user != null)
            {
                model.CreateUserId = user.Id;
            }
            _repositoryFactory.Schedules.Add(model);
            _repositoryFactory.ScheduleLocks.Add(new ScheduleLockEntity { ScheduleId = model.Id, Status = 0 });

            if (_unitOfWork.Commit() > 0)
            {
                bool extended = false;
                if (keepers != null && keepers.Count > 0)
                {
                    extended = true;
                    _repositoryFactory.ScheduleKeepers.AddRange(keepers.Select(x => new ScheduleKeeperEntity
                    {
                        ScheduleId = model.Id,
                        UserId = x
                    }));
                }
                if (nexts != null && nexts.Count > 0)
                {
                    extended = true;
                    _repositoryFactory.ScheduleReferences.AddRange(nexts.Select(x => new ScheduleReferenceEntity
                    {
                        ScheduleId = model.Id,
                        ChildId = x
                    }));
                }
                if (extended) _unitOfWork.Commit();
                return ServiceResult(ResultStatus.Success, "任务创建成功!");
            }
            return ServiceResult(ResultStatus.Failed, "数据保存失败!");
        }

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ServiceResponseMessage Edit(ScheduleInfo model)
        {
            var task = _repositoryFactory.Schedules.FirstOrDefault(m => m.Id == model.Id);
            if (task == null)
            {
                return ServiceResult(ResultStatus.Failed, "任务不存在!");
            }
            if (task.Status != (int)ScheduleStatus.Stop)
            {
                return ServiceResult(ResultStatus.Failed, "在停止状态下才能编辑任务信息!");
            }
            _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
            {
                AssemblyName = model.AssemblyName,
                ClassName = model.ClassName,
                CronExpression = model.CronExpression,
                RunLoop = model.RunLoop,
                CustomParamsJson = model.CustomParamsJson,
                EndDate = model.EndDate,
                Remark = model.Remark,
                StartDate = model.StartDate,
                Title = model.Title
            });
            _repositoryFactory.ScheduleKeepers.DeleteBy(x => x.ScheduleId == model.Id);
            _repositoryFactory.ScheduleKeepers.AddRange(model.Keepers.Select(x => new ScheduleKeeperEntity
            {
                ScheduleId = model.Id,
                UserId = x
            }));
            _repositoryFactory.ScheduleReferences.DeleteBy(x => x.ScheduleId == model.Id);
            _repositoryFactory.ScheduleReferences.AddRange(model.Nexts.Select(x => new ScheduleReferenceEntity
            {
                ScheduleId = model.Id,
                ChildId = x
            }));
            if (_unitOfWork.Commit() > 0)
            {
                return ServiceResult(ResultStatus.Success, "任务编辑成功!");
            }
            return ServiceResult(ResultStatus.Failed, "任务编辑失败!");
        }

        /// <summary>
        /// 是否存在可用的work节点
        /// </summary>
        /// <returns></returns>
        private bool HasAvailableWorker()
        {
            return _repositoryFactory.ServerNodes.Any(x => x.NodeType == "worker" && x.Status == 2);
        }

        /// <summary>
        /// 遍历所有worker并执行操作
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="router"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        private bool WorkersTraverseAction(Guid sid, string router, string verb = "post")
        {
            var nodeList = _repositoryFactory.ServerNodes.Where(x => x.NodeType == "worker" && x.Status == 2).ToList();
            if (nodeList.Any())
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                if (sid != Guid.Empty)
                {
                    //param.Add("sid", sid.ToString());
                }
                var result = nodeList.AsParallel().Select(n =>
                  {
                      return NodeRequest(n, router, "post", param);
                  }).ToArray();
                return result.All(x => x == true);
            }
            return false;
        }

        /// <summary>
        /// 根据权重选择一个worker执行操作
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="router"></param>
        /// <returns></returns>
        private bool WorkerSelectOne(Guid sid, string router)
        {
            //根据节点权重来选择一个节点运行
            var list = _repositoryFactory.ServerNodes.Where(m => m.NodeType == "worker" && m.Status == 2).OrderBy(x => x.Priority).ToList();
            int[] arry = new int[list.Count + 1];
            arry[0] = 0;
            for (int i = 0; i < list.Count; i++)
            {
                arry[i + 1] = list[i].Priority + arry[i];
            }
            var sum = list.Sum(x => x.Priority);
            int rnd = new Random().Next(0, sum);
            ServerNodeEntity selectedNode = null;
            for (int i = 1; i < arry.Length; i++)
            {
                if (rnd >= arry[i - 1] && rnd < arry[i])
                {
                    selectedNode = list[i - 1];
                    break;
                }
            }
            if (selectedNode != null)
            {
                Dictionary<string, string> param = new Dictionary<string, string> { { "sid", sid.ToString() } };
                return NodeRequest(selectedNode, router, "post", param);
            }
            return false;
        }

        private bool NodeRequest(ServerNodeEntity node, string router, string method, Dictionary<string, string> param)
        {
            //var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get,
            //$"{node.AccessProtocol}://{node.Host}/{router}");
            //request.Headers.Add("sm_secret", node.AccessSecret);
            //if (param != null)
            //{
            //    request.Content = new System.Net.Http.FormUrlEncodedContent(param.AsEnumerable());
            //}
            //var client = new System.Net.Http.HttpClient();

            //var response = await client.SendAsync(request);
            //if (response.IsSuccessStatusCode) { }
            Dictionary<string, string> header = new Dictionary<string, string> { { "sm_secret", node.AccessSecret } };
            var result = HttpRequest.Send($"{node.AccessProtocol}://{node.Host}/{router}", method, param, header);
            return result.Key == HttpStatusCode.OK;
        }

        /// <summary>
        /// 恢复运行中的任务
        /// </summary>
        public void RunningRecovery()
        {
            _repositoryFactory.Schedules.Where(x => x.Status == (int)ScheduleStatus.Running).AsNoTracking().ToList().ForEach(x => InnerStart(x.Id));
        }

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ServiceResponseMessage Start(ScheduleEntity model)
        {
            if (model == null) return ServiceResult(ResultStatus.Failed, "任务信息不能为空！");
            if (model.Status != (int)ScheduleStatus.Stop)
            {
                return ServiceResult(ResultStatus.Failed, "任务在停止状态下才能启动！");
            }
            if (model.EndDate.HasValue && model.EndDate < DateTime.Now)
            {
                return ServiceResult(ResultStatus.Failed, "任务结束时间不能小于当前时间！");
            }
            return InnerStart(model.Id);
        }
        private ServiceResponseMessage InnerStart(Guid sid)
        {
            //启动任务
            bool success = WorkersTraverseAction(sid, "api/quartz/start?sid=" + sid);
            if (success)
            {
                //启动成功后更新任务状态为运行中
                _repositoryFactory.Schedules.UpdateBy(m => m.Id == sid, m => new ScheduleEntity
                {
                    Status = (int)ScheduleStatus.Running
                });
                if (_unitOfWork.Commit() > 0)
                {
                    return ServiceResult(ResultStatus.Success, "任务启动成功!");
                }
                return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
            }
            else
            {
                WorkersTraverseAction(sid, "api/quartz/stop?sid=" + sid);
                _repositoryFactory.Schedules.UpdateBy(m => m.Id == sid, m => new ScheduleEntity
                {
                    Status = (int)ScheduleStatus.Stop,
                    NextRunTime = null
                });
                _unitOfWork.Commit();
                return ServiceResult(ResultStatus.Failed, "任务启动失败!");
            }
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ServiceResponseMessage Pause(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Running)
            {
                bool success = WorkersTraverseAction(task.Id, "api/quartz/pause?sid=" + sid);
                if (success)
                {
                    //暂停成功后更新任务状态为已暂停
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Paused,
                        NextRunTime = null
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务暂停成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                else
                {
                    WorkersTraverseAction(sid, "api/quartz/resume?sid=" + sid);
                    return ServiceResult(ResultStatus.Failed, "任务暂停失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能暂停!");
        }

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ServiceResponseMessage Resume(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Paused)
            {
                bool success = WorkersTraverseAction(task.Id, "api/quartz/resume?sid=" + sid);
                if (success)
                {
                    //恢复运行后更新任务状态为运行中
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Running
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务恢复成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                else
                {
                    WorkersTraverseAction(sid, "api/quartz/pause?sid=" + sid);
                    return ServiceResult(ResultStatus.Failed, "任务恢复失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能恢复运行!");
        }

        /// <summary>
        /// 执行一次任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ServiceResponseMessage RunOnce(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Running)
            {
                bool success = WorkerSelectOne(sid, "api/quartz/runonce?sid=" + sid);
                if (success)
                {
                    //运行成功后更新信息
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        LastRunTime = DateTime.Now,
                        TotalRunCount = task.TotalRunCount + 1
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务运行成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务运行时间失败!");
                }
                else
                {
                    return ServiceResult(ResultStatus.Failed, "任务运行失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "任务不在运行状态下!");
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ServiceResponseMessage Stop(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status > (int)ScheduleStatus.Stop)
            {
                bool success = !HasAvailableWorker();
                if (!success) success = WorkersTraverseAction(task.Id, "api/quartz/stop?sid=" + sid);
                if (success)
                {
                    //更新任务状态为已停止
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Stop,
                        NextRunTime = null
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务已停止运行!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                return ServiceResult(ResultStatus.Failed, "任务停止失败!");
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能停止!");
        }

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ServiceResponseMessage Delete(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status != (int)ScheduleStatus.Deleted)
            {
                if (task.Status == (int)ScheduleStatus.Stop)
                {
                    //停止状态下的才能删除
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Deleted,
                        NextRunTime = null
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务已删除!");
                    }
                    return ServiceResult(ResultStatus.Failed, "任务删除失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能删除!");
        }

        /// <summary>
        /// worker健康检查
        /// </summary>
        public void WorkerHealthCheck()
        {
            var workers = _repositoryFactory.ServerNodes.Where(x => x.NodeType == "worker" && x.Status != 0).ToList();
            if (!workers.Any())
            {
                return;
            }
            workers.ForEach((w) =>
            {
                var success = NodeRequest(w, "api/quartz/healthcheck", "get", null);
                if (!success)
                {
                    w.Status = 0;
                }
                w.LastUpdateTime = DateTime.Now;
                _repositoryFactory.ServerNodes.Update(w);
            });
            _unitOfWork.Commit();
        }

        /// <summary>
        /// 查询运行记录分页信息
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<ScheduleTraceEntity> QueryTracePager(ListPager<ScheduleTraceEntity> pager)
        {
            return _repositoryFactory.ScheduleTraces.WherePager(pager, m => m.TraceId != null, m => m.StartTime, false);
        }

        /// <summary>
        /// 查询运行记录日志
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<SystemLogEntity> QueryTraceDetail(ListPager<SystemLogEntity> pager)
        {
            return _repositoryFactory.SystemLogs.WherePager(pager, m => m.Id > 0, m => m.CreateTime, false);
        }

        /// <summary>
        /// 添加一条运行记录
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public Guid AddRunTrace(Guid sid)
        {
            ScheduleTraceEntity entity = new ScheduleTraceEntity();
            entity.TraceId = Guid.NewGuid();
            entity.ScheduleId = sid;
            entity.StartTime = DateTime.Now;
            entity.Result = (int)ScheduleRunResult.Null;
            _repositoryFactory.ScheduleTraces.Add(entity);
            if (_unitOfWork.Commit() > 0)
            {
                return entity.TraceId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 更新运行记录
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="timeSpan"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool UpdateRunTrace(Guid traceId, double timeSpan, ScheduleRunResult result)
        {
            if (traceId == Guid.Empty)
            {
                return false;
            }
            _repositoryFactory.ScheduleTraces.UpdateBy(x => x.TraceId == traceId, x => new ScheduleTraceEntity
            {
                EndTime = DateTime.Now,
                Result = (int)result,
                ElapsedTime = timeSpan
            });
            return _unitOfWork.Commit() > 0;
        }
    }
}
