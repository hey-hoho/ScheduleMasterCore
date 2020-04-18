using Hos.ScheduleMaster.Base.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Hos.ScheduleMaster.Base
{
    /// <summary>
    /// 任务运行时的上下文
    /// </summary>
    public class TaskContext
    {
        private readonly TaskBase _instance;

        public TaskContext(TaskBase instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// 所在节点
        /// </summary>
        public string Node { private get; set; }

        /// <summary>
        /// 运行轨迹
        /// </summary>
        public Guid TraceId { private get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public Dictionary<string, object> ParamsDict { private get; set; }

        /// <summary>
        /// 前置任务的运行结果
        /// </summary>
        public object PreviousResult { get; set; }

        /// <summary>
        /// 本次运行的返回结果
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 获取自定义参数字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetArgument<T>(string name)
        {
            if (ParamsDict == null)
            {
                return default;
            }
            try
            {
                object value;
                ParamsDict.TryGetValue(name, out value);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void InstanceRun()
        {
            _instance.InnerRun(this);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void WriteLog(string message, LogCategory type = LogCategory.Info)
        {
            _instance.logger.Enqueue(new ScheduleLog
            {
                Category = (int)type,
                Message = message,
                CreateTime = DateTime.Now,
                ScheduleId = _instance.TaskId,
                Node = Node,
                TraceId = TraceId
            });
        }

        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="ex"></param>
        public void WriteError(Exception ex)
        {
            _instance.logger.Enqueue(new ScheduleLog
            {
                Category = (int)LogCategory.Error,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                CreateTime = DateTime.Now,
                ScheduleId = _instance.TaskId,
                Node = Node,
                TraceId = TraceId
            });
        }

    }
}
