using Hos.ScheduleMaster.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Demo
{
    /// <summary>
    /// 演示如何终止一个长任务
    /// </summary>
    public class TaskCancel : TaskBase
    {
        public override void Run(TaskContext context)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        context.WriteLog("[IsCancellationRequested: true]我要终止运行运行了~");
                        break;
                    }
                    System.Diagnostics.Debug.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()} : {Guid.NewGuid().ToString()}");
                }
            });
        }
    }
}
