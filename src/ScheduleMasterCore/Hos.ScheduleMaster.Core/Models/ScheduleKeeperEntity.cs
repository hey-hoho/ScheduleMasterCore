using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("schedulekeepers")]
    public class ScheduleKeeperEntity : IEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("scheduleid", TypeName = "varchar(36)")]
        public Guid ScheduleId { get; set; }

        [Column("userid")]
        public int UserId { get; set; }
    }
}
