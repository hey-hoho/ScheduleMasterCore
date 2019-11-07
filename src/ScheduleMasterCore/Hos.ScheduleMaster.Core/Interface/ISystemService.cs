using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <returns></returns>
        bool SaveConfig(Dictionary<string, string> items);
    }
}
