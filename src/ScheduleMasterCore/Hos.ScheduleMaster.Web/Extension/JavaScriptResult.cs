using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.Extension
{
    public class JavaScriptResult : ActionResult
    {
        public string Scripts;

        public JavaScriptResult()
        {

        }
        public JavaScriptResult(string scrpits)
        {
            this.Scripts = scrpits;
        }
        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("ActionContext");
            }
            HttpResponse response = context.HttpContext.Response;
            response.ContentType = "application/javascript";
            response.WriteAsync(Scripts);
        }
    }

    public static class JavaScriptResultExtension
    {
        public static JavaScriptResult JavaScript(this Controller controller, string scripts)
        {
            return new JavaScriptResult(scripts);
        }
    }
}
