using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Log;
using Hos.ScheduleMaster.Web.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        public GlobalExceptionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            LogHelper.Error(context.HttpContext.Request.Path, context.Exception);
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                var accept = context.HttpContext.Request.Headers["Accept"].FirstOrDefault();
                if (accept.Contains("application/json"))
                {
                    context.Result = new JsonNetResult()
                    {
                        Data = new { Success = false, Message = "服务出现异常请稍后再试！" }
                    };
                }
                else
                {
                    context.Result = new JavaScriptResult("alert('服务出现异常请稍后再试！')");
                }
            }
            else
            {
                context.Result = new RedirectResult("/Static/Page404");
            }
            context.ExceptionHandled = true;
        }
    }
}
