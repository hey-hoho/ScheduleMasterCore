using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Interface
{
    public interface ISystemService
    {
        /// <summary>
        /// 查询所有的配置项
        /// </summary>
        /// <returns></returns>
        List<SystemConfigEntity> GetConfigList();

        /// <summary>
        /// 保存配置项
        /// </summary>
        /// <param name="items"></param>
        /// <param name="updateUserName"></param>
        /// <returns></returns>
        bool SaveConfig(Dictionary<string, string> items, string updateUserName);

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
        Task<int> DeleteLog(Guid? sid, int? category, DateTime? startdate, DateTime? enddate);

    }
}
