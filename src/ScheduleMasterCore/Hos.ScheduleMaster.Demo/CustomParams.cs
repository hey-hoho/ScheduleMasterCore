using Hos.ScheduleMaster.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Demo
{
    /// <summary>
    /// 演示如何读取自定义参数
    /// </summary>
    public class CustomParams : TaskBase
    {
        public override void Run(TaskContext context)
        {
            context.WriteLog($"我获取了参数信息-first：{context.GetArgument<int>("first")}");
            context.WriteLog($"我获取了参数信息-second：{context.GetArgument<int>("second")}");
            context.WriteLog($"我获取了参数信息-xxoo：{context.GetArgument<string>("xxoo")}");
        }
    }
}
