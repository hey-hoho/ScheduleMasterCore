using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.QuartzHost.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Log;
using System.Net;
using System.IO.Compression;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Hos.ScheduleMaster.QuartzHost.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class QuartzController : ControllerBase
    {
        private readonly ILogger<QuartzController> _logger;
        private SmDbContext _db;

        public QuartzController(ILogger<QuartzController> logger, SmDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Start(Guid sid)
        {
            var model = _db.Schedules.FirstOrDefault(x => x.Id == sid && x.Status == (int)ScheduleStatus.Stop);
            if (model != null)
            {
                await LoadPluginFile(model.AssemblyName);
                ScheduleView view = new ScheduleView() { Schedule = model };
                view.Keepers = (from t in _db.ScheduleKeepers
                                join u in _db.SystemUsers on t.UserId equals u.Id
                                where t.ScheduleId == model.Id && !string.IsNullOrEmpty(u.Email)
                                select new KeyValuePair<string, string>(u.RealName, u.Email)
                        ).ToList();
                view.Children = (from c in _db.ScheduleReferences
                                 join t in _db.Schedules on c.ChildId equals t.Id
                                 where c.ScheduleId == model.Id
                                 select new { t.Id, t.Title }
                                ).ToDictionary(x => x.Id, x => x.Title);
                bool success = await QuartzManager.StartWithRetry(view, StartedEvent);
                if (success) return Ok();
            }
            return BadRequest();
        }

        private void StartedEvent(Guid sid, DateTime? nextRunTime)
        {
            //每次运行成功后更新任务的运行情况
            var task = _db.Schedules.FirstOrDefault(x => x.Id == sid);
            if (task == null) return;
            task.LastRunTime = DateTime.Now;
            task.NextRunTime = nextRunTime;
            task.TotalRunCount += 1;
            _db.SaveChanges();
            //LogHelper.Info($"任务[{task.Title}]运行成功！", task.Id);
        }

        private async Task LoadPluginFile(string name)
        {
            var sourcePath = "https://localhost:44301/static/downloadpluginfile?pluginname=test";
            var zipPath = $"{Directory.GetCurrentDirectory()}\\Plugins\\{name}.zip";
            var pluginPath = $"{Directory.GetCurrentDirectory()}\\Plugins\\{name}";
            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(sourcePath), zipPath);
                //将指定 zip 存档中的所有文件都解压缩到文件系统的一个目录下
                ZipFile.ExtractToDirectory(zipPath, pluginPath, true);
                System.IO.File.Delete(zipPath);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Stop(Guid sid)
        {
            bool success = await QuartzManager.Stop(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Pause(Guid sid)
        {
            bool success = await QuartzManager.Pause(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Resume(Guid sid)
        {
            bool success = await QuartzManager.Resume(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> RunOnce(Guid sid)
        {
            bool success = await QuartzManager.RunOnce(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok("i am ok");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Get()
        {
            var sourcePath = "https://localhost:44301/static/downloadpluginfile?pluginname=test";
            var zipPath = $"{Directory.GetCurrentDirectory()}\\Plugins\\test.zip";
            var pluginPath = $"{Directory.GetCurrentDirectory()}\\Plugins\\test";
            WebClient client = new WebClient();
            client.DownloadFile(new Uri(sourcePath), zipPath);
            //将指定 zip 存档中的所有文件都解压缩到文件系统的一个目录下
            ZipFile.ExtractToDirectory(zipPath, pluginPath, true);
            System.IO.File.Delete(zipPath);
            return Ok("5555555555555555");
            //Common.QuartzManager.StartWithRetry(new Core.Models.ScheduleView
            //{
            //    Schedule = new Core.Models.ScheduleEntity
            //    {
            //        AssemblyName = "Hos.ScheduleMaster.Demo",
            //        ClassName = "Hos.ScheduleMaster.Demo.Simple",
            //        CreateTime = DateTime.Now,
            //        CreateUserId = 1,
            //        CreateUserName = "hh",
            //        CronExpression = "0/1 * * * * ?",
            //        Id = Guid.NewGuid(),
            //        RunMoreTimes = true,
            //        StartDate = DateTime.Now.AddMinutes(-3),
            //        Status = 0,
            //        Title = "测试任务"
            //    }
            //}, (sid, time) => { });
        }
    }
}
