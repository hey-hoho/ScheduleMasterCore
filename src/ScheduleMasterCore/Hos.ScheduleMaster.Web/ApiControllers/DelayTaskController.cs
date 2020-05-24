using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hos.ScheduleMaster.Web.ApiControllers
{
    [Route("api/[controller]/[action]")]
    public class DelayTaskController : ApiController
    {
        [Autowired]
        public IDelayedTaskService _taskService { get; set; }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ServiceResponseMessage> Create([FromForm]ScheduleDelayedInfo task)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(ResultStatus.ParamError, "非法提交参数");
            }
            ScheduleDelayedEntity entity = ObjectMapper<ScheduleDelayedInfo, ScheduleDelayedEntity>.Convert(task);
            entity.Status = (int)ScheduleDelayStatus.Created;
            entity.CreateUserName = CurrentUserName;
            var result = await _taskService.Add(entity, task.Executors);
            return result;
        }
    }
}