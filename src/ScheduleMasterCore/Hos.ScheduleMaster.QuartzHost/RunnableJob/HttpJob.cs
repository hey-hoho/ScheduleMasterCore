using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.QuartzHost.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.RunnableJob
{
    /// <summary>
    /// http任务的入口
    /// by hoho
    /// </summary>
    public class HttpJob : RootJob
    {
        /// <summary>
        /// 执行任务ing
        /// </summary>
        /// <param name="context"></param>
        public override void OnExecuting(TaskContext context)
        {
            context.InstanceRun();
        }

        /// <summary>
        /// 执行完成
        /// </summary>
        /// <param name="context"></param>
        public override void OnExecuted(TaskContext context)
        {

        }
    }

}
