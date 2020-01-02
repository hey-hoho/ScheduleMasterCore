using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Common;

namespace Hos.ScheduleMaster.Web.Filters
{
    public class AccessControlAttribute : IActionFilter
    {
        private IHttpContextAccessor _accessor;
        private IAccountService _account;

        public AccessControlAttribute(IHttpContextAccessor accessor, IAccountService account)
        {
            _accessor = accessor;
            _account = account;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //var conn = _accessor.HttpContext.Connection;
            //if (conn.RemoteIpAddress.Equals(conn.LocalIpAddress))
            //{
            //    //同域请求不做验证
            //    return;
            //}
            string userName = context.HttpContext.Request.Headers["ms_auth_user"].FirstOrDefault();
            string secret = context.HttpContext.Request.Headers["ms_auth_secret"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(secret))
            {
                var user = _account.GetUserbyUserName(userName);
                if (user != null && user.Status == (int)SystemUserStatus.Available)
                {
                    string se = SecurityHelper.MD5($"{userName}{user.Password}{userName}");
                    if (se == secret) return;
                }
            }
            context.Result = new UnauthorizedResult();
        }
    }
}
