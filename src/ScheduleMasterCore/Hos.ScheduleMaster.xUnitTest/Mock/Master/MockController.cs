using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.Core.Repository;
using Hos.ScheduleMaster.Web.Controllers;
using Hos.ScheduleMaster.Web.Extension;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Hos.ScheduleMaster.xUnitTest.Mock.Master
{
    
    public class MockController
    {
        public static T CreateMvcController<T>() where T : AdminController, new()
        {
            var _controller = new T();

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();

            var claims = new List<Claim>()
                {
                 new Claim(ClaimTypes.Name, "admin"),
                 new Claim(ClaimTypes.NameIdentifier, "1")
                };
            var indentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var user = new ClaimsPrincipal(indentity);

            mockHttpContext.Setup(s => s.User).Returns(user);
            mockHttpContext.Setup(x => x.Request).Returns(new DefaultHttpContext().Request);


            IServiceCollection services = new ServiceCollection();
            //EF数据库上下文
            services.AddDbContext<SmDbContext>(option => option.UseMySql("Data Source=192.168.8.27;Database=schedule_master;User ID=root;Password=123456;pooling=true;CharSet=utf8;port=3306;sslmode=none;TreatTinyAsBoolean=true"));
            //注入Uow依赖
            services.AddScoped<IUnitOfWork, UnitOfWork<SmDbContext>>();
            services.AddAppServices();

            IServiceProvider provider = services.BuildServiceProvider();
            //Mock<IServiceProvider> mockService = new Mock<IServiceProvider>();
            //mockService.Setup(x => x.GetService<IServiceProvider>()).Returns(provider);

            PropertyActivate(_controller, provider);

            _controller.ControllerContext.HttpContext = mockHttpContext.Object;

            return _controller;
        }

        private static void PropertyActivate(object service, IServiceProvider provider)
        {
            var serviceType = service.GetType();
            var properties = serviceType.GetProperties().AsEnumerable().Where(x => x.Name.StartsWith("_"));
            foreach (PropertyInfo property in properties)
            {
                var autowiredAttr = property.GetCustomAttribute<AutowiredAttribute>();
                if (autowiredAttr != null)
                {
                    //从DI容器获取实例
                    var innerService = provider.GetService(property.PropertyType);
                    if (innerService != null)
                    {
                        //递归解决服务嵌套问题
                        PropertyActivate(innerService, provider);
                        //属性赋值
                        property.SetValue(service, innerService);
                    }
                }
            }
        }
    }
}
