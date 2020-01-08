using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Web.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Common;
using Microsoft.AspNetCore.Http;
using System.IO;
using Hos.ScheduleMaster.Web.Extension;

namespace Hos.ScheduleMaster.Web.Controllers
{
    [Route("/[controller]/[action]")]
    public class ScheduleController : AdminController
    {
        [Autowired]
        public IScheduleService _scheduleService { get; set; }

        /// <summary>
        /// 任务列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QueryPager(string name = "")
        {
            var pager = new ListPager<ScheduleEntity>(PageIndex, PageSize);
            if (!string.IsNullOrEmpty(name))
            {
                pager.AddFilter(m => m.Title.Contains(name));
            }
            pager = _scheduleService.QueryPager(pager);
            return GridData(pager.Total, pager.Rows.Select(m => new
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
            }));
        }

        /// <summary>
        /// 详情页面
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult Detail(Guid sid)
        {
            var model = _scheduleService.QueryScheduleView(sid);
            if (model == null || model.Schedule == null)
            {
                return PageNotFound();
            }
            return View(model);
        }

        /// <summary>
        /// 创建任务页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.UserList = _accountService.GetUserAll();
            ViewBag.TaskList = _scheduleService.QueryAll().ToDictionary(x => x.Id, x => x.Title);
            return View();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            IFormFile file = Request.Form.Files["file"];
            if (file != null && file.Length > 0)
            {
                var filePath = Directory.GetCurrentDirectory() + "/Plugins/" + file.FileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Content("ok");
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ScheduleInfo task)
        {
            if (!ModelState.IsValid)
            {
                return DangerTip("数据验证失败！");
            }
            var admin = CurrentAdmin;
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
                CreateUserName = admin.UserName,
                CreateUserId = admin.Id
            };
            var result = _scheduleService.Add(model, task.Keepers, task.Nexts);
            if (result.Status == ResultStatus.Success)
            {
                if (task.RunNow)
                {
                    var start = _scheduleService.Start(model);
                    return this.JsonNet(true, "任务创建成功！启动状态为：" + (start.Status == ResultStatus.Success ? "成功" : "失败"), Url.Action("Index"));
                }
                return this.JsonNet(true, "任务创建成功！", Url.Action("Index"));
            }
            return this.JsonNet(false, "任务创建失败！");
        }

        /// <summary>
        /// 编辑任务页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var model = _scheduleService.QueryById(id);
            if (model == null)
            {
                return PageNotFound();
            }
            ViewBag.UserList = _accountService.GetUserAll();
            ViewBag.TaskList = _scheduleService.QueryAll().ToDictionary(x => x.Id, x => x.Title);
            ScheduleInfo viewer = ObjectMapper<ScheduleEntity, ScheduleInfo>.Convert(model);
            viewer.Keepers = _scheduleService.QueryScheduleKeepers(id).Select(x => x.UserId).ToList();
            viewer.Nexts = _scheduleService.QueryScheduleReferences(id).Select(x => x.ChildId).ToList();
            return View("Create", viewer);
        }

        /// <summary>
        /// 编辑任务信息
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        //[ApiParamValidation]
        public ActionResult Edit(ScheduleInfo task)
        {
            var result = _scheduleService.Edit(task);
            if (result.Status == ResultStatus.Success)
            {
                return this.JsonNet(true, "任务编辑成功！", Url.Action("Index"));
            }
            return this.JsonNet(false, "任务编辑失败！");
        }

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Start([FromQuery]Guid id)
        {
            var task = _scheduleService.QueryById(id);
            var result = _scheduleService.Start(task);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Pause([FromQuery]Guid id)
        {
            var result = _scheduleService.Pause(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 立即运行一次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RunOnce([FromQuery]Guid id)
        {
            var result = _scheduleService.RunOnce(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Resume([FromQuery]Guid id)
        {
            var result = _scheduleService.Resume(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Stop([FromQuery]Guid id)
        {
            var result = _scheduleService.Stop(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete([FromQuery]Guid id)
        {
            var result = _scheduleService.Delete(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 任务运行记录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TraceLog()
        {
            return View();
        }

        /// <summary>
        /// 查询运行记录分页
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="result"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult QueryTraceLog(Guid? sid, int? result, DateTime startDate, DateTime endDate)
        {
            if (!sid.HasValue)
            {
                return NotFound();
            }
            var pager = new ListPager<ScheduleTraceEntity>(PageIndex, PageSize);
            pager.AddFilter(m => m.ScheduleId == sid.Value);
            pager.AddFilter(m => m.StartTime >= startDate && m.StartTime <= endDate);
            pager = _scheduleService.QueryTracePager(pager);
            return GridData(pager.Total, pager.Rows);
        }

        /// <summary>
        /// 查询运行记录日志
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="tid"></param>
        /// <returns></returns>
        public ActionResult QueryTraceDetail(Guid sid, Guid tid)
        {
            var pager = new ListPager<SystemLogEntity>(PageIndex, PageSize);
            pager.AddFilter(m => m.ScheduleId == sid);
            pager.AddFilter(m => m.TraceId == tid);
            pager = _scheduleService.QueryTraceDetail(pager);
            return GridData(pager.Total, pager.Rows);
        }
    }
}
