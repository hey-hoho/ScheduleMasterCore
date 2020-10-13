using System;
using System.Linq;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Repository;
using Hos.ScheduleMaster.Core.Services.RemoteCaller;
using Hos.ScheduleMaster.Web.Extension;
using Hos.ScheduleMaster.Web.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddHosControllers(this);
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
                options.Conventions.Add(new ApiControllerAuthorizeConvention());
            }).AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.PropertyNamingPolicy = null;
                option.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });

            services.AddHttpClient("workercaller", options => { options.Timeout = TimeSpan.FromSeconds(30); });

            //配置authorrize
            services.AddAuthentication(b =>
            {
                b.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                b.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                b.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(b =>
            {
                b.LoginPath = "/login";
                b.Cookie.Name = "smc_auth_name";
                b.Cookie.Path = "/";
                b.Cookie.HttpOnly = true;
                b.ExpireTimeSpan = TimeSpan.FromHours(5);
            });

            services.AddScheduleMasterDb(Configuration);

            //注入Uow依赖
            services.AddScoped<IUnitOfWork, UnitOfWork<SmDbContext>>();
            //自动注册所有业务service
            services.AddAppServices();
            services.AddScoped<AccessControlFilter>();

            services.AddHostedService<AppStart.AppLifetimeHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCookiePolicy();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Console}/{action=Index}/{id?}");
            });

            ConfigurationCache.RootServiceProvider = app.ApplicationServices;
        }


    }
    public class ApiControllerAuthorizeConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (controller.Filters.Any(x => x is ApiControllerAttribute) && !controller.Filters.Any(x => x is AccessControlFilter))
                {
                    controller.Filters.Add(new ServiceFilterAttribute(typeof(AccessControlFilter)));
                }
            }
        }
    }
    //public class ApiControllerAuthorizeConvention : IControllerModelConvention
    //{
    //    public void Apply(ControllerModel controller)
    //    {
    //        if (controller.Filters.Any(x => x is ApiControllerAttribute))
    //        {
    //            controller.Filters.Add(new ServiceFilterAttribute(typeof(AccessControlAttribute)));
    //        }
    //    }
    //}
}
