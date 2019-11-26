using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class ScheduleTraceEntity : IEntity
    {
        [Key]
        public Guid TraceId { get; set; }

        public Guid ScheduleId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double TimeSpan { get; set; }

        public int Result { get; set; }

    }

    /// <summary>
    /// 任务运行结果
    /// </summary>
    public enum ScheduleRunResult
    {

        /// <summary>
        /// 无结果
        /// </summary>
        Null = 0,

        /// <summary>
        /// 运行成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 运行失败
        /// </summary>
        Failed = 2,

        /// <summary>
        /// 互斥取消
        /// </summary>
        Conflict = 3

    }
}
