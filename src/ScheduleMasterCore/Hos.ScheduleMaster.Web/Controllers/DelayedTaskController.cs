using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Common;
using Microsoft.AspNetCore.Http;
using System.IO;
using Hos.ScheduleMaster.Web.Extension;

namespace Hos.ScheduleMaster.Web.Controllers
{
    [Route("/[controller]/[action]")]
    public class DelayedTaskController : AdminController
    {
        [Autowired]
        public IDelayedTaskService _taskService { get; set; }

        [Autowired]
        public INodeService _nodeService { get; set; }

        /// <summary>
        /// 任务列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.PagerQueryUrl = Url.Action("QueryPager", "DelayedTask");
            return View();
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QueryPager(DateTime? startDate, DateTime? endDate, int? status, string topic = "", string contentkey = "", string workerName = "")
        {
            var pager = new ListPager<ScheduleDelayedInfo>(PageIndex, PageSize);
            if (startDate.HasValue)
            {
                pager.AddFilter(m => m.CreateTime >= startDate);
            }
            if (endDate.HasValue)
            {
                pager.AddFilter(m => m.CreateTime <= endDate);
            }
            if (status.HasValue && status.Value >= 0)
            {
                pager.AddFilter(m => m.Status == status.Value);
            }
            if (!string.IsNullOrEmpty(topic))
            {
                pager.AddFilter(m => m.Topic.Contains(topic));
            }
            if (!string.IsNullOrEmpty(contentkey))
            {
                pager.AddFilter(m => m.ContentKey.Contains(contentkey));
            }
            pager = _taskService.QueryPager(pager, workerName);
            return GridData(pager.Total, pager.Rows);
        }

        /// <summary>
        /// 创建页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.WorkerList = _nodeService.QueryWorkerList();
            return View();
        }

        /// <summary>
        /// 创建页面提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create(ScheduleDelayedInfo task)
        {
            if (!ModelState.IsValid)
            {
                return this.JsonNet(false, "数据验证失败！");
            }
            ScheduleDelayedEntity entity = ObjectMapper<ScheduleDelayedInfo, ScheduleDelayedEntity>.Convert(task);
            entity.Status = (int)ScheduleDelayStatus.Created;
            entity.CreateUserName = CurrentAdmin.UserName;
            var result = await _taskService.Add(entity, task.Executors);
            if (result.Status == ResultStatus.Success)
            {
                return this.JsonNet(true, "任务创建成功！", Url.Action("Index"));
            }
            return this.JsonNet(false, "任务创建失败！");
        }

        /// <summary>
        /// 详情页面
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult Detail(Guid sid)
        {
            var model = _taskService.QueryById(sid);
            if (model == null)
            {
                return PageNotFound();
            }
            return View(model);
        }

        /// <summary>
        /// 执行一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Execute([FromQuery]Guid id)
        {
            var result = await _taskService.Execute(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 重置一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Start([FromQuery]Guid id)
        {
            var result = await _taskService.Start(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 作废任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Stop([FromQuery]Guid id)
        {
            var result = await _taskService.Stop(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }
    }
}
