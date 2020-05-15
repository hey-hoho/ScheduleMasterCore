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
    public class ServerController : ControllerBase
    {
        private readonly ILogger<ServerController> _logger;

        private SmDbContext _db;

        public ServerController(ILogger<ServerController> logger, SmDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> StartUp()
        {
            try
            {
                DelayedTask.DelayPlanManager.Init();
                await QuartzManager.InitScheduler();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Shutdown()
        {
            try
            {
                DelayedTask.DelayPlanManager.Clear();
                await QuartzManager.Shutdown(false);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Connect()
        {
            string workerof = AppCommandResolver.GetTargetMasterName();
            string encodeKey = Request.Headers["sm_connection"].FirstOrDefault();
            if (string.IsNullOrEmpty(workerof) || string.IsNullOrEmpty(encodeKey))
            {
                _logger.LogWarning("connect failed! workerof or encodekey is null...");
                return BadRequest("Unauthorized Connection.");
            }
            if (!Core.Common.SecurityHelper.MD5(workerof).Equals(encodeKey))
            {
                _logger.LogWarning("connect failed! encodekey is unvalid, wokerof:{0}, encodekey:{1}", workerof, encodeKey);
                return BadRequest("Unauthorized Connection.");
            }
            string workerName = Request.Headers["sm_nameto"].FirstOrDefault();
            var node = _db.ServerNodes.FirstOrDefault(x => x.NodeName == workerName);
            if (node == null)
            {
                _logger.LogWarning("connect failed! unkown worker name:{0}...", workerName);
                return BadRequest("Unkown Worker Name.");
            }
            Core.ConfigurationCache.SetNode(node);
            string secret = Guid.NewGuid().ToString("n");
            QuartzManager.AccessSecret = secret;
            _logger.LogInformation("successfully connected to {0}!", workerof);
            LogHelper.Info($"与{workerof}连接成功~");
            return Ok(secret);
        }
    }
}
