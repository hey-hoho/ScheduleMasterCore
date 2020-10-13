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
            services.AddHealthChecks();
            services.AddHttpClient();

            services.AddScheduleMasterDb(Configuration);
            services.AddTransient<HosLock.IHosLock, HosLock.DatabaseLock>();
            services.AddTransient<Core.Interface.IScheduleService, Core.Services.ScheduleService>();
            services.AddScoped<Common.RunTracer>();

            services.AddHostedService<AppStart.AppLifetimeHostedService>();
            services.AddHostedService<AppStart.ConfigurationRefreshService>();
            services.AddHostedService<AppStart.DelayedTaskConsumerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                endpoints.MapHealthChecks("/health");
            });

            ConfigurationCache.RootServiceProvider = app.ApplicationServices;

        }

    }
}
