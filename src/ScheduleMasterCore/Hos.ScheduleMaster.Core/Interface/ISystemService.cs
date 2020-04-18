using Hos.ScheduleMaster.Core.Common;
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
        int DeleteLog(Guid? sid, int? category, DateTime? startdate, DateTime? enddate);

        /// <summary>
        /// 查询节点分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        ListPager<ServerNodeEntity> QueryNodePager(ListPager<ServerNodeEntity> pager);

        /// <summary>
        /// 通过节点名称查询节点信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ServerNodeEntity GetNodeByName(string name);

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool AddNode(ServerNodeEntity entity);

        /// <summary>
        /// 修改节点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool EditNode(ServerNodeEntity entity);

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DeleteNode(string name);

        /// <summary>
        /// 节点操作
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="status">1-连接 2-空闲 3-运行</param>
        /// <returns></returns>
        bool NodeSwich(string nodeName, int status);
    }
}
