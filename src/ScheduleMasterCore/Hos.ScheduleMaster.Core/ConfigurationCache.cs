using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core
{
    public static class ConfigurationCache
    {
        public static IServiceProvider RootServiceProvider { get; set; }

        private static ConcurrentDictionary<string, string> _cacheInstance;

        static ConfigurationCache()
        {
            //初始分配1000个容量，避免配置项多了以后频繁扩容，1000基本够用了
            //并发级别为1，表示仅允许1个线程同时更新
            _cacheInstance = new ConcurrentDictionary<string, string>(1, 1000);
        }

        public static void Refresh()
        {
            using (var serviceScope = RootServiceProvider.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TaskDbContext>();
                var configList = context.SystemConfigs.ToList();
                foreach (var item in configList)
                {
                    _cacheInstance[item.Key] = item.Value;
                }
            }
        }

        public static T GetField<T>(string key)
        {
            if (_cacheInstance == null)
            {
                Refresh();
            }
            if (!_cacheInstance.ContainsKey(key))
            {
                return default(T);
            }
            string value = _cacheInstance[key];
            return (T)Convert.ChangeType(value, typeof(T));
        }

        #region 配置项key

        #region 邮件相关

        public const string Email_SmtpServer = "Email_SmtpServer";

        public const string Email_SmtpPort = "Email_SmtpPort";

        public const string Email_FromAccount = "Email_FromAccount";

        public const string Email_FromAccountPwd = "Email_FromAccountPwd";

        #endregion

        #endregion

    }
}
