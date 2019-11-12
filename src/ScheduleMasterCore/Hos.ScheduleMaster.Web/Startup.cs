using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql;

namespace Hos.ScheduleMaster.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddMvc(options =>
            {
                //options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(
                //    new System.Text.Json.JsonSerializerOptions
                //    {

                //    }
                //    ));
            }).AddJsonOptions(option =>
            {
                ////忽略循环引用
                //option.JsonSerializerOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                ////不使用驼峰样式的key
                //option.SerializerSettings.ContractResolver = new DefaultContractResolver();
                ////设置时间格式
                //option.SerializerSettings.DateFormatString = "yyyy-MM-dd";
            });

            //配置authorrize
            services.AddAuthentication(b =>
            {
                b.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                b.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                b.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(b =>
            {
                b.LoginPath = "/login";
                b.Cookie.Name = "msc_auth_name";
                b.Cookie.Path = "/";
                b.Cookie.HttpOnly = true;
                //b.Cookie.Expiration = new TimeSpan(2, 0, 0);
                b.ExpireTimeSpan = new TimeSpan(2, 0, 0);
            });
            services.AddTransient<Core.Repository.IUnitOfWork, TaskDbContext>();
            //EF数据库上下文
            services.AddDbContext<TaskDbContext>(option => option.UseMySql(Configuration.GetConnectionString("MysqlConnection")));


            services.AddAppServices();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Login}/{action=Index}/{id?}");
            });

            AutowiredServiceProvider.serviceProvider = app.ApplicationServices;
        }
    }

    public static class AppServiceExtensions
    {
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
    }

}
