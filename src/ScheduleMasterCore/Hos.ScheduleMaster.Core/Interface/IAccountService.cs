using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Core.Interface
{
    public interface IAccountService
    {
        /// <summary>
        /// 账号登入
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SystemUserEntity LoginCheck(string userName, string password);

        /// <summary>
        /// 根据id查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SystemUserEntity GetUserById(int id);

        /// <summary>
        /// 查询用户分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        ListPager<SystemUserEntity> GetUserPager(ListPager<SystemUserEntity> pager);

        List<SystemUserEntity> GetUserAll();

        /// <summary>
        /// 判断用户名时候已存在
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckUserName(string userName, int id = 0);

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddUser(SystemUserEntity model);

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool EditUser(SystemUserEntity model);

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool UpdateUserStatus(int id, int status);

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool UpdateUserPassword(int id, string password);
    }
}
