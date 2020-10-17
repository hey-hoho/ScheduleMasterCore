using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("servernodes")]
    public class ServerNodeEntity : IEntity
    {
        /// <summary>
        /// 节点标识
        /// </summary>
        [Key]
        [MaxLength(100)]
        [Column("nodename")]
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型 master/worker
        /// </summary>
        [Required]
        [Column("nodetype")]
        [MaxLength(20)]
        public string NodeType { get; set; }

        /// <summary>
        /// 所在机器
        /// </summary>
        [Column("machinename")]
        [MaxLength(100)]
        public string MachineName { get; set; }

        /// <summary>
        /// 访问协议，http/https
        /// </summary>
        [Required]
        [Column("accessprotocol")]
        [MaxLength(20)]
        public string AccessProtocol { get; set; }

        /// <summary>
        /// 节点主机(IP+端口)
        /// </summary>
        [Required]
        [Column("host")]
        [MaxLength(100)]
        public string Host { get; set; }

        /// <summary>
        /// 访问秘钥，每次节点激活时会更新，用来验证访问权限
        /// </summary>
        [Column("accesssecret")]
        [MaxLength(50)]
        public string AccessSecret { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Column("lastupdatetime")]
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 节点状态，0-下线，1-空闲，2-运行
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        [Column("priority")]
        public int Priority { get; set; }

        /// <summary>
        /// 当节点类型为worker时，Quartz最大并发执行数量
        /// </summary>
        [Column("maxconcurrency")]
        public int MaxConcurrency { get; set; }
    }
}
