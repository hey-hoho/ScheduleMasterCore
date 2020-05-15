using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.QuartzHost.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Log;
using Microsoft.AspNetCore.Authorization;
using Hos.ScheduleMaster.Core;

namespace Hos.ScheduleMaster.QuartzHost.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class QuartzController : ControllerBase
    {
        private readonly ILogger<QuartzController> _logger;

        public QuartzController(ILogger<QuartzController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Start([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.StartWithRetry(sid);
            if (success) return Ok();
            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> Stop([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.Stop(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Pause([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.Pause(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Resume([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.Resume(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> RunOnce([FromQuery]Guid sid)
        {
            bool success = await QuartzManager.RunOnce(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Get()
        {
            return Ok("");
        }
    }
}
