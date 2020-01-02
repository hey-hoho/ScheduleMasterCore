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
    [Route("api/[controller]/[action]")]
    public class TaskController : ApiController
    {
        [Autowired]
        public IScheduleService _scheduleService { get; set; }

        [Autowired]
        public ISystemService _systemService { get; set; }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public object QueryList(string name = "")
        {
            var pager = new ListPager<ScheduleEntity>(PageIndex, PageSize);
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
                    RunMode = m.RunLoop ? "周期运行" : "一次运行",
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
        [HttpGet]
        public ServiceResponseMessage QueryTaskDetail(Guid id)
        {
            var entity = _scheduleService.QueryById(id);
            return ApiResponse(ResultStatus.Success, "请求数据成功", entity);
        }


        /// <summary>
        /// 创建任务并启动
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        // [ApiParamValidation]
        public ServiceResponseMessage Create([FromForm]ScheduleInfo task)
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
                RunLoop = task.RunLoop,
                TotalRunCount = 0,
                CreateUserName = task.CreateUserName
            };
            var result = _scheduleService.Add(model, task.Keepers, task.Nexts);
            if (result.Status == ResultStatus.Success)
            {
                if (task.RunNow)
                {
                    var start = _scheduleService.Start(model);
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
        [HttpPost]
        //[ApiParamValidation]
        public ServiceResponseMessage Edit([FromForm]ScheduleInfo task)
        {
            var result = _scheduleService.Edit(task);
            return result;
        }

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResponseMessage Start([FromQuery]Guid id)
        {
            var task = _scheduleService.QueryById(id);
            var result = _scheduleService.Start(task);
            return result;
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResponseMessage Pause([FromQuery]Guid id)
        {
            var result = _scheduleService.Pause(id);
            return result;
        }

        /// <summary>
        /// 立即运行一次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResponseMessage RunOnce([FromQuery]Guid id)
        {
            var result = _scheduleService.RunOnce(id);
            return result;
        }

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResponseMessage Resume([FromQuery]Guid id)
        {
            var result = _scheduleService.Resume(id);
            return result;
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResponseMessage Stop([FromQuery]Guid id)
        {
            var result = _scheduleService.Stop(id);
            return result;
        }
    }
}
