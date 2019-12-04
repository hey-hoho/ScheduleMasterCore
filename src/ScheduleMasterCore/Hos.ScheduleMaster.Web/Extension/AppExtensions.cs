using Hos.ScheduleMaster.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.Extension
{
    public static class AppExtensions
    {
        /// <summary>
        /// 迁移数据库
        /// </summary>
        /// <param name="webhost"></param>
        /// <returns></returns>
        public static IHost Migrate(this IHost webhost)
        {
            using (var scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<Core.Models.SmDbContext>())
                {
                    //不需要先执行Add-migration迁移命令，如果数据库不存在，则自动创建并返回true
                    var c = dbContext.Database.EnsureCreated();
                    //dbContext.GetInfrastructure().GetService<IMigrator>().Migrate();
                    //检测是否有待迁移内容，有的话，自动应用迁移
                    //IEnumerable<string> migrations = dbContext.Database.GetPendingMigrations();
                    //if (migrations.Any())
                    //{
                    //    dbContext.Database.Migrate();
                    //}
                }
            }
            return webhost;
        }

        /// <summary>
        /// 自定义控制器激活，并手动注册所有控制器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="obj"></param>
        public static void AddHosControllers(this IServiceCollection services, object obj)
        {
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, HosControllerActivator>());
            var assembly = obj.GetType().GetTypeInfo().Assembly;
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(assembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);
            feature.Controllers.Select(ti => ti.AsType()).ToList().ForEach(t =>
            {
                services.AddTransient(t);
            });
        }

        /// <summary>
        /// 注册应用中的业务service
        /// </summary>
        /// <param name="services"></param>
        public static void AddAppServices(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains("Hos.ScheduleMaster.Core"));
            if (assembly == null) return;
            foreach (var type in assembly.GetTypes())
            {
                var serviceAttribute = type.GetCustomAttribute<ServiceMapToAttribute>();

                if (serviceAttribute != null)
                {
                    switch (serviceAttribute.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(serviceAttribute.ServiceType, type);
                            break;
                        case ServiceLifetime.Scoped:
                            services.AddScoped(serviceAttribute.ServiceType, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(serviceAttribute.ServiceType, type);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否异步请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            bool isAjax = false;
            var xreq = request.Headers.ContainsKey("x-requested-with");
            if (xreq)
            {
                isAjax = request.Headers["x-requested-with"] == "XMLHttpRequest";
            }
            return isAjax;
        }
    }
}
