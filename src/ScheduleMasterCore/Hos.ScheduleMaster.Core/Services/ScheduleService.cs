using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Hos.ScheduleMaster.Core.Services
{
    [ServiceMapTo(typeof(IScheduleService))]
    public class ScheduleService : BaseService, IScheduleService
    {

        public List<ScheduleEntity> QueryAll()
        {
            return _repositoryFactory.Schedules.Where(m => m.Status != (int)ScheduleStatus.Deleted).ToList();
        }

        public ListPager<ScheduleEntity> QueryPager(ListPager<ScheduleEntity> pager)
        {
            return _repositoryFactory.Schedules.WherePager(pager, m => m.Status != (int)ScheduleStatus.Deleted, m => m.CreateTime, false);
        }

        public ListPager<SystemLogEntity> QueryLogPager(ListPager<SystemLogEntity> pager)
        {
            return _repositoryFactory.SystemLogs.WherePager(pager, m => true, m => m.Id, false);
        }

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

        public ScheduleEntity QueryById(Guid sid)
        {
            return _repositoryFactory.Schedules.FirstOrDefault(m => m.Id == sid);
        }

        public List<ScheduleKeeperEntity> QueryTaskGuardians(Guid sid)
        {
            return _repositoryFactory.ScheduleKeepers.Where(x => x.ScheduleId == sid).ToList();
        }

        public List<ScheduleReferenceEntity> QueryTaskReferences(Guid sid)
        {
            return _repositoryFactory.ScheduleReferences.Where(x => x.ScheduleId == sid).ToList();
        }

        public ApiResponseMessage AddTask(ScheduleEntity model, List<int> keepers, List<Guid> nexts)
        {
            model.CreateTime = DateTime.Now;
            _repositoryFactory.Schedules.Add(model);

            if (_unitOfWork.Commit() > 0)
            {
                bool extended = false;
                if (keepers != null && keepers.Count > 0)
                {
                    extended = true;
                    _repositoryFactory.ScheduleKeepers.AddRange(keepers.Select(x => new ScheduleKeeperEntity
                    {
                        ScheduleId = model.Id,
                        UserId = x
                    }));
                }
                if (nexts != null && nexts.Count > 0)
                {
                    extended = true;
                    _repositoryFactory.ScheduleReferences.AddRange(nexts.Select(x => new ScheduleReferenceEntity
                    {
                        ScheduleId = model.Id,
                        ChildId = x
                    }));
                }
                if (extended) _unitOfWork.Commit();
                //创建专属目录
                //string path = $"{AppDomain.CurrentDomain.BaseDirectory}/TaskAssembly/{model.AssemblyName}";
                //if (!System.IO.Directory.Exists(path))
                //{
                //    System.IO.Directory.CreateDirectory(path);
                //}
                return ServiceResult(ResultStatus.Success, "任务创建成功!");
            }
            return ServiceResult(ResultStatus.Failed, "数据保存失败!");
        }

        public ApiResponseMessage EditTask(ScheduleInfo model)
        {
            var task = _repositoryFactory.Schedules.FirstOrDefault(m => m.Id == model.Id);
            if (task == null)
            {
                return ServiceResult(ResultStatus.Failed, "任务不存在!");
            }
            if (task.Status != (int)ScheduleStatus.Stop)
            {
                return ServiceResult(ResultStatus.Failed, "在停止状态下才能编辑任务信息!");
            }
            _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
            {
                AssemblyName = model.AssemblyName,
                ClassName = model.ClassName,
                CronExpression = model.CronExpression,
                RunMoreTimes = model.RunMoreTimes,
                CustomParamsJson = model.CustomParamsJson,
                EndDate = model.EndDate,
                Remark = model.Remark,
                StartDate = model.StartDate,
                Title = model.Title
            });
            _repositoryFactory.ScheduleKeepers.DeleteBy(x => x.ScheduleId == model.Id);
            _repositoryFactory.ScheduleKeepers.AddRange(model.Guardians.Select(x => new ScheduleKeeperEntity
            {
                ScheduleId = model.Id,
                UserId = x
            }));
            _repositoryFactory.ScheduleReferences.DeleteBy(x => x.ScheduleId == model.Id);
            _repositoryFactory.ScheduleReferences.AddRange(model.Nexts.Select(x => new ScheduleReferenceEntity
            {
                ScheduleId = model.Id,
                ChildId = x
            }));
            if (_unitOfWork.Commit() > 0)
            {
                return ServiceResult(ResultStatus.Success, "任务编辑成功!");
            }
            return ServiceResult(ResultStatus.Failed, "任务编辑失败!");
        }

        private bool NodesTraverseAction(Guid sid, string router)
        {
            var nodeList = _repositoryFactory.ServerNodes.Where(x => x.Status == 1).ToList();
            if (nodeList.Any())
            {
                Dictionary<string, string> param = new Dictionary<string, string> { { "sid", sid.ToString() } };
                bool success = false;
                System.Threading.Tasks.Parallel.ForEach(nodeList, (n) =>
                {
                    Dictionary<string, string> header = new Dictionary<string, string> { { "sm_secret", n.AccessSecret } };
                    var result = HttpRequest.Send($"{n.AccessProtocol}://{n.Host}/{router}", "post", param, header);
                    success = success || result.Key == System.Net.HttpStatusCode.OK;
                });
                return success;
            }
            return false;
        }
        private bool NodesSelectOne(Guid sid, string router)
        {
            //根据节点权重来选择一个节点运行
            var list = _repositoryFactory.ServerNodes.Where(m => m.Status == 1).OrderBy(x => x.Priority).ToList();
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
            if (selectedNode != null)
            {
                Dictionary<string, string> param = new Dictionary<string, string> { { "sid", sid.ToString() } };
                Dictionary<string, string> header = new Dictionary<string, string> { { "sm_secret", selectedNode.AccessSecret } };
                var result = HttpRequest.Send($"{selectedNode.AccessProtocol}://{selectedNode.Host}/{router}", "post", param, header);
                return result.Key == System.Net.HttpStatusCode.OK;
            }
            return false;
        }

        public ApiResponseMessage TaskStart(ScheduleEntity task)
        {
            if (task == null) return ServiceResult(ResultStatus.Failed, "任务信息不能为空！");
            //启动任务
            bool success = NodesTraverseAction(task.Id, "api/quartz/start");
            if (success)
            {
                //启动成功后更新任务状态为运行中
                _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                {
                    Status = (int)ScheduleStatus.Running
                });
                if (_unitOfWork.Commit() > 0)
                {
                    return ServiceResult(ResultStatus.Success, "任务启动成功!");
                }
                return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
            }
            else
            {
                _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                {
                    Status = (int)ScheduleStatus.Stop,
                    NextRunTime = null
                });
                _unitOfWork.Commit();
                return ServiceResult(ResultStatus.Failed, "任务启动失败!");
            }
        }

        public ApiResponseMessage PauseTask(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Running)
            {
                bool success = NodesTraverseAction(task.Id, "api/quartz/pause");
                if (success)
                {
                    //暂停成功后更新任务状态为已暂停
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Paused,
                        NextRunTime = null
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务暂停成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                else
                {
                    return ServiceResult(ResultStatus.Failed, "任务暂停失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能暂停!");
        }

        public ApiResponseMessage ResumeTask(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Paused)
            {
                bool success = NodesTraverseAction(task.Id, "api/quartz/resume");
                if (success)
                {
                    //恢复运行后更新任务状态为运行中
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Running
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务恢复成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                else
                {
                    return ServiceResult(ResultStatus.Failed, "任务恢复失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能恢复运行!");
        }

        public ApiResponseMessage RunOnceTask(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status == (int)ScheduleStatus.Running)
            {
                bool success = NodesSelectOne(sid, "api/quartz/runonce");
                if (success)
                {
                    //运行成功后更新信息
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        LastRunTime = DateTime.Now,
                        TotalRunCount = task.TotalRunCount + 1
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务运行成功!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务运行时间失败!");
                }
                else
                {
                    return ServiceResult(ResultStatus.Failed, "任务运行失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "任务不在运行状态下!");
        }
        public ApiResponseMessage StopTask(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status > (int)ScheduleStatus.Stop)
            {
                bool success = NodesTraverseAction(task.Id, "api/quartz/stop");
                if (success)
                {
                    //更新任务状态为已停止
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Stop,
                        NextRunTime = null
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务已停止运行!");
                    }
                    return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
                }
                return ServiceResult(ResultStatus.Failed, "任务停止失败!");
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能停止!");
        }

        public ApiResponseMessage DeleteTask(Guid sid)
        {
            var task = QueryById(sid);
            if (task != null && task.Status != (int)ScheduleStatus.Deleted)
            {
                if (task.Status == (int)ScheduleStatus.Stop)
                {
                    //停止状态下的才能删除
                    _repositoryFactory.Schedules.UpdateBy(m => m.Id == task.Id, m => new ScheduleEntity
                    {
                        Status = (int)ScheduleStatus.Deleted,
                        NextRunTime = null
                    });
                    if (_unitOfWork.Commit() > 0)
                    {
                        return ServiceResult(ResultStatus.Success, "任务已删除!");
                    }
                    return ServiceResult(ResultStatus.Failed, "任务删除失败!");
                }
            }
            return ServiceResult(ResultStatus.Failed, "当前任务状态下不能删除!");
        }

        /// <summary>
        /// 添加一条运行记录
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public Guid AddRunTrace(Guid sid)
        {
            ScheduleTraceEntity entity = new ScheduleTraceEntity();
            entity.TraceId = Guid.NewGuid();
            entity.ScheduleId = sid;
            entity.StartTime = DateTime.Now;
            entity.Result = (int)ScheduleRunResult.Null;
            _repositoryFactory.ScheduleTraces.Add(entity);
            if (_unitOfWork.Commit() > 0)
            {
                return entity.TraceId;
            }
            return Guid.Empty;
        }

        public bool UpdateRunTrace(Guid traceId, double timeSpan, ScheduleRunResult result)
        {
            if (traceId == Guid.Empty)
            {
                return false;
            }
            _repositoryFactory.ScheduleTraces.UpdateBy(x => x.TraceId == traceId, x => new ScheduleTraceEntity
            {
                EndTime = DateTime.Now,
                Result = (int)result,
                ElapsedTime = timeSpan
            });
            return _unitOfWork.Commit() > 0;
        }
    }
}
