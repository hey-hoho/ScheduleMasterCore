using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core
{
    public static class ConfigurationCache
    {
        private static Dictionary<string, string> _cacheInstance;

        static ConfigurationCache()
        {
            _cacheInstance = new Dictionary<string, string>();
        }

        public static void Refresh()
        {
            //var configList = new Services.SystemService().GetConfigList();
            //foreach (var item in configList)
            //{
            //    _cacheInstance[item.Key] = item.Value;
            //}
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
