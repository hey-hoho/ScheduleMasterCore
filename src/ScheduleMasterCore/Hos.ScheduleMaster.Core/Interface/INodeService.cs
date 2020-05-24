using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Interface
{
    public interface INodeService
    {

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
        /// 查询指定worker状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        int QueryWorkerCount(int? status);

        /// <summary>
        /// 查询所有worker列表
        /// </summary>
        /// <returns></returns>
        List<ServerNodeEntity> QueryWorkerList();

        /// <summary>
        /// 节点操作
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="status">1-连接 2-空闲 3-运行</param>
        /// <returns></returns>
        Task<bool> NodeSwich(string nodeName, int status);

        /// <summary>
        /// 查询指定任务正在运行状态的worker列表
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<ServerNodeEntity> GetAvaliableWorkerForSchedule(Guid sid);

        /// <summary>
        /// 查询所有可用worker节点
        /// </summary>
        /// <returns></returns>
        List<ServerNodeEntity> GetAllAvaliableWorker();

        /// <summary>
        /// 获取指定数量的可用worker（根据权重选择）
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        List<ServerNodeEntity> GetAvaliableWorkerByPriority(Guid? sid, int size = 1);

        /// <summary>
        /// worker健康检查
        /// </summary>
        void WorkerHealthCheck();
    }
}
