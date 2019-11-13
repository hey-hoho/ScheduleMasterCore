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
                    Group = "Email",
                    Remark = remark,
                    Sort = 1
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Email_SmtpPort",
                    Name = "邮件服务器端口",
                    Value = "",
                    Group = "Email",
                    Remark = remark,
                    Sort = 2
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Email_FromAccount",
                    Name = "发件人账号",
                    Value = "",
                    Group = "Email",
                    Remark = remark,
                    Sort = 3
                },
                new SystemConfigEntity()
                {
                    CreateTime = DateTime.Now,
                    IsReuired = true,
                    Key = "Email_FromAccountPwd",
                    Name = "发件人账号密码",
                    Value = "",
                    Group = "Email",
                    Remark = remark,
                    Sort = 4
                }
                );
            return builder;
        }
    }
}
