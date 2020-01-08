using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <returns></returns>
        ListPager<ScheduleEntity> QueryPager(ListPager<ScheduleEntity> pager);

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
        ScheduleView QueryScheduleView(Guid sid);

        /// <summary>
        /// 查看指定用户的监护任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="takeSize"></param>
        /// <returns></returns>
        List<ScheduleEntity> QueryUserSchedule(int userId, int takeSize);

        /// <summary>
        /// 查询指定状态的任务数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryScheduleCount(int? status);

        /// <summary>
        /// 查询指定worker状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryWorkerCount(int? status);

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
        List<KeyValuePair<long, int>> QueryTraceWeeklyReport(int? status);

        /// <summary>
        /// 查询任务的监护人
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<ScheduleKeeperEntity> QueryScheduleKeepers(Guid sid);

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
        /// <param name="keepers"></param>
        /// <param name="nexts"></param>
        /// <returns></returns>
        ServiceResponseMessage Add(ScheduleEntity model, List<int> keepers, List<Guid> nexts);

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ServiceResponseMessage Edit(ScheduleInfo model);

        /// <summary>
        /// 恢复运行中的任务
        /// </summary>
        void RunningRecovery();

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ServiceResponseMessage Start(ScheduleEntity task);

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Pause(Guid sid);

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Resume(Guid sid);

        /// <summary>
        /// 执行一次任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage RunOnce(Guid sid);

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage Stop(Guid sid);

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
