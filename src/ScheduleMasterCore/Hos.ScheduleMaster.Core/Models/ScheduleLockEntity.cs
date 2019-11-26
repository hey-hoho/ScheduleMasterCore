using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class ScheduleLockEntity : IEntity
    {
        [Key]
        public Guid ScheduleId { get; set; }

        [Required]
        public int Status { get; set; }
    }

}
