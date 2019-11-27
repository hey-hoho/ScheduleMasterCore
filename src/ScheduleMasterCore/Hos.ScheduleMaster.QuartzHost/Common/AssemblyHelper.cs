using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Web;

namespace Hos.ScheduleMaster.QuartzHost.Common
{
    public class AssemblyHelper
    {
        public static Type GetClassType(string assemblyPath, string className)
        {
            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
                Type type = assembly.GetType(className, true, true);
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T CreateInstance<T>(Type type) where T : class
        {
            try
            {
                return Activator.CreateInstance(type) as T;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static TaskBase CreateTaskInstance(PluginLoadContext context, string assemblyName, string className)
        {
            try
            {
                string pluginLocation = GetTaskAssemblyPath(assemblyName);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
                Type type = assembly.GetType(className, true, true);
                return Activator.CreateInstance(type) as TaskBase;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetTaskAssemblyPath(string assemblyName)
        {
            return $"{Directory.GetCurrentDirectory()}\\Plugins\\{assemblyName}\\{assemblyName}.dll".Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// 加载应用程序域
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static PluginLoadContext LoadAssemblyContext(string assemblyName)
        {
            try
            {
                string pluginLocation = GetTaskAssemblyPath(assemblyName);
                PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
                return loadContext;

                //AppDomainSetup setup = new AppDomainSetup();
                //setup.ApplicationName = assemblyName;
                //setup.ApplicationBase = Path.GetDirectoryName(dllPath);
                //if (File.Exists(dllPath + ".config"))
                //{
                //    setup.ConfigurationFile = dllPath + ".config";
                //}
                ////setup.ShadowCopyFiles = "true"; //启用影像复制程序集
                ////setup.ShadowCopyDirectories = setup.ApplicationBase;
                ////AppDomain.CurrentDomain.SetShadowCopyFiles();
                //Assembly assembly = Assembly.Load(File.ReadAllBytes(dllPath));
                //AssemblyLoadContext context = new AssemblyLoadContext("");
                //AppDomain domain = AppDomain.CreateDomain(assemblyName, null, setup);
                ////AppDomain.MonitoringIsEnabled = true;
                //return domain;
            }
            catch (Exception exp)
            {
                LogHelper.Error($"加载应用程序域{assemblyName}失败！", exp);
                throw exp;
            }
        }

        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        /// <param name="context"></param>
        public static void UnLoadAssemblyLoadContext(PluginLoadContext context)
        {
            if (context != null)
            {
                context.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}