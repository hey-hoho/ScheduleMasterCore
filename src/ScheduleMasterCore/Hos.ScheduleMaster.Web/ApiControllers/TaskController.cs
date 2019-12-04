using System;
using System.Collections.Generic;
using System.Linq;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hos.ScheduleMaster.Web.ApiControllers
{
    [Route("api/[controller]")]
    public class TaskController : ApiController
    {
        [Autowired]
        public IScheduleService _scheduleService { get; set; }

        //private readonly ILogger<TaskController> _logger;

        //public TaskController(IScheduleService taskService)
        //{
        //    _taskService = taskService;
        //}

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet, Route("QueryList")]
        public object QueryList(string name = "")
        {
            var pager = new ListPager<ScheduleEntity>();
            if (!string.IsNullOrEmpty(name))
            {
                pager.AddFilter(m => m.Title.Contains(name));
            }
            pager = _scheduleService.QueryPager(pager);
            var result = new
            {
                total = pager.Total,
                rows = pager.Rows.Select(m => new
                {
                    m.CreateTime,
                    m.Id,
                    StartTime = m.StartDate,
                    m.LastRunTime,
                    m.NextRunTime,
                    RunMode = m.RunMoreTimes ? "周期运行" : "一次运行",
                    m.Remark,
                    m.Status,
                    m.Title,
                    m.TotalRunCount
                })
            };
            return result;
        }

        /// <summary>
        /// 查询详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("QueryTaskDetail")]
        public ServiceResponseMessage QueryTaskDetail(Guid id)
        {
            var entity = _scheduleService.QueryById(id);
            return ApiResponse(ResultStatus.Success, "请求数据成功", entity);
        }

        /// <summary>
        /// 查询日志记录
        /// </summary>
        /// <param name="enddate"></param>
        /// <param name="task"></param>
        /// <param name="startdate"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet, Route("QueryLogPager")]
        public object QueryLogPager(DateTime? startdate, DateTime? enddate, Guid? task, int? category)
        {
            var pager = new ListPager<SystemLogEntity>();
            if (task.HasValue)
            {
                pager.AddFilter(m => m.ScheduleId == task);
            }
            if (category.HasValue && category.Value > 0)
            {
                pager.AddFilter(m => m.Category == category);
            }
            if (startdate.HasValue)
            {
                pager.AddFilter(m => m.CreateTime >= startdate);
            }
            if (enddate.HasValue)
            {
                pager.AddFilter(m => m.CreateTime <= enddate);
            }
            pager = _scheduleService.QueryLogPager(pager);
            var result = new
            {
                total = pager.Total,
                rows = pager.Rows
            };
            return result;
        }

        /// <summary>
        /// 创建任务并启动
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost, Route("CreateTask")]
        // [ApiParamValidation]
        public ServiceResponseMessage CreateTask([FromBody]ScheduleInfo task)
        {
            ScheduleEntity model = new ScheduleEntity
            {
                AssemblyName = task.AssemblyName,
                ClassName = task.ClassName,
                CreateTime = DateTime.Now,
                CronExpression = task.CronExpression,
                EndDate = task.EndDate,
                Remark = task.Remark,
                StartDate = task.StartDate,
                Title = task.Title,
                Status = (int)ScheduleStatus.Stop,
                CustomParamsJson = task.CustomParamsJson,
                RunMoreTimes = task.RunMoreTimes,
                TotalRunCount = 0
            };
            var result = _scheduleService.AddTask(model, task.Guardians, task.Nexts);
            if (result.Status == ResultStatus.Success)
            {
                if (task.RunNow)
                {
                    var start = _scheduleService.TaskStart(model);
                    return ApiResponse(ResultStatus.Success, "任务创建成功！启动状态为：" + (start.Status == ResultStatus.Success ? "成功" : "失败"), model.Id);
                }
            }
            return result;
        }

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost, Route("EditTask")]
        //[ApiParamValidation]
        public ServiceResponseMessage EditTask([FromBody]ScheduleInfo task)
        {
            var result = _scheduleService.EditTask(task);
            return result;
        }

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("StartTask")]
        public ServiceResponseMessage StartTask(Guid id)
        {
            var task = _scheduleService.QueryById(id);
            if (task == null || task.Status != (int)ScheduleStatus.Stop)
            {
                return ApiResponse(ResultStatus.Failed, "任务在停止状态下才能启动！");
            }
            var result = _scheduleService.TaskStart(task);
            return result;
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("PauseTask")]
        public ServiceResponseMessage PauseTask(Guid id)
        {
            var result = _scheduleService.PauseTask(id);
            return result;
        }

        /// <summary>
        /// 立即运行一次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("RunOnceTask")]
        public ServiceResponseMessage RunOnceTask(Guid id)
        {
            var result = _scheduleService.RunOnceTask(id);
            return result;
        }

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("ResumeTask")]
        public ServiceResponseMessage ResumeTask(Guid id)
        {
            var result = _scheduleService.ResumeTask(id);
            return result;
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("StopTask")]
        public ServiceResponseMessage StopTask(Guid id)
        {
            var result = _scheduleService.StopTask(id);
            return result;
        }

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("DeleteTask")]
        public ServiceResponseMessage DeleteTask(Guid id)
        {
            var result = _scheduleService.DeleteTask(id);
            return result;
        }
    }
}
