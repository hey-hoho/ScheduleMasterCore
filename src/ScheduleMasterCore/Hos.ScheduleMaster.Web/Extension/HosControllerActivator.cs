using Hos.ScheduleMaster.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.Extension
{
    public class HosControllerActivator : IControllerActivator
    {
        public object Create(ControllerContext actionContext)
        {
            var controllerType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
            var instance = actionContext.HttpContext.RequestServices.GetRequiredService(controllerType);
            PropertyActivate(instance, actionContext.HttpContext.RequestServices);
            return instance;
        }

        public virtual void Release(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }
            if (controller is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void PropertyActivate(object service, IServiceProvider provider)
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
