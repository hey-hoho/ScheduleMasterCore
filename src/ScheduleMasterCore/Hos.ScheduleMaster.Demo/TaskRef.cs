using Hos.ScheduleMaster.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Hos.ScheduleMaster.Demo
{
    /// <summary>
    /// 演示如何使用任务依赖关系
    /// </summary>
    public class TaskRef : TaskBase
    {
        public override void Run(TaskContext context)
        {
            //前面可以做具体的业务并得到一个结果，传给后面的任务用
            //.................
            //do something
            //.................
            context.Result = new { success = true, message = "后面的兄弟大家好~" };
        }
    }

    public class TaskRefNext : TaskBase
    {
        public override void Run(TaskContext context)
        {
            context.WriteLog(System.Text.RegularExpressions.Regex.Unescape($"收到了前面的结果：{JsonSerializer.Serialize(context.PreviousResult)}"));
        }
    }
}
