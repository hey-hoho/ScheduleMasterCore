using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Services.RemoteCaller;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Services
{
    [ServiceMapTo(typeof(IScheduleService))]
    public class ScheduleService : BaseService, IScheduleService
    {
        [Autowired]
        public INodeService _nodeService { get; set; }

        [Autowired]
        public WorkerDispatcher _workerDispatcher { get; set; }

        /// <summary>
        /// 查询所有未删除的任务
        /// </summary>
        /// <returns></returns>
        public List<ScheduleEntity> QueryAll()
        {
            return _repositoryFactory.Schedules.WhereNoTracking(m => m.Status != (int)ScheduleStatus.Deleted).ToList();
        }

        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ListPager<ScheduleInfo> QueryPager(ListPager<ScheduleInfo> pager, int? userId, string workerName = "")
        {
            bool basUserId = userId.HasValue && userId.Value > 0;
            bool hasWorkerName = !string.IsNullOrEmpty(workerName);
            var schedule = _repositoryFactory.Schedules.Table;
            var keeper = _repositoryFactory.ScheduleKeepers.Table;
            var executor = _repositoryFactory.ScheduleExecutors.Table;
            var query = (from s in schedule
                         where basUserId ? (from k in keeper where k.UserId == userId && k.ScheduleId == s.Id select 1).Any() : true
                         && hasWorkerName ? (from e in executor where e.WorkerName == workerName && e.ScheduleId == s.Id select 1).Any() : true
                         //orderby s.CreateTime descending
                         select new ScheduleInfo
                         {
                             Id = s.Id,
                             Title = s.Title,
                             RunLoop = s.RunLoop,
                             StartDate = s.StartDate,
                             LastRunTime = s.LastRunTime,
                             NextRunTime = s.NextRunTime,
                             TotalRunCount = s.TotalRunCount,
                             Status = s.Status,
                             CreateTime = s.CreateTime,
                             Executors = (from e in executor where e.ScheduleId == s.Id select e.WorkerName).ToList()
                         });
            pager.AddFilter(x => x.Status != (int)ScheduleStatus.Deleted);

            foreach (var filter in pager.Filters)
            {
                query = query.Where(filter);
            }
            var orderList = query.OrderByDescending(x => x.CreateTime);
            pager.Rows = orderList.Skip(pager.SkipCount).Take(pager.PageSize).AsNoTracking().ToList();
            pager.Total = orderList.Count();
            return pager;
        }

        /// <summary>
        /// 查看指定用户的监护任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        private ListPager<ScheduleEntity> QueryUserSchedule(int userId, ListPager<ScheduleEntity> pager)
        {
            var keeper = _repositoryFactory.ScheduleKeepers.Table;
            var schedule = _repositoryFactory.Schedules.Table;
            var query = (from s in schedule
                         join k in keeper on s.Id equals k.ScheduleId
                         where k.UserId == userId
                         orderby s.CreateTime descending
                         select s);
            return query.WherePager(pager, x => x.Status != (int)ScheduleStatus.Deleted, x => x.CreateTime, false);
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
        public ScheduleContext QueryScheduleContext(Guid sid)
        {
            ScheduleContext context = new ScheduleContext()
            {
                Schedule = QueryById(sid),
                HttpOption = QueryScheduleHttpOptions(sid)
            };
            if (context.Schedule != null)
            {
                context.Keepers = (from t in _repositoryFactory.ScheduleKeepers.Table
                                   join u in _repositoryFactory.SystemUsers.Table on t.UserId equals u.Id
                                   where t.ScheduleId == sid
                                   select new KeyValuePair<string, string>(u.UserName, u.RealName)
                        ).ToList();
                context.Children = (from c in _repositoryFactory.ScheduleReferences.Table
                                    join t in _repositoryFactory.Schedules.Table on c.ChildId equals t.Id
                                    where c.ScheduleId == sid && c.ChildId != sid
                                    select new { t.Id, t.Title }
                                ).ToDictionary(x => x.Id, x => x.Title);
                context.Executors = _repositoryFactory.ScheduleExecutors.Where(x => x.ScheduleId == sid).Select(x => x.WorkerName).ToList();
            }
            return context;
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
        /// 查询指定运行状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int QueryTraceCount(int? status)
        {
            var query = _repositoryFactory.TraceStatisticss.TableNoTracking;
            if (!status.HasValue)
            {
                return query.Sum(x => x.Success + x.Fail + x.Other);
            }
            else if (status.Value == 1)
            {
                return query.Sum(x => x.Success);
            }
            else if (status.Value == 2)
            {
                return query.Sum(x => x.Fail);
            }
            return 0;
        }

        /// <summary>
        /// 查询运行情况周报表
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<long, int>> QueryTraceWeeklyReport(int? status)
        {
            Dictionary<long, int> result = new Dictionary<long, int>();

            long start = DateTime.Today.AddDays(-9).ToTimeStamp();
            var query = _repositoryFactory.TraceStatisticss.WhereNoTracking(x => x.DateStamp >= start);
            if (!status.HasValue)
            {
                result = query.ToDictionary(x => x.DateStamp, x => x.Success + x.Fail + x.Other);
            }
            else if (status.Value == 1)
            {
                result = query.ToDictionary(x => x.DateStamp, x => x.Success);
            }
            else if (status.Value == 2)
            {
                result = query.ToDictionary(x => x.DateStamp, x => x.Fail);
            }
            for (int i = 9; i >= 0; i--)
            {
                long day = DateTime.Today.AddDays(-i).ToTimeStamp();
                int cnt = 0;
                result.TryGetValue(day, out cnt);
                yield return new KeyValuePair<long, int>(day, cnt);
            }
        }

        /// <summary>
        /// 查询任务的http配置
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ScheduleHttpOptionEntity QueryScheduleHttpOptions(Guid sid)
        {
            return _repositoryFactory.ScheduleHttpOptions.FirstOrDefault(x => x.ScheduleId == sid);
        }

        /// <summary>
        /// 查询任务的监护人
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<ScheduleKeeperEntity> QueryScheduleKeepers(Guid sid)
        {
            return _repositoryFactory.ScheduleKeepers.WhereNoTracking(x => x.ScheduleId == sid).ToList();
        }

        /// <summary>
        /// 查询任务指派的运行节点
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<ScheduleExecutorEntity> QueryScheduleExecutors(Guid sid)
        {
            return _repositoryFactory.ScheduleExecutors.WhereNoTracking(x => x.ScheduleId == sid).ToList();
        }

        /// <summary>
        /// 查询任务的子级任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<ScheduleReferenceEntity> QueryScheduleReferences(Guid sid)
        {
            return _repositoryFactory.ScheduleReferences.WhereNoTracking(x => x.ScheduleId == sid).ToList();
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpOption"></param>
        /// <param name="keepers"></param>
        /// <param name="nexts"></param>
        /// <param name="executors"></param>
        /// <returns></returns>
        public ServiceResponseMessage Add(ScheduleEntity model, ScheduleHttpOptionEntity httpOption, List<int> keepers, List<Guid> nexts, List<string> executors = null)
        {
            if (executors == null || !executors.Any())
            {
                //没有指定worker就根据权重选择2个
                executors = _nodeService.GetAvaliableWorkerByPriority(null, 2).Select(x => x.NodeName).ToList();
            }
            if (!executors.Any())
            {
                return ServiceResult(ResultStatus.Failed, "没有可用节点!");
            }
            model.CreateTime = DateTime.Now;
            var user = _repositoryFactory.SystemUsers.FirstOrDefault(x => x.UserName == model.CreateUserName);
            if (user != null)
            {
                model.CreateUserId = user.Id;
            }
            //保存主信息
            _repositoryFactory.Schedules.Add(model);
            //创建并保存任务锁
            _repositoryFactory.ScheduleLocks.Add(new ScheduleLockEntity { ScheduleId = model.Id, Status = 0 });
            //保存http数据
            if (httpOption != null)
            {
                httpOption.ScheduleId = model.Id;
                _repositoryFactory.ScheduleHttpOptions.Add(httpOption);
            }
            //保存运行节点
            _repositoryFactory.ScheduleExecutors.AddRange(executors.Select(x => new ScheduleExecutorEntity
            {
                ScheduleId = model.Id,
                WorkerName = x
            }));
            //保存监护人
            if (keepers != null && keepers.Count > 0)
            {
                _repositoryFactory.ScheduleKeepers.AddRange(keepers.Select(x => new ScheduleKeeperEntity
                {
                    ScheduleId = model.Id,
                    UserId = x
                }));
            }
            //保存子任务
            if (nexts != null && nexts.Count > 0)
            {
                _repositoryFactory.ScheduleReferences.AddRange(nexts.Select(x => new ScheduleReferenceEntity
                {
                    ScheduleId = model.Id,
                    ChildId = x
                }));
            }
            //事务提交
            if (_unitOfWork.Commit() > 0)
            {
                return ServiceResult(ResultStatus.Success, "任务创建成功!", model.Id);
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
                MetaType = model.MetaType,
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
            if (model.MetaType == (int)ScheduleMetaType.Http)
            {
                _repositoryFactory.ScheduleHttpOptions.DeleteBy(x => x.ScheduleId == model.Id);
                _repositoryFactory.ScheduleHttpOptions.Add(new ScheduleHttpOptionEntity
                {
                    ScheduleId = model.Id,
                    Body = model.HttpBody,
                    ContentType = model.HttpContentType,
                    Headers = model.HttpHeaders,
                    Method = model.HttpMethod,
                    RequestUrl = model.HttpRequestUrl
                });
            }
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
            _repositoryFactory.ScheduleExecutors.DeleteBy(x => x.ScheduleId == model.Id);
            _repositoryFactory.ScheduleExecutors.AddRange(model.Executors.Select(x => new ScheduleExecutorEntity
            {
                ScheduleId = model.Id,
                WorkerName = x
            }));
            if (_unitOfWork.Commit() > 0)
            {
                return ServiceResult(ResultStatus.Success, "任务编辑成功!");
            }
            return ServiceResult(ResultStatus.Failed, "任务编辑失败!");
        }

        /// <summary>
        /// 恢复运行中的任务
        /// </summary>
        public void RunningRecovery()
        {
            _repositoryFactory.Schedules.WhereNoTracking(x => x.Status == (int)ScheduleStatus.Running).ToList()
                .ForEach(async x =>
                {
                    try { await InnerStart(x.Id); } catch { }
                });
        }

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponseMessage> Start(ScheduleEntity model)
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
            return await InnerStart(model.Id);
        }
        private async Task<ServiceResponseMessage> InnerStart(Guid sid)
        {
            //启动任务
            bool success = await _workerDispatcher.ScheduleStart(sid);
            if (success)
            {
                //启动成功后更新任务状态为运行中
                _repositoryFactory.Schedules.UpdateBy(m => m.Id == sid, m => new ScheduleEntity
                {
                    Status = (int)ScheduleStatus.Running
                });
                if (await _unitOfWork.CommitAsync() > 0)
                {
                    return ServiceResult(ResultStatus.Success, "任务启动成功!");
                }
                return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
            }
            else
            {
                await _workerDispatcher.ScheduleStop(sid);
                _repositoryFactory.Schedules.UpdateBy(m => m.Id == sid, m => new ScheduleEntity
                {
                    Status = (int)ScheduleStatus.Stop,
                    NextRunTime = null
                });
                await _unitOfWork.CommitAsync();
                return ServiceResult(ResultStatus.Failed, "任务启动失败!");
            }
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<ServiceResponseMessage> Pause(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Running)
            {
                bool success = await _workerDispatcher.SchedulePause(sid);
                if (success)
                {
                    //暂停成功后更新任务状态为已暂停
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Paused,
                        NextRunTime = null
                    });
                    if (await _unitOfWork.CommitAsync() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务暂停成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                else
                {
                    await _workerDispatcher.ScheduleResume(sid);
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
        public async Task<ServiceResponseMessage> Resume(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Paused)
            {
                bool success = await _workerDispatcher.ScheduleResume(sid);
                if (success)
                {
                    //恢复运行后更新任务状态为运行中
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Running
                    });
                    if (await _unitOfWork.CommitAsync() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务恢复成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                else
                {
                    await _workerDispatcher.SchedulePause(sid);
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
        public async Task<ServiceResponseMessage> RunOnce(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Running)
            {
                bool success = await _workerDispatcher.ScheduleRunOnce(sid);
                if (success)
                {
                    return ServiceResult(ResultStatus.Success, "任务运行成功!");
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
        public async Task<ServiceResponseMessage> Stop(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status > (int)ScheduleStatus.Stop)
            {
                bool success = _nodeService.GetAvaliableWorkerForSchedule(sid).Any();
                if (success)
                {
                    success = await _workerDispatcher.ScheduleStop(sid);
                }
                else
                {
                    success = true;
                }
                if (success)
                {
                    //更新任务状态为已停止
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Stop,
                        NextRunTime = null
                    });
                    if (await _unitOfWork.CommitAsync() > 0)
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
                    //删除关联数据
                    _repositoryFactory.ScheduleHttpOptions.DeleteBy(x => x.ScheduleId == sid);
                    _repositoryFactory.ScheduleExecutors.DeleteBy(x => x.ScheduleId == sid);
                    _repositoryFactory.ScheduleKeepers.DeleteBy(x => x.ScheduleId == sid);
                    _repositoryFactory.ScheduleLocks.DeleteBy(x => x.ScheduleId == sid);
                    _repositoryFactory.ScheduleReferences.DeleteBy(x => x.ScheduleId == sid || x.ChildId == sid);
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
        /// 查询运行记录分页信息
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<ScheduleTraceEntity> QueryTracePager(ListPager<ScheduleTraceEntity> pager)
        {
            _repositoryFactory.ScheduleTraces.WherePager(pager, m => m.TraceId != null, m => m.StartTime, false, false);
            pager.Total = 150;
            return pager;
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
