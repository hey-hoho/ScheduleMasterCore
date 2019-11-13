using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public class ServerNodeEntity : IEntity
    {
        /// <summary>
        /// 节点标识
        /// </summary>
        [Key]
        public string NodeName { get; set; }

        /// <summary>
        /// 节点主机
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 节点状态，1-有效，0-停机
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Priority { get; set; }
    }
}
