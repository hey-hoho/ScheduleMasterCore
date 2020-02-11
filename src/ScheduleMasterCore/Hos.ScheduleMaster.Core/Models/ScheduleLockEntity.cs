using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("schedulelocks")]
    public class ScheduleLockEntity : IEntity
    {
        [Key]
        [Column("scheduleid")]
        public Guid ScheduleId { get; set; }

        [Required]
        [Column("status")]
        public int Status { get; set; }
    }

}
