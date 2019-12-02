using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.QuartzHost.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Log;

namespace Hos.ScheduleMaster.QuartzHost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuartzController : ControllerBase
    {
        private readonly ILogger<QuartzController> _logger;
        private SmDbContext _db;

        public QuartzController(ILogger<QuartzController> logger, SmDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost, Route("Start")]
        public async Task<IActionResult> Start(Guid sid)
        {
            var task = _db.Schedules.FirstOrDefault(x => x.Id == sid && x.Status == (int)ScheduleStatus.Stop);
            if (task != null)
            {
                ScheduleView view = new ScheduleView() { Schedule = task };
                view.Keepers = (from t in _db.ScheduleKeepers
                                join u in _db.SystemUsers on t.UserId equals u.Id
                                where t.ScheduleId == task.Id && !string.IsNullOrEmpty(u.Email)
                                select new KeyValuePair<string, string>(u.RealName, u.Email)
                        ).ToList();
                view.Children = (from c in _db.ScheduleReferences
                                 join t in _db.Schedules on c.ChildId equals t.Id
                                 where c.ScheduleId == task.Id
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

        [HttpPost, Route("Stop")]
        public async Task<IActionResult> Stop(Guid sid)
        {
            bool success = await QuartzManager.Stop(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost, Route("Pause")]
        public async Task<IActionResult> Pause(Guid sid)
        {
            bool success = await QuartzManager.Pause(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost, Route("Resume")]
        public async Task<IActionResult> Resume(Guid sid)
        {
            bool success = await QuartzManager.Resume(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost, Route("RunOnce")]
        public async Task<IActionResult> RunOnce(Guid sid)
        {
            bool success = await QuartzManager.RunOnce(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpGet]
        public IActionResult Get()
        {
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
