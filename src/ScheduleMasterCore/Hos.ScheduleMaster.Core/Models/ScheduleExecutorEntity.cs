using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class ScheduleExecutorEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        public Guid ScheduleId { get; set; }

        /// <summary>
        /// worker名称
        /// </summary>
        public string WorkerName { get; set; }
    }
}
