using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class ScheduleReferenceEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        public Guid ScheduleId { get; set; }

        public Guid ChildId { get; set; }
    }
}
