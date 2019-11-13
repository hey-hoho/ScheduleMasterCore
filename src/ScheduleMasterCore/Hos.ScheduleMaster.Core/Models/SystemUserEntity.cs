using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class SystemUserEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string RealName { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(500), EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; }

        [Required]
        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

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
