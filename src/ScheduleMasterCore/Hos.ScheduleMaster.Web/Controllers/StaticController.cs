using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace Hos.ScheduleMaster.Web.Controllers
{
    /// <summary>
    /// 一些公共的静态页面
    /// </summary>
    public class StaticController : Controller
    {
        /// <summary>
        /// 404页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Page404()
        {
            return View();
        }

        /// <summary>
        /// 下载插件包
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public IActionResult DownloadPluginFile(string pluginName)
        {
            var path = $"{ConfigurationCache.PluginPathPrefix}\\{pluginName}.zip".ToPhysicalPath();
            //using (var stream = System.IO.File.OpenRead(path))
            {
                var stream = System.IO.File.OpenRead(path);
                return File(stream, "application/octet-stream", $"{pluginName}.zip");
            }
        }
    }
}