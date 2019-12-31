using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class ScheduleTraceEntity : IEntity
    {
        [Key]
        public Guid TraceId { get; set; }

        public Guid ScheduleId { get; set; }

        /// <summary>
        /// 所在节点
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// 开始运行时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束运行时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 执行耗时，单位是秒
        /// </summary>
        public double ElapsedTime { get; set; }

        /// <summary>
        /// 运行结果 0-无结果 1-运行成功 2-运行失败 3- 互斥取消 
        /// </summary>
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
        [Description("无结果")]
        Null = 0,

        /// <summary>
        /// 运行成功
        /// </summary>
        [Description("运行成功")]
        Success = 1,

        /// <summary>
        /// 运行失败
        /// </summary>
        [Description("运行失败")]
        Failed = 2,

        /// <summary>
        /// 互斥取消
        /// </summary>
        [Description("互斥取消")]
        Conflict = 3

    }
}
