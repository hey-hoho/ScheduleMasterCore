using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class ScheduleEntity : IEntity
    {
        public ScheduleEntity()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// 任务id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Required, MaxLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否周期运行
        /// </summary>
        [Required]
        public bool RunMoreTimes { get; set; }

        /// <summary>
        /// cron表达式
        /// </summary>
        [MaxLength(50)]
        public string CronExpression { get; set; }

        /// <summary>
        /// 任务所在程序集
        /// </summary>
        [Required, MaxLength(200)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务的类型
        /// </summary>
        [Required, MaxLength(200)]
        public string ClassName { get; set; }

        /// <summary>
        /// 自定义参数（json格式）
        /// </summary>
        [MaxLength(2000)]
        public string CustomParamsJson { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        [Required]
        public int Status { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreateUserId { get; set; }

        /// <summary>
        /// 创建人账号
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 上次运行时间
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>
        public DateTime? NextRunTime { get; set; }

        /// <summary>
        /// 总运行成功次数
        /// </summary>
        public int TotalRunCount { get; set; }


    }

    public class ScheduleView
    {
        public ScheduleEntity Schedule { get; set; }

        public List<KeyValuePair<string, string>> Keepers { get; set; }

        public Dictionary<Guid, string> Children { get; set; }
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum ScheduleStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = -1,

        /// <summary>
        /// 已停止
        /// </summary>
        Stop = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused = 2

    }
}
