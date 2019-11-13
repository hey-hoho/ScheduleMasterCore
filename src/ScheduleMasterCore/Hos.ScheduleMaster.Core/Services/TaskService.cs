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
    [ServiceMapTo(typeof(ITaskService))]
    public class TaskService : BaseService, ITaskService
    {
        //public TaskService(Repository.IUnitOfWork unitOfWork) : base(unitOfWork) { }
        public List<TaskEntity> QueryAll()
        {
            return _repositoryFactory.Tasks.Where(m => m.Status != (int)TaskStatus.Deleted).ToList();
        }

        public ListPager<TaskEntity> QueryPager(ListPager<TaskEntity> pager)
        {
            return _repositoryFactory.Tasks.WherePager(pager, m => m.Status != (int)TaskStatus.Deleted, m => m.CreateTime, false);
        }

        public ListPager<SystemLogEntity> QueryLogPager(ListPager<SystemLogEntity> pager)
        {
            return _repositoryFactory.SystemLogs.WherePager(pager, m => true, m => m.Id, false);
        }

        public int DeleteLog(int? task, int? category, DateTime? startdate, DateTime? enddate)
        {
            Expression<Func<SystemLogEntity, bool>> where = m => true;
            if (task.HasValue)
            {
                where = where.And(x => x.TaskId == task.Value);
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

        public TaskEntity QueryById(int id)
        {
            return _repositoryFactory.Tasks.FirstOrDefault(m => m.Id == id);
        }

        public List<TaskGuardianEntity> QueryTaskGuardians(int taskId)
        {
            return _repositoryFactory.TaskGuardians.Where(x => x.TaskId == taskId).ToList();
        }

        public List<TaskReferenceEntity> QueryTaskReferences(int taskId)
        {
            return _repositoryFactory.TaskReferences.Where(x => x.ParentTaskId == taskId).ToList();
        }

        public ApiResponseMessage AddTask(TaskEntity model, List<int> guardians, List<int> nexts)
        {
            model.CreateTime = DateTime.Now;
            _repositoryFactory.Tasks.Add(model);

            if (_unitOfWork.Commit() > 0)
            {
                bool extended = false;
                if (guardians != null && guardians.Count > 0)
                {
                    extended = true;
                    _repositoryFactory.TaskGuardians.AddRange(guardians.Select(x => new TaskGuardianEntity
                    {
                        TaskId = model.Id,
                        UserId = x
                    }));
                }
                if (nexts != null && nexts.Count > 0)
                {
                    extended = true;
                    _repositoryFactory.TaskReferences.AddRange(nexts.Select(x => new TaskReferenceEntity
                    {
                        ParentTaskId = model.Id,
                        ChildTaskId = x
                    }));
                }
                if (extended) _unitOfWork.Commit();
                //创建专属目录
                string path = $"{AppDomain.CurrentDomain.BaseDirectory}/TaskAssembly/{model.AssemblyName}";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return ServiceResult(ResultStatus.Success, "任务创建成功!");
            }
            return ServiceResult(ResultStatus.Failed, "数据保存失败!");
        }

        public ApiResponseMessage EditTask(TaskInfo model)
        {
            var task = _repositoryFactory.Tasks.FirstOrDefault(m => m.Id == model.Id);
            if (task == null)
            {
                return ServiceResult(ResultStatus.Failed, "任务不存在!");
            }
            if (task.Status != (int)TaskStatus.Stop)
            {
                return ServiceResult(ResultStatus.Failed, "在停止状态下才能编辑任务信息!");
            }
            _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
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
            _repositoryFactory.TaskGuardians.DeleteBy(x => x.TaskId == model.Id);
            _repositoryFactory.TaskGuardians.AddRange(model.Guardians.Select(x => new TaskGuardianEntity
            {
                TaskId = model.Id,
                UserId = x
            }));
            _repositoryFactory.TaskReferences.DeleteBy(x => x.ParentTaskId == model.Id);
            _repositoryFactory.TaskReferences.AddRange(model.Nexts.Select(x => new TaskReferenceEntity
            {
                ParentTaskId = model.Id,
                ChildTaskId = x
            }));
            if (_unitOfWork.Commit() > 0)
            {
                return ServiceResult(ResultStatus.Success, "任务编辑成功!");
            }
            return ServiceResult(ResultStatus.Failed, "任务编辑失败!");
        }

        public ApiResponseMessage TaskStart(TaskEntity task)
        {
            if (task == null) return ServiceResult(ResultStatus.Failed, "任务信息不能为空！");
            TaskView view = new TaskView() { Task = task };
            var users = _repositoryFactory.SystemUsers.Table;
            var guardians = _repositoryFactory.TaskGuardians.Table;
            view.Guardians = (from t in guardians
                              join u in users on t.UserId equals u.Id
                              where t.TaskId == task.Id && !string.IsNullOrEmpty(u.Email)
                              select new { u.Email, u.RealName }
                    ).ToDictionary(x => x.Email, x => x.RealName);
            var children = _repositoryFactory.TaskReferences.Table;
            view.ChildTasks = (from c in children
                               join t in _repositoryFactory.Tasks.Table on c.ParentTaskId equals t.Id
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
                _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
                {
                    Status = (int)TaskStatus.Running
                });
                if (_unitOfWork.Commit() > 0)
                {
                    return ServiceResult(ResultStatus.Success, "任务启动成功!");
                }
                return ServiceResult(ResultStatus.Failed, "更新任务状态失败!");
            }
            else
            {
                _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
                {
                    Status = (int)TaskStatus.Stop,
                    NextRunTime = null
                });
                _unitOfWork.Commit();
                return ServiceResult(ResultStatus.Failed, "任务启动失败!");
            }
        }

        public ApiResponseMessage PauseTask(int id)
        {
            var task = QueryById(id);
            if (task != null && task.Status == (int)TaskStatus.Running)
            {
                bool success = false;//QuartzManager.Pause(task);
                if (success)
                {
                    //暂停成功后更新任务状态为已暂停
                    _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
                    {
                        Status = (int)TaskStatus.Paused,
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

        public ApiResponseMessage ResumeTask(int id)
        {
            var task = QueryById(id);
            if (task != null && task.Status == (int)TaskStatus.Paused)
            {
                bool success = false;//QuartzManager.Resume(task);
                if (success)
                {
                    //恢复运行后更新任务状态为运行中
                    _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
                    {
                        Status = (int)TaskStatus.Running
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

        public ApiResponseMessage RunOnceTask(int id)
        {
            var task = QueryById(id);
            if (task != null && task.Status == (int)TaskStatus.Running)
            {
                bool success = false;//QuartzManager.RunOnce(id);
                if (success)
                {
                    //运行成功后更新信息
                    _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
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
        public ApiResponseMessage StopTask(int id)
        {
            var task = QueryById(id);
            if (task != null && task.Status > (int)TaskStatus.Stop)
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

        public ApiResponseMessage DeleteTask(int id)
        {
            var task = QueryById(id);
            if (task != null && task.Status != (int)TaskStatus.Deleted)
            {
                if (task.Status == (int)TaskStatus.Stop)
                {
                    //停止状态下的才能删除
                    _repositoryFactory.Tasks.UpdateBy(m => m.Id == task.Id, m => new TaskEntity
                    {
                        Status = (int)TaskStatus.Deleted,
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
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Guid AddRunTrace(int taskId)
        {
            TaskRunTraceEntity entity = new TaskRunTraceEntity();
            entity.TraceId = Guid.NewGuid();
            entity.TaskId = taskId;
            entity.StartTime = DateTime.Now;
            entity.Result = (int)TaskRunResult.Null;
            _repositoryFactory.TaskRunTraces.Add(entity);
            if (_unitOfWork.Commit() > 0)
            {
                return entity.TraceId;
            }
            return Guid.Empty;
        }

        public bool UpdateRunTrace(Guid traceId, double timeSpan, TaskRunResult result)
        {
            if (traceId == Guid.Empty)
            {
                return false;
            }
            _repositoryFactory.TaskRunTraces.UpdateBy(x => x.TraceId == traceId, x => new TaskRunTraceEntity
            {
                EndTime = DateTime.Now,
                Result = (int)result,
                TimeSpan = timeSpan
            });
            return _unitOfWork.Commit() > 0;
        }
    }
}
