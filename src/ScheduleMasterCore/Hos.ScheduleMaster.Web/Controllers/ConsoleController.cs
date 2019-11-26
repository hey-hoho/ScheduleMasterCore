using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class ConsoleController : AdminController
    {

        public IActionResult Index()
        {
            ViewBag.CurrentAdmin = CurrentAdmin;
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }
    }
}