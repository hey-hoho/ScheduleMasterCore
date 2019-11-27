using System;
using Hos.ScheduleMaster.Base;

namespace Hos.ScheduleMaster.Demo
{
    public class Simple : TaskBase
    {
        public override void Run(TaskContext context)
        {
            context.WriteLog($"当前时间是：{DateTime.Now}");
        }
    }
}
