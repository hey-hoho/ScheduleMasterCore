using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class SystemLogEntity : IEntity
    {
        public SystemLogEntity()
        {
            //this.ScheduleId = Guid.NewGuid();//0表示系统运行日志
            //CreateTime = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [Required]
        public int Category { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        public Guid? ScheduleId { get; set; }

        /// <summary>
        /// 产生节点
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// 任务运行轨迹
        /// </summary>
        public Guid? TraceId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 日志类型枚举
    /// </summary>
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
