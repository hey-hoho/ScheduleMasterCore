using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Interface
{
    public interface IScheduleService
    {
        /// <summary>
        /// 查询所有未删除的任务
        /// </summary>
        /// <returns></returns>
        List<ScheduleEntity> QueryAll();

        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        ///  <param name="userId"></param>
        ///  <param name="workerName"></param>
        /// <returns></returns>
        ListPager<ScheduleInfo> QueryPager(ListPager<ScheduleInfo> pager, int? userId, string workerName = "");

        /// <summary>
        /// id查询任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ScheduleEntity QueryById(Guid sid);

        /// <summary>
        /// 查询任务详细信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ScheduleContext QueryScheduleContext(Guid sid);

        /// <summary>
        /// 查询指定状态的任务数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryScheduleCount(int? status);

        /// <summary>
        /// 查询指定运行状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryTraceCount(int? status);

        /// <summary>
        /// 查询运行情况周报表
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<long, int>> QueryTraceWeeklyReport(int? status);

        /// <summary>
        /// 查询任务的http配置
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ScheduleHttpOptionEntity QueryScheduleHttpOptions(Guid sid);

        /// <summary>
        /// 查询任务的监护人
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<ScheduleKeeperEntity> QueryScheduleKeepers(Guid sid);

        /// <summary>
        /// 查询任务指派的运行节点
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<ScheduleExecutorEntity> QueryScheduleExecutors(Guid sid);

        /// <summary>
        /// 查询任务的子级任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<ScheduleReferenceEntity> QueryScheduleReferences(Guid sid);

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpOption"></param>
        /// <param name="keepers"></param>
        /// <param name="nexts"></param>
        /// <param name="executors"></param>
        /// <returns></returns>
        ServiceResponseMessage Add(ScheduleEntity model, ScheduleHttpOptionEntity httpOption, List<int> keepers, List<Guid> nexts, List<string> executors = null);

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ServiceResponseMessage Edit(ScheduleInfo model);

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Start(ScheduleEntity task);

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Pause(Guid sid);

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Resume(Guid sid);

        /// <summary>
        /// 执行一次任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> RunOnce(Guid sid);

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Stop(Guid sid);

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Delete(Guid sid);

        /// <summary>
        /// 查询运行记录分页信息
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        ListPager<ScheduleTraceEntity> QueryTracePager(ListPager<ScheduleTraceEntity> pager);

        /// <summary>
        /// 查询运行记录日志
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        ListPager<SystemLogEntity> QueryTraceDetail(ListPager<SystemLogEntity> pager);

        /// <summary>
        /// 添加一条运行记录
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Guid AddRunTrace(Guid sid);

        /// <summary>
        /// 更新运行记录
        /// </summary>
        /// <param name="traceId"></param>
        /// <param name="timeSpan"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool UpdateRunTrace(Guid traceId, double timeSpan, ScheduleRunResult result);

    }
}
