using Hos.ScheduleMaster.Base.Dto;
using System;
using System.Collections.Generic;

namespace Hos.ScheduleMaster.Base
{
    /// <summary>
    /// 任务抽象基类，要加入的任务必须继承此类
    /// by hoho
    /// </summary>
    public abstract class TaskBase : MarshalByRefObject, IDisposable
    {
        internal Queue<ScheduleLog> logger = new Queue<ScheduleLog>();

        private bool _isRunning = false;

        /// <summary>
        /// 任务执行的方法，由具体任务去重写实现
        /// </summary>
        public abstract void Run(TaskContext context);

        /// <summary>
        /// 停止任务后可能需要的处理
        /// </summary>
        public virtual void Dispose()
        {
            ///TODO:
        }

        /// <summary>
        /// 保证前一次运行完才开始下一次，否则就跳过本次执行
        /// </summary>
        internal void InnerRun(TaskContext context)
        {
            if (!_isRunning)
            {
                _isRunning = true;
                try
                {
                    Run(context);
                }
                //catch (Exception err)
                //{
                //    throw err;//这里不再抛一次，保留原始堆栈信息
                //}
                finally
                {
                    _isRunning = false;
                }
            }
            else
            {
                new RunConflictException("互斥跳过");
            }
        }

        public ScheduleLog ReadLog()
        {
            ScheduleLog q = null;
            if (logger.Count > 0)
            {
                q = logger.Dequeue();
            }
            return q;
        }
    }
}
