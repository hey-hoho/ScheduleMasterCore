using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("schedulereferences")]
    public class ScheduleReferenceEntity : IEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("scheduleid")]
        public Guid ScheduleId { get; set; }

        [Column("childid")]
        public Guid ChildId { get; set; }
    }
}
