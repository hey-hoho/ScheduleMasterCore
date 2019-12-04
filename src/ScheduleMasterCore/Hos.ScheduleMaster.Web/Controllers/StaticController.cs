using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var stream = System.IO.File.OpenRead($"{System.IO.Directory.GetCurrentDirectory()}\\Plugins\\{pluginName}.zip");
            return File(stream, "application/octet-stream", $"{pluginName}.zip");
        }
    }
}