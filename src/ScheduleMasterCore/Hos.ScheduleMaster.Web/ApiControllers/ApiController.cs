using Hos.ScheduleMaster.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.ApiControllers
{
    [ApiController]
    public class ApiController: ControllerBase
    {
        /// <summary>
        /// 接口统一的返回消息
        /// </summary>
        /// <param name="result">返回内容</param>
        /// <returns></returns>
        protected ApiResponseMessage ApiResponse(ResultStatus status, string message, object data = null)
        {
            return new ApiResponseMessage(status, message, data);
        }
    }
}
