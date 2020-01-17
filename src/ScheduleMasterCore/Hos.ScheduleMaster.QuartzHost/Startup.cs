using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.QuartzHost.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hos.ScheduleMaster.QuartzHost
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
            services.AddControllersWithViews(config =>
            {
                //添加全局过滤器
                config.Filters.Add(typeof(ApiValidationFilter));
                config.Filters.Add(typeof(GlobalExceptionFilter));
            });
            services.AddDbContextPool<SmDbContext>(option => option.UseMySql(Configuration.GetConnectionString("MysqlConnection")));
            services.AddTransient<Core.Interface.IScheduleService, Core.Services.ScheduleService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            var s = Environment.MachineName;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigurationCache.RootServiceProvider = app.ApplicationServices;
            //加载全局缓存
            ConfigurationCache.SetNode(InitNodeSetting());
            ConfigurationCache.Reload();
            //初始化日志管理器
            Core.Log.LogManager.Init();
            //初始化Quartz
            Common.QuartzManager.InitScheduler().Wait();
            //启动系统任务
            Common.QuartzManager.Start<AppStart.TaskClearJob>("task-clear", "0 0/1 * * * ? *").Wait();

            appLifetime.ApplicationStopping.Register(OnStopping);
        }

        private NodeSetting InitNodeSetting()
        {
            NodeSetting node = Configuration.GetSection("NodeSetting").Get<NodeSetting>();
            var ev = Environment.GetEnvironmentVariables();
            if (ev.Contains("IdentityName"))
            {
                node.IdentityName = ev["IdentityName"].ToString();
            }
            if (ev.Contains("Protocol"))
            {
                node.Protocol = ev["Protocol"].ToString();
            }
            if (ev.Contains("IP"))
            {
                node.IP = ev["IP"].ToString();
            }
            if (ev.Contains("Port"))
            {
                node.Port = Convert.ToInt32(ev["Port"].ToString());
            }
            if (ev.Contains("Priority"))
            {
                node.Priority = Convert.ToInt32(ev["Priority"].ToString());
            }
            return node;
        }

        private void OnStopping()
        {
            // Perform on-stopping activities here
            Common.QuartzManager.Shutdown(true).Wait();
            Core.Log.LogManager.Shutdown();
        }
    }
}
