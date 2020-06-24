using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using Hos.ScheduleMaster.Core.Services.RemoteCaller;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Services
{
    [ServiceMapTo(typeof(INodeService))]
    public class NodeService : BaseService, INodeService
    {
        public ServerClient _serverClient;

        public NodeService(ServerClient serverClient)
        {
            _serverClient = serverClient;
        }

        /// <summary>
        /// 查询节点分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<ServerNodeEntity> QueryNodePager(ListPager<ServerNodeEntity> pager)
        {
            return _repositoryFactory.ServerNodes.WherePager(pager, m => true, m => m.NodeType, false);
        }

        /// <summary>
        /// 通过节点名称查询节点信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ServerNodeEntity GetNodeByName(string name)
        {
            return _repositoryFactory.ServerNodes.FirstOrDefault(x => x.NodeName == name);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddNode(ServerNodeEntity entity)
        {
            entity.Status = 0;
            _repositoryFactory.ServerNodes.Add(entity);
            return _unitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 修改节点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool EditNode(ServerNodeEntity entity)
        {
            entity.LastUpdateTime = DateTime.Now;
            _repositoryFactory.ServerNodes.UpdateBy(x => x.NodeName == entity.NodeName, new
            {
                entity.AccessProtocol,
                entity.Host,
                entity.LastUpdateTime,
                entity.MachineName,
                //entity.NodeType,
                entity.Priority
            });
            return _unitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteNode(string name)
        {
            var entity = GetNodeByName(name);
            if (entity == null || entity.Status == 2)
            {
                return false;
            }
            _repositoryFactory.ServerNodes.DeleteBy(x => x.NodeName == name);
            _repositoryFactory.ScheduleExecutors.DeleteBy(x => x.WorkerName == name);
            return _unitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 查询指定worker状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int QueryWorkerCount(int? status)
        {
            var query = _repositoryFactory.ServerNodes.Where(x => x.NodeType == "worker");
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            return query.Count();
        }

        /// <summary>
        /// 查询所有worker列表
        /// </summary>
        /// <returns></returns>
        public List<ServerNodeEntity> QueryWorkerList()
        {
            var query = _repositoryFactory.ServerNodes.WhereNoTracking(x => x.NodeType == "worker").OrderByDescending(x => x.LastUpdateTime);
            return query.ToList();
        }

        /// <summary>
        /// 节点操作
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="status">1-连接 2-空闲 3-运行</param>
        /// <returns></returns>
        public async Task<bool> NodeSwich(string nodeName, int status)
        {
            var node = GetNodeByName(nodeName);
            if (node == null || node.NodeType == "master") return false;

            _serverClient.Server = node;
            switch (status)
            {
                case 1:
                    {
                        if (node.Status != 0) return false;
                        var result = await _serverClient.Connect();
                        if (result.success)
                        {
                            _repositoryFactory.ServerNodes.UpdateBy(x => x.NodeName == nodeName, x => new ServerNodeEntity
                            {
                                AccessSecret = result.content,
                                LastUpdateTime = DateTime.Now,
                                Status = 1//连接成功更新为空闲状态
                            });
                            await _unitOfWork.CommitAsync();
                        }
                        else
                        {
                            Log.LogHelper.Warn($"{nodeName}连接异常：{result.content}");
                        }
                        return result.success;
                    }
                case 2:
                    {
                        if (node.Status != 2) return false;
                        return await _serverClient.Shutdown();
                    }
                case 3:
                    {
                        if (node.Status != 1) return false;
                        return await _serverClient.StartUp();
                    }
                default: return false;
            }
        }

        /// <summary>
        /// 查询指定任务正在运行状态的worker列表
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<ServerNodeEntity> GetAvaliableWorkerForSchedule(Guid sid)
        {
            var query = from n in _repositoryFactory.ServerNodes.Table
                        where n.NodeType == "worker" && n.Status == 2
                        && (from e in _repositoryFactory.ScheduleExecutors.Table where e.ScheduleId == sid && n.NodeName == e.WorkerName select 1).Any()
                        select n;
            return query.AsNoTracking().ToList();
        }

        /// <summary>
        /// 查询所有可用worker节点
        /// </summary>
        /// <returns></returns>
        public List<ServerNodeEntity> GetAllAvaliableWorker()
        {
            var query = from n in _repositoryFactory.ServerNodes.Table
                        where n.NodeType == "worker" && n.Status == 2
                        select n;
            return query.AsNoTracking().ToList();
        }

        /// <summary>
        /// 获取指定数量的可用worker（根据权重选择）
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<ServerNodeEntity> GetAvaliableWorkerByPriority(Guid? sid, int size = 1)
        {
            List<ServerNodeEntity> result = new List<ServerNodeEntity>();
            List<ServerNodeEntity> targetList = sid.HasValue ? GetAvaliableWorkerForSchedule(sid.Value) : GetAllAvaliableWorker();
            if (targetList.Any())
            {
                size = size > targetList.Count ? targetList.Count : size;
                while (result.Count < size)
                {
                    var worker = WorkerSelect(targetList);
                    if (result.Contains(worker))
                    {
                        continue;
                    }
                    result.Add(worker);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据权重比例选择一个worker
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ServerNodeEntity WorkerSelect(List<ServerNodeEntity> list)
        {
            list = list.OrderBy(x => x.Priority).ToList();
            //根据节点权重来选择一个节点运行
            int[] arry = new int[list.Count + 1];
            arry[0] = 0;
            for (int i = 0; i < list.Count; i++)
            {
                arry[i + 1] = list[i].Priority + arry[i];
            }
            var sum = list.Sum(x => x.Priority);
            int rnd = new Random().Next(0, sum);
            ServerNodeEntity selectedNode = null;
            for (int i = 1; i < arry.Length; i++)
            {
                if (rnd >= arry[i - 1] && rnd < arry[i])
                {
                    selectedNode = list[i - 1];
                    break;
                }
            }
            return selectedNode;
        }


        /// <summary>
        /// worker健康检查
        /// </summary>
        public void WorkerHealthCheck()
        {
            var workers = _repositoryFactory.ServerNodes.Where(x => x.NodeType == "worker" && x.Status != 0).ToList();
            if (!workers.Any())
            {
                return;
            }
            //允许最大失败次数
            int allowMaxFailed = ConfigurationCache.GetField<int>("System_WorkerUnHealthTimes");
            if (allowMaxFailed <= 0) allowMaxFailed = 3;
            //遍历处理
            workers.ForEach(async (w) =>
            {
                using (var scope = new Core.ScopeDbContext())
                {
                    var db = scope.GetDbContext();
                    _serverClient.Server = w;
                    //初始化计数器
                    ConfigurationCache.WorkerUnHealthCounter.TryAdd(w.NodeName, 0);
                    var success = await _serverClient.HealthCheck();
                    if (success)
                    {
                        w.LastUpdateTime = DateTime.Now;
                        db.ServerNodes.Update(w);
                        await db.SaveChangesAsync();
                        ConfigurationCache.WorkerUnHealthCounter[w.NodeName] = 0;
                    }
                    else
                    {
                        //获取已失败次数
                        int failedCount = ConfigurationCache.WorkerUnHealthCounter[w.NodeName];
                        System.Threading.Interlocked.Increment(ref failedCount);
                        if (failedCount >= allowMaxFailed)
                        {
                            w.Status = 0;//标记下线，实际上可能存在因为网络抖动等原因导致检查失败但worker进程还在运行的情况
                            db.ServerNodes.Update(w);
                            //释放该节点占据的锁
                            var locks = db.ScheduleLocks.Where(x => x.LockedNode == w.NodeName && x.Status == 1).ToList();
                            locks.ForEach(x =>
                            {
                                x.Status = 0;
                                x.LockedNode = null;
                                x.LockedTime = null;
                            });
                            db.ScheduleLocks.UpdateRange(locks);
                            await db.SaveChangesAsync();
                            //重置计数器
                            ConfigurationCache.WorkerUnHealthCounter[w.NodeName] = 0;
                        }
                        else
                        {
                            ConfigurationCache.WorkerUnHealthCounter[w.NodeName] = failedCount;
                        }
                    }
                }
            });
        }
    }
}
