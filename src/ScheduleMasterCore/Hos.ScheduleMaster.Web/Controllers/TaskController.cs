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

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class TaskController : AdminController
    {
        [Autowired]
        public IScheduleService _scheduleService { get; set; }

        //public TaskController(IAccountService accountService, IScheduleService taskService)
        //{
        //    _taskService = taskService;
        //}


        /// <summary>
        /// 任务列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
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
        /// 创建任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateTask(ScheduleInfo task)
        {
            if (!ModelState.IsValid)
            {
                return DangerTip("数据验证失败！");
            }
            IFormFile file = Request.Form.Files["file"];
            if (file != null && file.Length > 0)
            {
                var filePath = Directory.GetCurrentDirectory() + "/Plugins/" + file.FileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
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
                    return SuccessTip("任务创建成功！启动状态为：" + (start.Status == ResultStatus.Success ? "成功" : "失败"), Url.Action("Index"));
                }
                return SuccessTip("任务创建成功！", Url.Action("Index"));
            }
            return DangerTip("任务创建失败！");
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
            viewer.Guardians = _scheduleService.QueryScheduleKeepers(id).Select(x => x.UserId).ToList();
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
        public ActionResult EditTask(ScheduleInfo task)
        {
            var result = _scheduleService.EditTask(task);
            if (result.Status == ResultStatus.Success)
            {
                return SuccessTip("任务编辑成功！", Url.Action("Index"));
            }
            return DangerTip("任务编辑失败！");

        }
        /// <summary>
        /// 日志列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Log()
        {
            return View();
        }

        /// <summary>
        /// 清理日志页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearLog()
        {
            List<SelectListItem> selectData = new List<SelectListItem>();
            selectData.Add(new SelectListItem() { Text = "系统日志", Value = "0" });
            selectData.AddRange(_scheduleService.QueryAll().Select(row => new SelectListItem
            {
                Text = row.Title,
                Value = row.Id.ToString(),
                Selected = false
            }));
            ViewBag.TaskList = selectData;
            return View();
        }

        /// <summary>
        /// 清理日志
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        [HttpPost, AjaxRequestOnly]
        public ActionResult ClearLog(Guid? sid, int? category, DateTime? startdate, DateTime? enddate)
        {
            var result = _scheduleService.DeleteLog(sid, category, startdate, enddate);
            if (result > 0)
            {
                return SuccessTip($"清理成功！本次清理【{result}】条");
            }
            return DangerTip("没有符合条件的记录！");
        }
    }
}
