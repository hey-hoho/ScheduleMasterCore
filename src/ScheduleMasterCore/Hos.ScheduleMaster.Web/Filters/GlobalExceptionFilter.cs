using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Log;
using Microsoft.AspNetCore.Hosting;
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
            if (context.Exception is UserFriendlyException)
            {

            }
            LogHelper.Error(context.Exception.Message, context.Exception);
            context.ExceptionHandled = true;
        }
    }
}
