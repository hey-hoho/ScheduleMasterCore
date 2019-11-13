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
            this.TaskId = 0;//0表示系统运行日志
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int Category { get; set; }

        [Required]
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public int TaskId { get; set; }

        public Guid? TraceId { get; set; }

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
