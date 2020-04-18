using System;
using Hos.ScheduleMaster.Base;

namespace Hos.ScheduleMaster.Demo
{
    /// <summary>
    /// 一个简单的使用示例：在日志中记录当前执行时间
    /// </summary>
    public class Simple : TaskBase
    {
        public override void Run(TaskContext context)
        {
            context.WriteLog($"当前时间是：{DateTime.Now}");
        }
    }
}
