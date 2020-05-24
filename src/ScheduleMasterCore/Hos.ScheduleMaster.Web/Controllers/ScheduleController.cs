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

        [Autowired]
        public INodeService _nodeService { get; set; }

        /// <summary>
        /// 任务列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.PagerQueryUrl = Url.Action("QueryPager", "Schedule");
            return View();
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="status"></param>
        /// <param name="title"></param>
        /// <param name="workerName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QueryPager(int? status, string title = "", string workerName = "")
        {
            return QueryList(null, status, title, workerName);
        }

        /// <summary>
        /// 我负责监护的任务列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="title"></param>
        /// <param name="workerName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QueryCurrentUserPager(int? status, string title, string workerName)
        {
            return QueryList(CurrentAdmin.Id, status, title, workerName);
        }

        /// <summary>
        /// 通用列表查询
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="title"></param>
        /// <param name="workerName"></param>
        /// <returns></returns>
        private ActionResult QueryList(int? userId, int? status, string title, string workerName)
        {
            var pager = new ListPager<ScheduleInfo>(PageIndex, PageSize);
            if (status.HasValue && status.Value >= 0)
            {
                pager.AddFilter(m => m.Status == status.Value);
            }
            if (!string.IsNullOrEmpty(title))
            {
                pager.AddFilter(m => m.Title.Contains(title));
            }
            pager = _scheduleService.QueryPager(pager, userId, workerName);
            return GridData(pager.Total, pager.Rows);
        }

        /// <summary>
        /// 详情页面
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult Detail(Guid sid)
        {
            var model = _scheduleService.QueryScheduleContext(sid);
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
            ViewBag.WorkerList = _nodeService.QueryWorkerList();
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
                var filePath = $"{ConfigurationCache.PluginPathPrefix}\\{file.FileName}".ToPhysicalPath();
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
        public async Task<ActionResult> Create(ScheduleInfo task)
        {
            if (!ModelState.IsValid)
            {
                return this.JsonNet(false, "数据验证失败！");
            }
            var admin = CurrentAdmin;
            ScheduleEntity main = new ScheduleEntity
            {
                MetaType = task.MetaType,
                CronExpression = task.CronExpression,
                EndDate = task.EndDate,
                Remark = task.Remark,
                StartDate = task.StartDate,
                Title = task.Title,
                Status = (int)ScheduleStatus.Stop,
                CustomParamsJson = task.CustomParamsJson,
                RunLoop = task.RunLoop,
                TotalRunCount = 0,
                CreateUserName = admin.UserName
            };
            if (task.MetaType == (int)ScheduleMetaType.Assembly)
            {
                main.AssemblyName = task.AssemblyName;
                main.ClassName = task.ClassName;
            }
            ScheduleHttpOptionEntity httpOption = null;
            if (task.MetaType == (int)ScheduleMetaType.Http)
            {
                httpOption = new ScheduleHttpOptionEntity
                {
                    RequestUrl = task.HttpRequestUrl,
                    Method = task.HttpMethod,
                    ContentType = task.HttpContentType,
                    Headers = task.HttpHeaders,
                    Body = task.HttpBody
                };
            }
            var result = _scheduleService.Add(main, httpOption, task.Keepers, task.Nexts, task.Executors);
            if (result.Status == ResultStatus.Success)
            {
                if (task.RunNow)
                {
                    var start = await _scheduleService.Start(main);
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
            ViewBag.WorkerList = _nodeService.QueryWorkerList();
            ScheduleInfo viewer = ObjectMapper<ScheduleEntity, ScheduleInfo>.Convert(model);
            if (model.MetaType == (int)ScheduleMetaType.Http)
            {
                ObjectMapper<ScheduleHttpOptionEntity, ScheduleInfo>.Convert(_scheduleService.QueryScheduleHttpOptions(id), viewer);
            }
            viewer.Keepers = _scheduleService.QueryScheduleKeepers(id).Select(x => x.UserId).ToList();
            viewer.Nexts = _scheduleService.QueryScheduleReferences(id).Select(x => x.ChildId).ToList();
            viewer.Executors = _scheduleService.QueryScheduleExecutors(id).Select(x => x.WorkerName).ToList();
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
            if (!ModelState.IsValid)
            {
                return this.JsonNet(false, "数据验证失败！");
            }
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
        public async Task<ActionResult> Start([FromQuery]Guid id)
        {
            var task = _scheduleService.QueryById(id);
            var result = await _scheduleService.Start(task);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Pause([FromQuery]Guid id)
        {
            var result = await _scheduleService.Pause(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 立即运行一次
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RunOnce([FromQuery]Guid id)
        {
            var result = await _scheduleService.RunOnce(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Resume([FromQuery]Guid id)
        {
            var result = await _scheduleService.Resume(id);
            return this.JsonNet(result.Status == ResultStatus.Success, result.Message);
        }

        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Stop([FromQuery]Guid id)
        {
            var result = await _scheduleService.Stop(id);
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
            if (result.HasValue)
            {
                pager.AddFilter(m => m.Result == result.Value);
            }
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
