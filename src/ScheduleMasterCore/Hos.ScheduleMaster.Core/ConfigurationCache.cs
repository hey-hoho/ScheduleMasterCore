using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Hos.ScheduleMaster.Core
{
    public static class ConfigurationCache
    {
        public const string PluginPathPrefix = "\\wwwroot\\plugins";

        public static DbConnector DbConnector { get; set; }

        public static NodeSetting NodeSetting { get; private set; }

        public static IServiceProvider RootServiceProvider { get; set; }

        private static ConcurrentDictionary<string, string> _cacheContainer;

        public static ConcurrentDictionary<string, int> WorkerUnHealthCounter;

        static ConfigurationCache()
        {
            //初始分配100个容量，避免配置项多了以后频繁扩容，100基本够用了
            //并发级别为1，表示仅允许1个线程同时更新
            _cacheContainer = new ConcurrentDictionary<string, string>(1, 100);
            WorkerUnHealthCounter = new ConcurrentDictionary<string, int>(1, 16);
        }

        /// <summary>
        /// 配置节点信息
        /// </summary>
        /// <param name="configuration"></param>
        public static void SetNode(IConfiguration configuration)
        {
            NodeSetting = configuration.GetSection("NodeSetting").Get<NodeSetting>(); 

            string identity = AppCommandResolver.GetCommandLineArgsValue("identity");
            if (!string.IsNullOrEmpty(identity))
            {
                NodeSetting.IdentityName = identity;
            }
            string protocol = AppCommandResolver.GetCommandLineArgsValue("protocol");
            if (!string.IsNullOrEmpty(protocol))
            {
                NodeSetting.Protocol = protocol;
            }
            string ip = AppCommandResolver.GetCommandLineArgsValue("ip");
            if (!string.IsNullOrEmpty(ip))
            {
                NodeSetting.IP = ip;
            }
            string port = AppCommandResolver.GetCommandLineArgsValue("port");
            if (!string.IsNullOrEmpty(port))
            {
                NodeSetting.Port = Convert.ToInt32(port);
            }
            string priority = AppCommandResolver.GetCommandLineArgsValue("priority");
            if (!string.IsNullOrEmpty(priority))
            {
                NodeSetting.Priority = Convert.ToInt32(priority);
            }
            string maxConcurrency = AppCommandResolver.GetCommandLineArgsValue("maxc");
            if (!string.IsNullOrEmpty(maxConcurrency))
            {
                NodeSetting.MaxConcurrency = Convert.ToInt32(maxConcurrency);
            }
            NodeSetting.MachineName = Environment.MachineName;
        }

        /// <summary>
        /// 配置节点信息
        /// </summary>
        /// <param name="model"></param>
        public static void SetNode(Models.ServerNodeEntity model)
        {
            if (model == null) return;
            NodeSetting = new NodeSetting();
            NodeSetting.IdentityName = model.NodeName;
            NodeSetting.Role = model.NodeType;
            NodeSetting.Protocol = model.AccessProtocol;
            NodeSetting.IP = model.Host.Split(':')[0];
            NodeSetting.Port = Convert.ToInt32(model.Host.Split(':')[1]);
            NodeSetting.Priority = model.Priority;
            NodeSetting.MachineName = model.MachineName;
            NodeSetting.MaxConcurrency = model.MaxConcurrency;
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
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
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

    public class AppCommandResolver
    {
        private static List<string> CommandLineArgs = Environment.GetCommandLineArgs().ToList();

        /// <summary>
        /// 判断应用是否自动注册节点信息
        /// </summary>
        /// <returns></returns>
        public static bool IsAutoRegister()
        {
            //优先读取环境参数
            string option = Environment.GetEnvironmentVariable("SMCORE_AUTOR");
            //再看命令行参数中是否也有设置
            string cmdArg = GetCommandLineArgsValue("autor");
            if (!string.IsNullOrEmpty(cmdArg))
            {
                option = cmdArg;
            }
            return option != "false";
        }

        /// <summary>
        /// 非自动注册模式下要绑定的master名称
        /// </summary>
        /// <returns></returns>
        public static string GetTargetMasterName()
        {
            //优先读取环境参数
            string option = Environment.GetEnvironmentVariable("SMCORE_WORKEROF");
            //再看命令行参数中是否也有设置
            string cmdArg = GetCommandLineArgsValue("workerof");
            if (!string.IsNullOrEmpty(cmdArg))
            {
                option = cmdArg;
            }
            return option;
        }

        /// <summary>
        /// 获取命令行参数值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCommandLineArgsValue(string key)
        {
            string cmdArg = CommandLineArgs.FirstOrDefault(x => x.StartsWith($"--{key}="));
            if (!string.IsNullOrEmpty(cmdArg))
            {
                return cmdArg.Split('=')[1];
            }
            return string.Empty;
        }
    }

    public class NodeSetting
    {
        public string IdentityName { get; set; }

        public string Role { get; set; }

        public string Protocol { get; set; }

        public string IP { get; set; }

        private string _machineName;
        public string MachineName
        {
            get { return _machineName; }
            set { if (string.IsNullOrEmpty(value)) { _machineName = Environment.MachineName; } else { _machineName = value; } }
        }

        public int Port { get; set; }

        public int Priority { get; set; }

        public int MaxConcurrency { get; set; }
    }

    public class DbConnector
    {
        public DbProvider Provider { get; set; }

        public string ConnectionString { get; set; }
    }

    public enum DbProvider
    {
        MySQL,
        SQLServer,
        PostgreSQL
    }
}
