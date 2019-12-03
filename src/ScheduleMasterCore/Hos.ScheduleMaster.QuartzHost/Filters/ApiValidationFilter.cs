using Microsoft.AspNetCore.Mvc;
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
            var secret = context.HttpContext.Request.Headers["sm_secret"].FirstOrDefault();
            if (!Common.QuartzManager.AccessSecret.Equals(secret))
            {
                context.Result = new BadRequestResult();
                context.Canceled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
