using Hos.ScheduleMaster.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Demo
{
    /// <summary>
    /// 演示如何设置自己的配置文件，以及读取配置项
    /// </summary>
    public class CustomConfigFile : TaskBase
    {
        public override void Initialize()
        {
            //指定配置文件
            base.SetConfigurationFile("myconfig.json");
        }

        public override void Run(TaskContext context)
        {
            context.WriteLog($"我的配置TestKey1：{Configuration["TestKey1"]}");
            context.WriteLog($"我的配置TestKey2：name->{Configuration["TestKey2:Name"]}  age->{Configuration["TestKey2:Age"]}");
        }
    }
}
