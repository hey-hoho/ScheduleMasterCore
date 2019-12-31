using System;
using System.Linq;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Repository;
using Hos.ScheduleMaster.Web.Extension;
using Hos.ScheduleMaster.Web.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            //services.AddControllersWithViews();
            //services.AddControllers();
            services.AddHosControllers(this);
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            }).AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.PropertyNamingPolicy = null;
                option.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });
            //    .AddNewtonsoftJson(option =>
            //{
            //    ////忽略循环引用
            //    option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //    //不使用驼峰样式的key
            //    option.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //    //设置时间格式
            //    option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //});
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
            //EF数据库上下文
            services.AddDbContext<SmDbContext>(option => option.UseMySql(Configuration.GetConnectionString("MysqlConnection")));

            //注入Uow依赖
            services.AddScoped<IUnitOfWork, UnitOfWork<SmDbContext>>();
            //自动注册所有业务service
            services.AddAppServices();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
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
            //加载全局缓存
            ConfigurationCache.RootServiceProvider = app.ApplicationServices;
            ConfigurationCache.SetNode(Configuration.GetSection("NodeSetting").Get<NodeSetting>());
            ConfigurationCache.Reload();
            //初始化日志管理器
            Core.Log.LogManager.Init();
            //注册节点
            AppStart.NodeRegistry.Register();
            //初始化系统任务
            FluentScheduler.JobManager.Initialize(new AppStart.SystemSchedulerRegistry());
            FluentScheduler.JobManager.JobException += info => Core.Log.LogHelper.Error("An error just happened with a FluentScheduler job", info.Exception);
            //任务恢复
            using (var scope = app.ApplicationServices.CreateScope())
            {
                Core.Services.ScheduleService service = new Core.Services.ScheduleService();
                AutowiredServiceProvider provider = new AutowiredServiceProvider();
                provider.PropertyActivate(service, scope.ServiceProvider);
                service.RunningRecovery();
            }
            appLifetime.ApplicationStopping.Register(OnStopping);
        }
        private void OnStopping()
        {
            // Perform on-stopping activities here
            Core.Log.LogManager.Shutdown();
            AppStart.NodeRegistry.Shutdown();
        }

    }
}
