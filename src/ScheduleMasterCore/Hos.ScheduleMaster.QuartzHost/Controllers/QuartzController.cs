﻿using Hos.ScheduleMaster.QuartzHost.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Start([FromForm]Guid sid)
        {
            bool success = await QuartzManager.StartWithRetry(sid);
            if (success) return Ok();
            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> Stop([FromForm]Guid sid)
        {
            bool success = await QuartzManager.Stop(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Pause([FromForm]Guid sid)
        {
            bool success = await QuartzManager.Pause(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Resume([FromForm]Guid sid)
        {
            bool success = await QuartzManager.Resume(sid);
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> RunOnce([FromForm]Guid sid)
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
