using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("systemconfigs")]
    public class SystemConfigEntity : IEntity
    {
        [Key, MaxLength(50)]
        [Column("key")]
        public string Key { get; set; }

        [Required, MaxLength(50)]
        [Column("group")]
        public string Group { get; set; }

        [Required, MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(1000)]
        [Column("value")]
        public string Value { get; set; }

        [Column("sort")]
        public int Sort { get; set; }

        [Column("isreuired")]
        public bool IsReuired { get; set; }

        [MaxLength(500)]
        [Column("remark")]
        public string Remark { get; set; }

        [Column("createtime")]
        public DateTime CreateTime { get; set; }

        [Column("updatetime")]
        public DateTime? UpdateTime { get; set; }

        [Column("updateusername")]
        [MaxLength(50)]
        public string UpdateUserName { get; set; }
    }
}
