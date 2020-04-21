using Hos.ScheduleMaster.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Demo
{
    /// <summary>
    /// 演示如何在业务中使用DI
    /// </summary>
    public class TaskUseDI : TaskBase
    {
        IServiceProvider serviceProvider;

        public override void Initialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ITemplateService1, TemplateService1>();
            serviceCollection.AddTransient<ITemplateService2, TemplateService2>();
            serviceProvider = serviceCollection.BuildServiceProvider();

            base.Initialize();
        }

        public override void Run(TaskContext context)
        {
            context.WriteLog($"我是使用DI获取的结果：{new TaskUseDITest(serviceProvider.GetService<ITemplateService1>()).GetResult()}");
        }
    }

    public class TaskUseDITest
    {
        ITemplateService1 _service;

        public TaskUseDITest(ITemplateService1 service)
        {
            _service = service;
        }

        public string GetResult()
        {
            return _service.GetName();
        }
    }

    public interface ITemplateService1
    {
        string GetName();
    }

    public class TemplateService1 : ITemplateService1
    {
        ITemplateService2 _service2;
        public TemplateService1(ITemplateService2 service2)
        {
            _service2 = service2;
        }

        public string GetName()
        {
            return "I am TemplateService1   /   " + _service2.GetName();
        }
    }

    public interface ITemplateService2
    {
        string GetName();
    }

    public class TemplateService2 : ITemplateService2
    {
        public string GetName()
        {
            return "I am TemplateService2";
        }
    }
}
