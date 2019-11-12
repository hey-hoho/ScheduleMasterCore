using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceMapToAttribute : Attribute
    {
        /// <summary>
        /// 生命周期，默认为Transient
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// 要映射的类型
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// 通过构造函数要求必须指定类型
        /// </summary>
        /// <param name="serviceType"></param>
        public ServiceMapToAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }
    }
}
