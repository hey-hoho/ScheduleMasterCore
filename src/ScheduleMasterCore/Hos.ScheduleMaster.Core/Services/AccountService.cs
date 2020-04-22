using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hos.ScheduleMaster.Core.Services
{
    [ServiceMapTo(typeof(IAccountService))]
    public class AccountService : BaseService, IAccountService
    {
        /// <summary>
        /// 用户名和密码登录判断
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SystemUserEntity LoginCheck(string userName, string password)
        {
            string encodePwd = SecurityHelper.MD5(password);
            var user = _repositoryFactory.SystemUsers.FirstOrDefault(x => x.UserName == userName && x.Password == encodePwd && x.Status != (int)SystemUserStatus.Deleted);
            if (user == null || user.Status != (int)SystemUserStatus.Available)
            {
                return null;
            }
            //更新登录时间
            user.LastLoginTime = DateTime.Now;
            _repositoryFactory.SystemUsers.Update(user, "LastLoginTime");
            _unitOfWork.Commit();
            return user;
        }

        /// <summary>
        /// 根据id查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SystemUserEntity GetUserById(int id)
        {
            return _repositoryFactory.SystemUsers.FirstOrDefault(x => x.Id == id && x.Status != (int)SystemUserStatus.Deleted);
        }

        /// <summary>
        /// 使用用户名查找用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SystemUserEntity GetUserbyUserName(string name)
        {
            return _repositoryFactory.SystemUsers.FirstOrDefault(x => x.UserName == name && x.Status != (int)SystemUserStatus.Deleted);
        }

        /// <summary>
        /// 查询用户分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<SystemUserEntity> GetUserPager(ListPager<SystemUserEntity> pager)
        {
            return _repositoryFactory.SystemUsers.WherePager(pager, m => m.Status != (int)SystemUserStatus.Deleted, m => m.CreateTime, false);
        }

        /// <summary>
        /// 查询所有未删除的用户
        /// </summary>
        /// <returns></returns>
        public List<SystemUserEntity> GetUserAll()
        {
            return _repositoryFactory.SystemUsers.WhereOrderBy(m => m.Status != (int)SystemUserStatus.Deleted, m => m.CreateTime, false).ToList();
        }

        /// <summary>
        /// 判断用户名时候已存在
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckUserName(string userName, int id = 0)
        {
            if (id > 0)
            {
                var model = GetUserById(id);
                if (model != null && model.UserName.ToLower() == userName.ToLower())
                {
                    return true;
                }
            }
            return _repositoryFactory.SystemUsers.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower()) == null;
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUser(SystemUserEntity model)
        {
            model.Password = SecurityHelper.MD5(model.Password);
            model.Status = (int)SystemUserStatus.Available;
            model.CreateTime = DateTime.Now;
            _repositoryFactory.SystemUsers.Add(model);
            return _unitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool EditUser(SystemUserEntity model)
        {
            _repositoryFactory.SystemUsers.UpdateBy(x => x.Id == model.Id, new
            {
                model.Email,
                model.Phone,
                model.RealName,
                //model.UserName
            });
            return _unitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateUserStatus(int id, int status)
        {
            _repositoryFactory.SystemUsers.UpdateBy(x => x.Id == id, new
            {
                Status = status
            });
            return _unitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UpdateUserPassword(int id, string password)
        {
            password = SecurityHelper.MD5(password);
            _repositoryFactory.SystemUsers.UpdateBy(x => x.Id == id, new
            {
                Password = password
            });
            _unitOfWork.Commit();
            return true;
        }

        /// <summary>
        /// 查询指定用户状态数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int QueryUserCount(int? status)
        {
            var query = _repositoryFactory.SystemUsers.Where(x => x.Status != (int)SystemUserStatus.Deleted);
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            return query.Count();
        }
    }
}
