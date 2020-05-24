using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Interface
{
    public interface IDelayedTaskService
    {
        /// <summary>
        /// 查询任务列表
        /// </summary>
        /// <param name="pager"></param>
        ///  <param name="workerName"></param>
        /// <returns></returns>
        ListPager<ScheduleDelayedInfo> QueryPager(ListPager<ScheduleDelayedInfo> pager, string workerName = "");

        /// <summary>
        /// id查询任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        ScheduleDelayedInfo QueryById(Guid sid);

        /// <summary>
        /// 创建一个延时任务
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="executors"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Add(ScheduleDelayedEntity entity,List<string> executors);

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Start(Guid sid);

        /// <summary>
        /// 立即执行延时任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Execute(Guid sid);

        /// <summary>
        /// 作废任务
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<ServiceResponseMessage> Stop(Guid sid);
    }
}
