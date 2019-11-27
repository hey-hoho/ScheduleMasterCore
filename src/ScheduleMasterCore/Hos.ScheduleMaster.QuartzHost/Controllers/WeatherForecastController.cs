using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hos.ScheduleMaster.QuartzHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public void Get()
        {
            Common.QuartzManager.StartWithRetry(new Core.Models.ScheduleView
            {
                Schedule = new Core.Models.ScheduleEntity
                {
                    AssemblyName = "Hos.ScheduleMaster.Demo",
                    ClassName = "Hos.ScheduleMaster.Demo.Simple",
                    CreateTime = DateTime.Now,
                    CreateUserId = 1,
                    CreateUserName = "hh",
                    CronExpression = "0/1 * * * * ?",
                    Id = Guid.NewGuid(),
                    RunMoreTimes = true,
                    StartDate = DateTime.Now.AddMinutes(-3),
                    Status = 0,
                    Title = "测试任务"
                }
            }, (time) => { });
        }
    }
}
