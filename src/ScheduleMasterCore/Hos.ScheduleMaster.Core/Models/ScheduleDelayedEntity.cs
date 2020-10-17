using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("scheduledelayeds")]
    public class ScheduleDelayedEntity : IEntity
    {
        public ScheduleDelayedEntity()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// 任务id
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 应用来源
        /// </summary>
        [Required, MaxLength(50)]
        [Column("sourceapp")]
        public string SourceApp { get; set; }

        /// <summary>
        /// 主题，用来标记分组
        /// </summary>
        [Required, MaxLength(100)]
        [Column("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// 关键字，方便任务跟踪
        /// </summary>
        [Required, MaxLength(100)]
        [Column("contentkey")]
        public string ContentKey { get; set; }

        /// <summary>
        /// 延迟的间隔时间，单位是秒
        /// </summary>
        [Required]
        [Column("delaytimespan")]
        public int DelayTimeSpan { get; set; }

        /// <summary>
        /// 延迟的绝对时间
        /// </summary>
        [Required]
        [Column("delayabsolutetime")]
        public DateTime DelayAbsoluteTime { get; set; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        [Column("createtime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人用户名
        /// </summary>
        [MaxLength(50)]
        [Column("createusername")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 最后执行时间
        /// </summary>
        [Column("executetime")]
        public DateTime? ExecuteTime { get; set; }

        /// <summary>
        /// 执行成功时间
        /// </summary>
        [Column("finishtime")]
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// 任务状态 0-已作废 1-已创建 2-已就绪 3-执行成功 4-异常
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        /// <summary>
        /// 失败重试次数
        /// </summary>
        [Column("failedretrys")]
        public int FailedRetrys { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(255)]
        [Column("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 回调URL
        /// </summary>
        [Required, MaxLength(255)]
        [Column("notifyurl")]
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 回调的数据格式
        /// </summary>
        [Required, MaxLength(50)]
        [Column("notifydatatype")]
        public string NotifyDataType { get; set; }

        /// <summary>
        /// 回调参数
        /// </summary>
        [Required, MaxLength(1000)]
        [Column("notifybody")]
        public string NotifyBody { get; set; }

    }

    /// <summary>
    /// 延时任务状态
    /// </summary>
    public enum ScheduleDelayStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Deleted = 0,

        /// <summary>
        /// 已创建
        /// </summary>
        [Description("已创建")]
        Created = 1,

        /// <summary>
        /// 已就绪
        /// </summary>
        [Description("已就绪")]
        Ready = 2,

        /// <summary>
        /// 执行成功
        /// </summary>
        [Description("执行成功")]
        Successed = 3,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Failed = 4
    }
}
