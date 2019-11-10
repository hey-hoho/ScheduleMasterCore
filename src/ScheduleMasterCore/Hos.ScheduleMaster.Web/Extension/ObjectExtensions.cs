using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.Extension
{
    public static class ObjectExtensions
    {
        public static bool IsAjaxRequest(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            bool isAjax = false;
            var xreq = request.Headers.ContainsKey("x-requested-with");
            if (xreq)
            {
                isAjax = request.Headers["x-requested-with"] == "XMLHttpRequest";
            }
            return isAjax;
        }
    }
}
