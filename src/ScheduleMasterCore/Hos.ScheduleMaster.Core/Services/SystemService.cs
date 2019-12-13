using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Hos.ScheduleMaster.Core.Services
{
    [ServiceMapTo(typeof(ISystemService))]
    public class SystemService : BaseService, ISystemService
    {
        /// <summary>
        /// 查询所有配置项
        /// </summary>
        /// <returns></returns>
        public List<SystemConfigEntity> GetConfigList()
        {
            return _repositoryFactory.SystemConfigs.Table.ToList();
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool SaveConfig(Dictionary<string, string> items)
        {
            foreach (var item in items)
            {
                _repositoryFactory.SystemConfigs.UpdateBy(x => x.Key == item.Key, new string[] { "Value", "UpdateTime" }, new object[] { items[item.Key], DateTime.Now });
            }
            if (items.Any())
            {
                return _unitOfWork.Commit() > 0;
            }
            return true;
        }

        /// <summary>
        /// 查询日志分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<SystemLogEntity> QueryLogPager(ListPager<SystemLogEntity> pager)
        {
            return _repositoryFactory.SystemLogs.WherePager(pager, m => true, m => m.Id, false);
        }

        /// <summary>
        /// 根据条件删除日志
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="category"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public int DeleteLog(Guid? sid, int? category, DateTime? startdate, DateTime? enddate)
        {
            Expression<Func<SystemLogEntity, bool>> where = m => true;
            if (sid.HasValue)
            {
                where = where.And(x => x.ScheduleId == sid.Value);
            }
            if (category.HasValue)
            {
                where = where.And(x => x.Category == category.Value);
            }
            if (startdate.HasValue)
            {
                where = where.And(x => x.CreateTime >= startdate.Value);
            }
            if (enddate.HasValue)
            {
                where = where.And(x => x.CreateTime < enddate.Value);
            }
            _repositoryFactory.SystemLogs.DeleteBy(where);
            return _unitOfWork.Commit();
        }
    }
}
