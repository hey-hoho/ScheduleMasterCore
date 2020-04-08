using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Common;

namespace Hos.ScheduleMaster.Core.EntityFramework
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// 生成种子数据
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder SeedData(this ModelBuilder builder)
        {
            string remark = "seed by efcore auto migration";

            builder.Entity<SystemUserEntity>().HasData
                (
                new SystemUserEntity()
                {
                    Id = 1,
                    CreateTime = DateTime.Now,
                    Status = 1,
                    UserName = "admin",
                    RealName = "admin",
                    Password = SecurityHelper.MD5("111111")
                }
                );

            builder.Entity<SystemConfigEntity>().HasData
                (
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Email_SmtpServer",
                    Name = "邮件服务器",
                    Value = "",
                    Group = "邮件配置",
                    Remark = "smtp服务器地址",
                    Sort = 1
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Email_SmtpPort",
                    Name = "邮件服务器端口",
                    Value = "",
                    Group = "邮件配置",
                    Remark = "smtp端口号",
                    Sort = 2
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Email_FromAccount",
                    Name = "发件人账号",
                    Value = "",
                    Group = "邮件配置",
                    Remark = "邮箱账号",
                    Sort = 3
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Email_FromAccountPwd",
                    Name = "发件人账号密码",
                    Value = "",
                    Group = "邮件配置",
                    Remark = "登录密码或授权码等",
                    Sort = 4
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Assembly_ImagePullPolicy",
                    Name = "文件包拉取策略",
                    Value = "Always",
                    Group = "程序集配置",
                    Remark = "Always-总是拉取，IfNotPresent-本地没有时拉取",
                    Sort = 1
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Http_RequestTimeout",
                    Name = "请求超时时间",
                    Value = "10",
                    Group = "HTTP配置",
                    Remark = "单位是秒，默认值是10",
                    Sort = 1
                }
                );
            return builder;
        }
    }
}
