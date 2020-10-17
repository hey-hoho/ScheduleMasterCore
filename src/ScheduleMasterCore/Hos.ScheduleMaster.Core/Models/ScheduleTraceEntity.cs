using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("scheduletraces")]
    public class ScheduleTraceEntity : IEntity
    {
        [Key]
        [Column("traceid")]
        public Guid TraceId { get; set; }

        [Column("scheduleid")]
        public Guid ScheduleId { get; set; }

        /// <summary>
        /// 所在节点
        /// </summary>
        [Column("node")]
        [MaxLength(100)]
        public string Node { get; set; }

        /// <summary>
        /// 开始运行时间
        /// </summary>
        [Column("starttime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束运行时间
        /// </summary>
        [Column("endtime")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 执行耗时，单位是秒
        /// </summary>
        [Column("elapsedtime")]
        public double ElapsedTime { get; set; }

        /// <summary>
        /// 运行结果 0-无结果 1-运行成功 2-运行失败 3- 互斥取消 
        /// </summary>
        [Column("result")]
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
