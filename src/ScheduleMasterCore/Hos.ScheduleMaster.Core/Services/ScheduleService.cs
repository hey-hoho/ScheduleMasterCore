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

        public ApiResponseMessage EditTask(TaskInfo model)
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

        public ApiResponseMessage TaskStart(ScheduleEntity task)
        {
            if (task == null) return ServiceResult(ResultStatus.Failed, "任务信息不能为空！");
            ScheduleView view = new ScheduleView() { Schedule = task };
            var users = _repositoryFactory.SystemUsers.Table;
            var guardians = _repositoryFactory.ScheduleKeepers.Table;
            view.Keepers = (from t in guardians
                            join u in users on t.UserId equals u.Id
                            where t.ScheduleId == task.Id && !string.IsNullOrEmpty(u.Email)
                            select new KeyValuePair<string, string>(u.RealName, u.Email)
                    ).ToList();
            var children = _repositoryFactory.ScheduleKeepers.Table;
            view.Children = (from c in children
                             join t in _repositoryFactory.Schedules.Table on c.ScheduleId equals t.Id
                             select new { t.Id, t.Title }
                             ).ToDictionary(x => x.Id, x => x.Title);
            //启动任务
            bool success = false;
            //QuartzManager.StartWithRetry(view, (nextRunTime) =>
            //{
            //    //每次运行成功后更新任务的运行情况
            //    var t = QueryById(task.Id);
            //    _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
            //    {
            //        LastRunTime = DateTime.Now,
            //        NextRunTime = nextRunTime,
            //        TotalRunCount = t.TotalRunCount + 1
            //    });
            //    _unitOfWork.Commit();
            //    LogHelper.Info($"任务[{task.Title}]运行成功！", task.Id);
            //});
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
                bool success = false;//QuartzManager.Pause(task);
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
                bool success = false;//QuartzManager.Resume(task);
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
                bool success = false;//QuartzManager.RunOnce(id);
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
                bool success = false;
                //QuartzManager.Stop(task, () =>
                //{
                //    _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
                //    {
                //        Status = (int)TaskStatus.Stop,
                //        NextRunTime = null
                //    });
                //    _unitOfWork.Commit();
                //});
                if (success)
                {
                    return ServiceResult(ResultStatus.Success, "任务已停止运行!");
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
                TimeSpan = timeSpan
            });
            return _unitOfWork.Commit() > 0;
        }
    }
}
