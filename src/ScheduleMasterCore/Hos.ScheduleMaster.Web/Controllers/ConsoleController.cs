using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Web.Extension;
using Microsoft.AspNetCore.Mvc;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class ConsoleController : AdminController
    {
        [Autowired]
        public IScheduleService _scheduleService { get; set; }

        [Autowired]
        public INodeService _nodeService { get; set; }

        public IActionResult Index()
        {
            ViewBag.CurrentAdmin = CurrentAdmin;
            return View();
        }

        public IActionResult MySchedule()
        {
            ViewBag.PagerQueryUrl = Url.Action("QueryCurrentUserPager", "Schedule");
            return View("../Schedule/Index");
        }

        public ActionResult Home()
        {
            //我负责的top任务，创建时间倒序
            var pager = new ListPager<ScheduleInfo>(1, 7);
            ViewBag.MySchedule = _scheduleService.QueryPager(pager, CurrentAdmin.Id).Rows;
            return View();
        }

        [HttpGet]
        public ActionResult GetHomeSummary()
        {
            //总任务数
            int task_total = _scheduleService.QueryScheduleCount(null);
            //运行中任务数
            int task_running = _scheduleService.QueryScheduleCount((int)ScheduleStatus.Running);
            //总用户数
            int user_total = _accountService.QueryUserCount(null);
            //总运行次数
            int trace_total = _scheduleService.QueryTraceCount(null);
            //累计运行成功次数
            int trace_success = _scheduleService.QueryTraceCount(1);
            //累计运行失败次数
            int trace_failed = _scheduleService.QueryTraceCount(2);
            //总节点数
            int worker_total = _nodeService.QueryWorkerCount(null);
            //工作中节点数
            int worker_running = _nodeService.QueryWorkerCount(2);
            //休息中节点数
            int worker_ready = _nodeService.QueryWorkerCount(1);
            //近一周运行成功数据
            object weekly_success = _scheduleService.QueryTraceWeeklyReport(1);
            //近一周运行失败数据
            object weekly_failed = _scheduleService.QueryTraceWeeklyReport(2);
            return this.JsonNet(true, data: new { task_total, task_running, user_total, trace_total, trace_success, trace_failed, worker_total, worker_running, worker_ready, weekly_success, weekly_failed });
        }
    }
}