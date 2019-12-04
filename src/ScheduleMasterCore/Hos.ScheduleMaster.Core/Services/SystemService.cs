using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
