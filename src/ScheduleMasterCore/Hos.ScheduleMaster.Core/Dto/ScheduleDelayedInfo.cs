using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Dto
{
    [Serializable]
    public class ScheduleDelayedInfo: ScheduleDelayedEntity
    {

        /// <summary>
        /// 执行worker
        /// </summary>
        public List<string> Executors { get; set; }
    }
}
