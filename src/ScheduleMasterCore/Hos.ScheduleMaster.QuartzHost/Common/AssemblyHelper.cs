using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
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

        public static TaskBase CreateTaskInstance(PluginLoadContext context, Guid sid, string assemblyName, string className)
        {
            try
            {
                string pluginLocation = GetTaskAssemblyPath(sid, assemblyName);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
                Type type = assembly.GetType(className, true, true);
                return Activator.CreateInstance(type) as TaskBase;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetTaskAssemblyPath(Guid sid, string assemblyName)
        {
            return $"{ConfigurationCache.PluginPathPrefix}\\{sid}\\{assemblyName}.dll".ToPhysicalPath();
        }

        /// <summary>
        /// 加载应用程序域
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static PluginLoadContext LoadAssemblyContext(Guid sid, string assemblyName)
        {
            string pluginLocation = GetTaskAssemblyPath(sid, assemblyName);
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext;
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