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
        /// 查询日志分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        ListPager<SystemLogEntity> QueryLogPager(ListPager<SystemLogEntity> pager);

        /// <summary>
        /// 根据条件删除日志
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="category"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        int DeleteLog(Guid? sid, int? category, DateTime? startdate, DateTime? enddate);

        /// <summary>
        /// id查询任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ScheduleEntity QueryById(Guid sid);

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
        ServiceResponseMessage AddTask(ScheduleEntity model, List<int> guardians, List<Guid> nexts);

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ServiceResponseMessage EditTask(ScheduleInfo model);

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ServiceResponseMessage TaskStart(ScheduleEntity task);

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage PauseTask(Guid sid);

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage ResumeTask(Guid sid);

        /// <summary>
        /// 执行一次任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage RunOnceTask(Guid sid);

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage StopTask(Guid sid);

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ServiceResponseMessage DeleteTask(Guid sid);

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
