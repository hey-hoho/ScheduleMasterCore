using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Web.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class AdminController : Controller
    {
        [Autowired]
        public IAccountService _accountService { get; set; }

        public int PageIndex
        {
            get
            {
                var value = Request.Query["pageNumber"];
                if (string.IsNullOrEmpty(value) && Request.Method.ToLower() == "post" && Request.ContentType.Contains("form"))
                {
                    value = Request.Form["pageNumber"];
                }
                if (string.IsNullOrEmpty(value))
                {
                    return 1;
                }
                return Convert.ToInt32(value);
            }
        }
        public int PageSize
        {
            get
            {
                var value = Request.Query["pageSize"];
                if (string.IsNullOrEmpty(value) && Request.Method.ToLower() == "post" && Request.ContentType.Contains("form"))
                {
                    value = Request.Form["pageSize"];
                }
                if (string.IsNullOrEmpty(value))
                {
                    return 10;
                }
                return Convert.ToInt32(value);
            }
        }

        //public AdminController(IAccountService accountService)
        //{
        //    _accountService = accountService;
        //}

        /// <summary>
        /// 返回数据表格的json数据
        /// </summary>
        /// <param name="total"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public JsonNetResult GridData(int total, object data)
        {
            return this.JsonNet(new { total, rows = data });
        }

        /// <summary>
        /// 返回404页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PageNotFound()
        {
            return RedirectToAction("Page404", "Static");
        }

        #region 前端提示信息

        protected JavaScriptResult SuccessTip(string text, string redirect = "", string callback = "null")
        {
            return new JavaScriptResult() { Scripts = $"hos.ui.util.successTip('{text}','{redirect}',{callback});" };
        }

        protected JavaScriptResult DangerTip(string text, string redirect = "", string callback = "null")
        {
            return new JavaScriptResult() { Scripts = $"hos.ui.util.errorTip('{text}','{redirect}',{callback});" };
        }

        protected JavaScriptResult WarningTip(string text, string redirect = "", string callback = "null")
        {
            return new JavaScriptResult() { Scripts = $"hos.ui.util.warningTip('{text}','{redirect}',{callback});" };
        }

        #endregion

        /// <summary>
        /// 当前登陆的管理员
        /// </summary>
        public SystemUserEntity CurrentAdmin
        {
            get
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return null;
                }
                SystemUserEntity admin = _accountService.GetUserById(Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
                return admin;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //增加登录token的有效时间
            //HttpCookie token = Request.Cookies[tokenName];
            //if (token != null)
            //{
            //    var cookieToken = new HttpCookie(tokenName, token.Value)
            //    {
            //        Expires = DateTime.Now.AddDays(1),
            //        Secure = FormsAuthentication.RequireSSL,
            //        Domain = FormsAuthentication.CookieDomain,
            //        Path = FormsAuthentication.FormsCookiePath,
            //        HttpOnly = true
            //    };
            //    Response.Cookies.Remove(cookieToken.Name);
            //    Response.Cookies.Add(cookieToken);
            //}
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// 登录状态检查
        /// </summary>
        /// <param name="filterContext"></param>
        /// 
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var anonymousAction = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            if (!anonymousAction.Any())
            {
                var user = CurrentAdmin;
                if (user == null)
                {
                    if (context.HttpContext.Request.IsAjaxRequest())
                    {
                        var accept = context.HttpContext.Request.Headers["accept"];
                        if (accept.Contains("application/json"))
                        {
                            context.Result = new JsonNetResult()
                            {
                                Data = new { Success = false, Message = "登录已超时！" }
                            };
                        }
                        else
                        {
                            context.Result = new JavaScriptResult("alert('登录已超时！')");
                        }
                    }
                    else
                    {
                        context.Result = new RedirectResult("/Login/Index");
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}