using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.ApiControllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        public int PageIndex
        {
            get
            {
                var value = Request.Query["pageNumber"];
                if (string.IsNullOrEmpty(value))
                {
                    value = Request.Form["pageNumber"];
                }
                if (string.IsNullOrEmpty(value))
                {
                    return 1;
                }
                return Convert.ToInt32(value);
            }
        }
        public int PageSize
        {
            get
            {
                var value = Request.Query["pageSize"];
                if (string.IsNullOrEmpty(value))
                {
                    value = Request.Form["pageSize"];
                }
                if (string.IsNullOrEmpty(value))
                {
                    return 1;
                }
                return Convert.ToInt32(value);
            }
        }

        public string CurrentUserName => HttpContext.Request.Headers["ms_auth_user"].FirstOrDefault();

        /// <summary>
        /// 接口统一的返回消息
        /// </summary>
        /// <param name="result">返回内容</param>
        /// <returns></returns>
        protected ServiceResponseMessage ApiResponse(ResultStatus status, string message, object data = null)
        {
            return new ServiceResponseMessage(status, message, data);
        }

    }
}
