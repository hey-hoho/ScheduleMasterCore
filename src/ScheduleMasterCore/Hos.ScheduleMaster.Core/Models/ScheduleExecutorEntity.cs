using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("scheduleexecutors")]
    public class ScheduleExecutorEntity : IEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        [Column("scheduleid")]
        public Guid ScheduleId { get; set; }

        /// <summary>
        /// worker名称
        /// </summary>
        [Column("workername")]
        [MaxLength(100)]
        public string WorkerName { get; set; }
    }
}
