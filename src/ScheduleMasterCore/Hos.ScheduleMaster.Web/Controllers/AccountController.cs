using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Web.Extension;
using Hos.ScheduleMaster.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hos.ScheduleMaster.Core.Interface;

namespace Hos.ScheduleMaster.Web.Controllers
{
    public class AccountController : AdminController
    {
        [Autowired]
        public IConfiguration _config { get; set; }

        //public AccountController(IConfiguration config)
        //{
        //    _config = config;
        //}

        /// <summary>
        /// 用户列表页 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 异步查询用户分页数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [AjaxRequestOnlyAttribute]
        public ActionResult LoadUserPager(string name)
        {
            var pager = new ListPager<SystemUserEntity>(PageIndex, PageSize);
            if (!string.IsNullOrEmpty(name))
            {
                pager.AddFilter(m => m.UserName.Contains(name) || m.RealName.Contains(name));
            }
            pager = _accountService.GetUserPager(pager);
            return GridData(pager.Total, pager.Rows.Select(x => new
            {
                CreateTime = x.CreateTime.StringFormat(),
                x.Id,
                LastLoginTime = x.LastLoginTime.StringFormat(),
                x.Phone,
                Status = EnumHelper.GetEnumDescription(typeof(SystemUserStatus), x.Status),
                StatusCode = x.Status,
                x.RealName,
                x.UserName
            }));
        }

        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            if (id != 0)
            {
                var user = _accountService.GetUserById(id);
                if (user == null)
                {
                    return PageNotFound();
                }
                return View(user);
            }
            return View();
        }

        /// <summary>
        /// 保存页面的编辑信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AjaxRequestOnlyAttribute]
        public ActionResult Edit(SystemUserEntity model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var result = _accountService.EditUser(model);
                    if (result)
                    {
                        return SuccessTip("编辑用户成功！", Url.Action("Index"));
                    }
                    return DangerTip("编辑用户失败！");
                }
                else
                {
                    if (!_accountService.CheckUserName(model.UserName, model.Id))
                    {
                        return DangerTip("用户名已存在！");
                    }
                    var result = _accountService.AddUser(model);
                    if (result)
                    {
                        return SuccessTip("创建用户成功！", Url.Action("Index"));
                    }
                    return DangerTip("创建用户失败！");
                }
            }
            return DangerTip("数据验证失败！");
        }

        /// <summary>
        /// 账号启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, AjaxRequestOnlyAttribute]
        public ActionResult UserEnable(int id)
        {
            var result = _accountService.UpdateUserStatus(id, (int)SystemUserStatus.Available);
            if (result)
            {
                return this.JsonNet(true, "启用成功！");
            }
            return this.JsonNet(false, "启用失败！");
        }

        /// <summary>
        /// 账号禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, AjaxRequestOnlyAttribute]
        public ActionResult UserDisable(int id)
        {
            var result = _accountService.UpdateUserStatus(id, (int)SystemUserStatus.Disabled);
            if (result)
            {
                return this.JsonNet(true, "禁用成功！");
            }
            return this.JsonNet(false, "禁用失败！");
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, AjaxRequestOnlyAttribute]
        public ActionResult UserDelete(int id)
        {
            var result = _accountService.UpdateUserStatus(id, (int)SystemUserStatus.Deleted);
            if (result)
            {
                return this.JsonNet(true, "删除成功！");
            }
            return this.JsonNet(false, "删除失败！");
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, AjaxRequestOnlyAttribute]
        public ActionResult UserResetPwd(int id)
        {
            string defaultPwd = _config.GetValue<string>("AppSettings:AdminDefaultPwd");
            var result = _accountService.UpdateUserPassword(id, defaultPwd);
            if (result)
            {
                return this.JsonNet(true, "重置成功！");
            }
            return this.JsonNet(false, "重置失败！");
        }
    }
}