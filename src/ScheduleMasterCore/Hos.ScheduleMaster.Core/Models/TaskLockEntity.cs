using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class TaskLockEntity
    {
        [Key]
        public Guid TaskId { get; set; }

        [Required]
        public int Status { get; set; }
    }

}
