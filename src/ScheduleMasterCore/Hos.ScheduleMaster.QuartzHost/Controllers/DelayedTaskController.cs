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
using Hos.ScheduleMaster.QuartzHost.DelayedTask;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace Hos.ScheduleMaster.QuartzHost.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DelayedTaskController : ControllerBase
    {
        private readonly ILogger<DelayedTaskController> _logger;

        public DelayedTaskController(ILogger<DelayedTaskController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromForm]Guid sid)
        {
            bool success = await DelayPlanManager.InsertById(sid);
            if (success) return Ok();
            return BadRequest();
        }


        [HttpPost]
        public IActionResult Remove([FromForm]Guid sid)
        {
            bool success = DelayPlanManager.Remove(sid.ToString());
            if (success) return Ok();
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Execute([FromServices]SmDbContext db, [FromServices]IHttpClientFactory clientFactory, [FromForm]Guid sid)
        {
            var entity = await db.ScheduleDelayeds.FirstOrDefaultAsync(x => x.Id == sid);
            if (entity != null)
            {
                entity.ExecuteTime = DateTime.Now;
                try
                {
                    var httpClient = clientFactory.CreateClient();
                    string notifyBody = entity.NotifyBody.Replace("\r\n", "");
                    HttpContent reqContent = new StringContent(notifyBody, System.Text.Encoding.UTF8, "application/json");
                    if (entity.NotifyDataType == "application/x-www-form-urlencoded")
                    {
                        //任务创建是要确保参数是键值对的json格式
                        reqContent = new FormUrlEncodedContent(Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(notifyBody));
                    }
                    var response = await httpClient.PostAsync(entity.NotifyUrl, reqContent);
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode && content.Contains("success"))
                    {
                        entity.FinishTime = DateTime.Now;
                        entity.Status = (int)ScheduleDelayStatus.Successed;
                        entity.Remark += " | 手动执行回调成功";
                        db.Update(entity);
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        LogHelper.Warn($"延时任务[{entity.Topic}:{entity.ContentKey}]执行失败。响应码：{response.StatusCode.GetHashCode()}，响应内容：{(response.Content.Headers.GetValues("Content-Type").Any(x => x.Contains("text/html")) ? "html文档" : content)}", sid);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"延时任务[{entity.Topic}:{entity.ContentKey}]执行失败。", ex, sid);
                }
            }
            return BadRequest();
        }

    }
}
