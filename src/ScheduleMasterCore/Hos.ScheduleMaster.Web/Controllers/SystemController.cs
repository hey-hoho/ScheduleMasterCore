using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class SystemController : AdminController
    {
        [Autowired]
        public ISystemService _systemService { get; set; }

        //public SystemController(ISystemService systemService)
        //{
        //    _systemService = systemService;
        //}

        // GET: System
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 参数配置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Config()
        {
            var data = _systemService.GetConfigList();
            return View(data);
        }

        /// <summary>
        /// 保存参数配置
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveConfig(FormCollection form)
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach (string key in form.Keys)
            {
                items.Add(key, form[key]);
            }
            bool result = _systemService.SaveConfig(items);
            if (result)
            {
                return SuccessTip("保存成功！");
            }
            return DangerTip("保存失败！");
        }
    }
}
