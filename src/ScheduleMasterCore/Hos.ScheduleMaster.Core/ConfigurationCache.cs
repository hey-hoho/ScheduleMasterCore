using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Hos.ScheduleMaster.Core
{
    public static class ConfigurationCache
    {
        public const string PluginPathPrefix = "\\wwwroot\\plugins";

        public static NodeSetting NodeSetting { get; private set; }

        public static IServiceProvider RootServiceProvider { get; set; }

        private static ConcurrentDictionary<string, string> _cacheContainer;

        static ConfigurationCache()
        {
            //初始分配100个容量，避免配置项多了以后频繁扩容，100基本够用了
            //并发级别为1，表示仅允许1个线程同时更新
            _cacheContainer = new ConcurrentDictionary<string, string>(1, 100);
        }

        /// <summary>
        /// 配置节点信息
        /// </summary>
        /// <param name="configuration"></param>
        public static void SetNode(IConfiguration configuration)
        {
            NodeSetting = configuration.GetSection("NodeSetting").Get<NodeSetting>(); ;
            var ev = Environment.GetEnvironmentVariables();
            if (ev.Contains("identity"))
            {
                NodeSetting.IdentityName = ev["identity"].ToString();
            }
            if (ev.Contains("protocol"))
            {
                NodeSetting.Protocol = ev["Protocol"].ToString();
            }
            if (ev.Contains("ip"))
            {
                NodeSetting.IP = ev["ip"].ToString();
            }
            if (ev.Contains("port"))
            {
                NodeSetting.Port = Convert.ToInt32(ev["port"].ToString());
            }
            if (ev.Contains("priority"))
            {
                NodeSetting.Priority = Convert.ToInt32(ev["Priority"].ToString());
            }
        }

        public static void Reload()
        {
            using (var scope = new ScopeDbContext())
            {
                var configList = scope.GetDbContext().SystemConfigs.ToList();
                foreach (var item in configList)
                {
                    _cacheContainer[item.Key] = item.Value;
                }
            }
        }

        public static T GetField<T>(string key)
        {
            if (_cacheContainer == null)
            {
                Reload();
            }
            if (!_cacheContainer.ContainsKey(key))
            {
                return default(T);
            }
            string value = _cacheContainer[key];
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

    public class NodeSetting
    {
        public string IdentityName { get; set; }

        public string Role { get; set; }

        public string Protocol { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public int Priority { get; set; }
    }
}
