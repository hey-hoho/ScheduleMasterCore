using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("systemusers")]
    public class SystemUserEntity : IEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        [Column("username", TypeName = "varchar(50)")]
        public string UserName { get; set; }

        [Required, MaxLength(50)]
        [Column("password", TypeName = "varchar(50)")]
        public string Password { get; set; }

        [Required, MaxLength(50)]
        [Column("realname", TypeName = "varchar(50)")]
        public string RealName { get; set; }

        [MaxLength(15)]
        [Column("phone", TypeName = "varchar(15)")]
        public string Phone { get; set; }

        [MaxLength(500), EmailAddress(ErrorMessage = "邮箱格式错误")]
        [Column("email", TypeName = "varchar(500)")]
        public string Email { get; set; }

        [Required]
        [Column("status")]
        public int Status { get; set; }

        [Column("createtime")]
        public DateTime CreateTime { get; set; }

        [Column("lastlogintime")]
        public DateTime? LastLoginTime { get; set; }
    }

    public enum SystemUserStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Deleted = -1,

        /// <summary>
        /// 已锁定
        /// </summary>
        [Description("已锁定")]
        Disabled = 0,

        /// <summary>
        /// 有效
        /// </summary>
        [Description("有效")]
        Available = 1
    }
}
