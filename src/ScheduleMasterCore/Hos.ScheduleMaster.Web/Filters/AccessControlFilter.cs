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
    public class AccessControlFilter : IActionFilter
    {
        private IHttpContextAccessor _accessor;
        private SmDbContext _db;

        public AccessControlFilter(IHttpContextAccessor accessor, SmDbContext db)
        {
            _accessor = accessor;
            _db = db;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string userName = context.HttpContext.Request.Headers["ms_auth_user"].FirstOrDefault();
            string secret = context.HttpContext.Request.Headers["ms_auth_secret"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(secret))
            {
                var user = _db.SystemUsers.FirstOrDefault(x => x.UserName == userName);
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
