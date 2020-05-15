using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Hos.ScheduleMaster.Core.Services
{
    [ServiceMapTo(typeof(INodeService))]
    public class NodeService : BaseService, INodeService
    {

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
        public bool NodeSwich(string nodeName, int status)
        {
            var node = GetNodeByName(nodeName);
            if (node == null || node.NodeType == "master") return false;
            string router;
            switch (status)
            {
                case 1: { if (node.Status != 0) return false; router = "connect"; } break;
                case 2: { if (node.Status != 2) return false; router = "shutdown"; } break;
                case 3: { if (node.Status != 1) return false; router = "startup"; } break;
                default: return false;
            }
            string url = $"{node.AccessProtocol}://{node.Host}/api/server/{router}";
            if (status == 1)
            {
                Dictionary<string, string> header = new Dictionary<string, string> {
                    { "sm_connection", SecurityHelper.MD5(ConfigurationCache.NodeSetting.IdentityName) },
                    { "sm_nameto", nodeName }
                };
                var result = HttpRequest.Send(url, "post", null, header);
                bool success = result.Key == System.Net.HttpStatusCode.OK;
                if (success)
                {
                    _repositoryFactory.ServerNodes.UpdateBy(x => x.NodeName == nodeName, x => new ServerNodeEntity
                    {
                        AccessSecret = result.Value,
                        LastUpdateTime = DateTime.Now,
                        Status = 1//连接成功更新为空闲状态
                    });
                    _unitOfWork.Commit();
                }
                else
                {
                    Log.LogHelper.Warn($"{nodeName}连接异常[{result.Key.GetHashCode()}]：{result.Value}");
                }
                return success;
            }
            else
            {
                Dictionary<string, string> header = new Dictionary<string, string> { { "sm_secret", node.AccessSecret } };
                var result = HttpRequest.Send(url, "post", null, header);
                return result.Key == System.Net.HttpStatusCode.OK;
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
        /// 遍历所有worker并执行操作
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="router"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        public bool WorkersTraverseAction(Guid sid, string router, string verb = "post")
        {
            var nodeList = GetAvaliableWorkerForSchedule(sid);
            if (nodeList.Any())
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                if (sid != Guid.Empty)
                {
                    //param.Add("sid", sid.ToString());
                }
                var result = nodeList.AsParallel().Select(n =>
                {
                    return WorkerRequest(n, router, verb, param);
                }).ToArray();
                return result.All(x => x == true);
            }
            throw new InvalidOperationException("running worker not found.");
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
        /// 向worker发送请求
        /// </summary>
        /// <param name="node"></param>
        /// <param name="router"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool WorkerRequest(ServerNodeEntity node, string router, string method, Dictionary<string, string> param)
        {
            if (node == null)
            {
                Log.LogHelper.Warn($"没有可以发送请求的目标节点。");
                return false;
            }
            Dictionary<string, string> header = new Dictionary<string, string> { { "sm_secret", node.AccessSecret } };
            string url = $"{node.AccessProtocol}://{node.Host}/{router}";
            var result = HttpRequest.Send(url, method, param, header);
            var success = result.Key == HttpStatusCode.OK;
            if (!success)
            {
                Log.LogHelper.Warn($"响应码：{result.Key.GetHashCode()}，请求地址：{url}，响应消息：{result.Value}");
            }
            return success;
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
            workers.ForEach((w) =>
            {
                //初始化计数器
                ConfigurationCache.WorkerUnHealthCounter.TryAdd(w.NodeName, 0);
                //获取已失败次数
                int failedCount = ConfigurationCache.WorkerUnHealthCounter[w.NodeName];
                var success = WorkerRequest(w, "health", "get", null);
                if (!success)
                {
                    System.Threading.Interlocked.Increment(ref failedCount);
                }
                if (failedCount >= allowMaxFailed)
                {
                    w.Status = 0;//标记下线，实际上可能存在因为网络抖动等原因导致检查失败但worker进程还在运行的情况
                    w.LastUpdateTime = DateTime.Now;
                    _repositoryFactory.ServerNodes.Update(w);
                    //释放该节点占据的锁
                    _repositoryFactory.ScheduleLocks.UpdateBy(
                        x => x.LockedNode == w.NodeName && x.Status == 1
                        , x => new ScheduleLockEntity
                        {
                            Status = 0,
                            LockedNode = null,
                            LockedTime = null
                        });
                    _unitOfWork.Commit();
                    //重置计数器
                    ConfigurationCache.WorkerUnHealthCounter[w.NodeName] = 0;
                }
                else
                {
                    ConfigurationCache.WorkerUnHealthCounter[w.NodeName] = failedCount;
                }
            });
        }
    }
}
