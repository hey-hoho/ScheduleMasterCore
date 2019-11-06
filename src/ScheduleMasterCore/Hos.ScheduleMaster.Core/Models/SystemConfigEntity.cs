using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class SystemConfigEntity
    {
        [Key, MaxLength(50)]
        public string Key { get; set; }

        [Required, MaxLength(50)]
        public string Group { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Value { get; set; }

        public int Sort { get; set; }

        public bool IsReuired { get; set; }

        [MaxLength(500)]
        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string UpdateUserName { get; set; }
    }
}
