using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class TaskGuardianEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        public int UserId { get; set; }
    }
}
