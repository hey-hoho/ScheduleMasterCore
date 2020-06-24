using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
        public bool SaveConfig(Dictionary<string, string> items, string updateUserName)
        {
            foreach (var item in items)
            {
                _repositoryFactory.SystemConfigs.UpdateBy(
                    x => x.Key == item.Key,
                    new string[] { "Value", "UpdateTime", "UpdateUserName" },
                    new object[] { items[item.Key], DateTime.Now, updateUserName });
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
            _repositoryFactory.SystemLogs.WherePager(pager, m => true, m => m.CreateTime, false, false);
            pager.Total = 600;
            return pager;
        }

        /// <summary>
        /// 根据条件删除日志
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="category"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public async Task<int> DeleteLog(Guid? sid, int? category, DateTime? startdate, DateTime? enddate)
        {
            IQueryable<SystemLogEntity> query = _repositoryFactory.SystemLogs.Table;
            StringBuilder sqlBulder = new StringBuilder("delete from systemlogs where 1=1 ");
            if (sid.HasValue)
            {
                query = query.Where(x => x.ScheduleId == sid.Value);
                sqlBulder.Append($"and scheduleid='{sid.Value}' ");
            }
            if (category.HasValue)
            {
                query = query.Where(x => x.Category == category.Value);
                sqlBulder.Append($"and category={category.Value} ");
            }
            if (startdate.HasValue)
            {
                query = query.Where(x => x.CreateTime >= startdate.Value);
                sqlBulder.Append($"and createtime>='{startdate.Value.ToString("yyyy-MM-dd")}' ");
            }
            if (enddate.HasValue)
            {
                query = query.Where(x => x.CreateTime < enddate.Value);
                sqlBulder.Append($"and createtime<'{enddate.Value.ToString("yyyy-MM-dd")}' ");
            }
            int rows = await query.CountAsync();
            _ = Task.Run(() =>
            {
                using (var scope = new Core.ScopeDbContext())
                {
                    scope.GetDbContext().Database.ExecuteSqlRaw(sqlBulder.ToString());
                }
            });
            return rows;
        }

    }
}
