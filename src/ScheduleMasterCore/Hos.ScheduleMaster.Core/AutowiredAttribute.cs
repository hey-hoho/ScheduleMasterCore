using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
//https://github.com/aspnet/Extensions/tree/master/src/DependencyInjection


namespace Hos.ScheduleMaster.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutowiredAttribute : Attribute
    {

    }

    public class AutowiredServiceProvider
    {
        //static Dictionary<Type, Action<object, IServiceProvider>> autowiredActions = new Dictionary<Type, Action<object, IServiceProvider>>();

        //public IServiceProvider ServiceProvider;

        public void PropertyActivate(object service, IServiceProvider provider)
        {
            //var serviceProvider = ServiceProvider;// ConfigurationCache.ServiceProvider;
            var serviceType = service.GetType();
            //HttpContext
            //字段赋值
            //foreach (FieldInfo field in serviceType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            //{
            //    var autowiredAttr = field.GetCustomAttribute<AutowiredAttribute>();
            //    if (autowiredAttr != null)
            //    {
            //        field.SetValue(service, serviceProvider.GetService(field.FieldType));
            //    }
            //}
            //属性赋值
            var properties = serviceType.GetProperties().AsEnumerable().Where(x => x.Name.StartsWith("_"));
            foreach (PropertyInfo property in properties)
            {
                var autowiredAttr = property.GetCustomAttribute<AutowiredAttribute>();
                if (autowiredAttr != null)
                {
                    var innerService = provider.GetService(property.PropertyType);
                    PropertyActivate(innerService, provider);
                    property.SetValue(service, innerService);
                }
            }
            return;


            //if (autowiredActions.TryGetValue(serviceType, out Action<object, IServiceProvider> act))
            //{
            //    act(service, serviceProvider);
            //}
            //else
            //{
            //    //参数
            //    var objParam = Expression.Parameter(typeof(object), "obj");
            //    var spParam = Expression.Parameter(typeof(IServiceProvider), "sp");

            //    var obj = Expression.Convert(objParam, serviceType);
            //    var GetService = typeof(IServiceProvider).GetMethod("GetService");
            //    List<Expression> setList = new List<Expression>();

            //    //字段赋值
            //    foreach (FieldInfo field in serviceType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            //    {
            //        var autowiredAttr = field.GetCustomAttribute<AutowiredAttribute>();
            //        if (autowiredAttr != null)
            //        {
            //            var fieldExp = Expression.Field(obj, field);
            //            var createService = Expression.Call(spParam, GetService, Expression.Constant(field.FieldType));
            //            var setExp = Expression.Assign(fieldExp, Expression.Convert(createService, field.FieldType));
            //            setList.Add(setExp);
            //        }
            //    }
            //    //属性赋值
            //    foreach (PropertyInfo property in serviceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            //    {
            //        var autowiredAttr = property.GetCustomAttribute<AutowiredAttribute>();
            //        if (autowiredAttr != null)
            //        {
            //            var propExp = Expression.Property(obj, property);
            //            var createService = Expression.Call(spParam, GetService, Expression.Constant(property.PropertyType));
            //            var setExp = Expression.Assign(propExp, Expression.Convert(createService, property.PropertyType));
            //            setList.Add(setExp);
            //        }
            //    }
            //    var bodyExp = Expression.Block(setList);
            //    var setAction = Expression.Lambda<Action<object, IServiceProvider>>(bodyExp, objParam, spParam).Compile();
            //    autowiredActions[serviceType] = setAction;
            //    setAction(service, serviceProvider);
        }

    }
}

