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
            return _repositoryFactory.SystemLogs.WherePager(pager, m => true, m => m.CreateTime, false);
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
            IQueryable<SystemLogEntity> query = _repositoryFactory.SystemLogs.Table;
            if (sid.HasValue)
            {
                query = query.Where(x => x.ScheduleId == sid.Value);
            }
            if (category.HasValue)
            {
                query = query.Where(x => x.Category == category.Value);
            }
            if (startdate.HasValue)
            {
                query = query.Where(x => x.CreateTime >= startdate.Value);
            }
            if (enddate.HasValue)
            {
                query = query.Where(x => x.CreateTime < enddate.Value);
            }
            _repositoryFactory.SystemLogs.DeleteBy(query);
            return _unitOfWork.Commit();
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
            string url = $"{node.AccessProtocol}://{node.Host}/api/quartz/{router}";
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
                    Log.LogHelper.Warn($"worker连接异常[{result.Key.GetHashCode()}]：{result.Value}");
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
    }
}
