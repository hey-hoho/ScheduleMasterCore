using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Base.Dto
{
    public class ScheduleLog
    {
        public int Category { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public Guid ScheduleId { get; set; }

        public string Node { get; set; }

        public Guid TraceId { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public enum LogCategory
    {
        /// <summary>
        /// 普通信息
        /// </summary>
        Info = 1,

        /// <summary>
        /// 警告
        /// </summary>
        Warn = 2,

        /// <summary>
        /// 异常
        /// </summary>
        Error = 3
    }
}
