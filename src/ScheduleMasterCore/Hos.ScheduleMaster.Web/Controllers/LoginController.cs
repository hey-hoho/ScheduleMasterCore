using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hos.ScheduleMaster.Core.Interface;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Hos.ScheduleMaster.Web.Extension;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class LoginController : Controller
    {
        private const string CookieKey = "smc_cookie";

        public IAccountService _accountService;

        public LoginController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Console");
            }
            return View();
        }

        /// <summary>
        /// 登录请求处理
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> In(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return this.JavaScript("showTips('请输入用户名和密码！');");
            }
            var user = _accountService.LoginCheck(username, password);
            if (user != null)
            {
                //登陆授权
                var claims = new List<Claim>()
                {
                 new Claim(ClaimTypes.Name, user.UserName),
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var indentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(indentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //序列化admin对象
                //string accountJson = JsonConvert.SerializeObject(user);
                //创建用户票据
                //var ticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddDays(1), false, accountJson);
                //加密
                //string encryptAccount = FormsAuthentication.Encrypt(ticket);
                //创建cookie
                //var options = new CookieOptions()
                //{
                //    HttpOnly = true,
                //    Expires = DateTime.Now.AddDays(1),
                //    Secure = true,
                //};
                //写入Cookie
                //Response.Cookies.Append(CookieKey, accountJson, options);
                return this.JavaScript($"$('#btnLogin').val('successed！redirecting...');location.href='{Url.Action("Index", "Console")}';");
            }
            return this.JavaScript("showTips('用户名或密码错误！');");
        }

        /// <summary>
        /// 注销登录状态
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Out()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Response.Cookies.Delete(CookieKey);
            return RedirectToAction("Index");
        }
    }
}