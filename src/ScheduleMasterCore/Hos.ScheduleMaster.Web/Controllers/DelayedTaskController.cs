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
        public ActionResult QueryPager(string topic = "", string contentkey = "", int? status = null, string workerName = "")
        {
            var pager = new ListPager<ScheduleDelayedInfo>(PageIndex, PageSize);
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
        public ActionResult Execute([FromQuery]Guid id)
        {
            var result = _taskService.Execute(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 重置一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Start([FromQuery]Guid id)
        {
            var result = _taskService.Start(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 作废任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Stop([FromQuery]Guid id)
        {
            var result = _taskService.Stop(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }
    }
}
