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
        [Column("key", TypeName = "varchar(50)")]
        public string Key { get; set; }

        [Required, MaxLength(50)]
        [Column("group", TypeName = "varchar(50)")]
        public string Group { get; set; }

        [Required, MaxLength(100)]
        [Column("name", TypeName = "varchar(100)")]
        public string Name { get; set; }

        [MaxLength(1000)]
        [Column("value", TypeName = "varchar(1000)")]
        public string Value { get; set; }

        [Column("sort")]
        public int Sort { get; set; }

        [Column("isreuired")]
        public bool IsReuired { get; set; }

        [MaxLength(500)]
        [Column("remark", TypeName = "varchar(500)")]
        public string Remark { get; set; }

        [Column("createtime")]
        public DateTime CreateTime { get; set; }

        [Column("updatetime")]
        public DateTime? UpdateTime { get; set; }

        [Column("updateusername", TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string UpdateUserName { get; set; }
    }
}
