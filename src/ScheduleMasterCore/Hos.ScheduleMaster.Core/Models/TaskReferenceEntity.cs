using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class TaskReferenceEntity
    {
        [Key]
        public int Id { get; set; }

        public int ParentTaskId { get; set; }

        public int ChildTaskId { get; set; }
    }
}
