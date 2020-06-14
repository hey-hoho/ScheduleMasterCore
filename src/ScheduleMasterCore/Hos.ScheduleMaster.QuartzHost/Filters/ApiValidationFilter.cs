using Hos.ScheduleMaster.Core.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.Filters
{
    public class ApiValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var anonymous = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            if (anonymous.Any())
            {
                return;
            }
            var secret = context.HttpContext.Request.Headers["sm_secret"].FirstOrDefault();
            //LogHelper.Info($"w:{Common.QuartzManager.AccessSecret} m:{secret}");
            if (string.Compare(Common.QuartzManager.AccessSecret, secret, StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                context.Result = new UnauthorizedObjectResult($"w:{Common.QuartzManager.AccessSecret} m:{secret}");
            }
        }
    }
}
