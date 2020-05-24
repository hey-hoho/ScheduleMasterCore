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
    [ServiceMapTo(typeof(IDelayedTaskService))]
    public class DelayedTaskService : BaseService, IDelayedTaskService
    {
        [Autowired]
        public INodeService _nodeService { get; set; }

        [Autowired]
        public WorkerDispatcher _workerDispatcher { get; set; }

        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ListPager<ScheduleDelayedInfo> QueryPager(ListPager<ScheduleDelayedInfo> pager, string workerName = "")
        {
            bool hasWorkerName = !string.IsNullOrEmpty(workerName);
            var schedule = _repositoryFactory.ScheduleDelayeds.Table;
            var executor = _repositoryFactory.ScheduleExecutors.Table;
            var query = (from s in schedule
                         where hasWorkerName ? (from e in executor where e.WorkerName == workerName && e.ScheduleId == s.Id select 1).Any() : true
                         select new ScheduleDelayedInfo
                         {
                             Id = s.Id,
                             ContentKey = s.ContentKey,
                             DelayAbsoluteTime = s.DelayAbsoluteTime,
                             DelayTimeSpan = s.DelayTimeSpan,
                             ExecuteTime = s.ExecuteTime,
                             FailedRetrys = s.FailedRetrys,
                             SourceApp = s.SourceApp,
                             Topic = s.Topic,
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
        /// id查询任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ScheduleDelayedInfo QueryById(Guid sid)
        {
            var entity = _repositoryFactory.ScheduleDelayeds.FirstOrDefault(m => m.Id == sid);
            ScheduleDelayedInfo info = ObjectMapper<ScheduleDelayedEntity, ScheduleDelayedInfo>.Convert(entity);
            if (info != null)
            {
                info.Executors = _repositoryFactory.ScheduleExecutors.Where(x => x.ScheduleId == sid).Select(x => x.WorkerName).ToList();
            }
            return info;
        }

        /// <summary>
        /// 创建一个延时任务
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="executors"></param>
        /// <returns></returns>
        public async Task<ServiceResponseMessage> Add(ScheduleDelayedEntity entity, List<string> executors)
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
            entity.CreateTime = DateTime.Now;

            //保存主信息
            _repositoryFactory.ScheduleDelayeds.Add(entity);
            //创建并保存任务锁
            _repositoryFactory.ScheduleLocks.Add(new ScheduleLockEntity { ScheduleId = entity.Id, Status = 0 });

            //保存执行节点
            _repositoryFactory.ScheduleExecutors.AddRange(executors.Select(x => new ScheduleExecutorEntity
            {
                ScheduleId = entity.Id,
                WorkerName = x
            }));

            //事务提交
            if (await _unitOfWork.CommitAsync() > 0)
            {
                //任务持久化成功后分发给worker，进入就绪状态
                return await Start(entity.Id);
            }
            return ServiceResult(ResultStatus.Failed, "数据保存失败!");
        }

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<ServiceResponseMessage> Start(Guid sid)
        {
            //启动任务
            bool success = await _workerDispatcher.DelayedTaskStart(sid);
            if (success)
            {
                //启动成功后更新任务状态为就绪
                _repositoryFactory.ScheduleDelayeds.UpdateBy(m => m.Id == sid, m => new ScheduleDelayedEntity
                {
                    Status = (int)ScheduleDelayStatus.Ready//就绪状态
                });
                if (await _unitOfWork.CommitAsync() > 0)
                {
                    return ServiceResult(ResultStatus.Success, "任务启动成功!", sid);
                }
                return ServiceResult(ResultStatus.Failed, "更新任务状态失败!", sid);
            }
            else
            {
                await _workerDispatcher.DelayedTaskRemove(sid);
                return ServiceResult(ResultStatus.Failed, "任务启动失败!", sid);
            }
        }

        /// <summary>
        /// 立即执行延时任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<ServiceResponseMessage> Execute(Guid sid)
        {
            bool success = await _workerDispatcher.DelayedTaskExecute(sid);
            if (success)
            {
                return ServiceResult(ResultStatus.Success, "任务运行成功!");
            }
            else
            {
                return ServiceResult(ResultStatus.Failed, "任务运行失败!");
            }
        }

        /// <summary>
        /// 作废任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<ServiceResponseMessage> Stop(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status < (int)ScheduleDelayStatus.Successed)
            {
                bool success = _nodeService.GetAvaliableWorkerForSchedule(sid).Any();
                if (success)
                {
                    success = await _workerDispatcher.DelayedTaskRemove(sid);
                }
                else
                {
                    success = true;
                }
                if (success)
                {
                    //更新任务状态为已作废
                    _repositoryFactory.ScheduleDelayeds.UpdateBy(m => m.Id == task.Id, m => new ScheduleDelayedEntity
                    {
                        Status = (int)ScheduleDelayStatus.Deleted
                    });
                    if (await _unitOfWork.CommitAsync() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务已作废!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                return ServiceResult(ResultStatus.Failed, "任务作废失败!");
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能作废!");
        }
    }
}
