using Hos.ScheduleMaster.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("schedulehttpoptions")]
    public class ScheduleHttpOptionEntity : IEntity
    {
        /// <summary>
        /// 任务id
        /// </summary>
        [Key]
        [Column("scheduleid", TypeName = "varchar(36)"), Required]
        public Guid ScheduleId { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [Column("requesturl", TypeName = "nvarchar(500)"), MaxLength(500), Required]
        [MapperSetting(BindField = "HttpRequestUrl")]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        [Column("method", TypeName = "nvarchar(10)"), MaxLength(10), Required]        
        [MapperSetting(BindField = "HttpMethod")]
        public string Method { get; set; }

        /// <summary>
        /// 数据格式
        /// </summary>
        [Column("contenttype", TypeName = "nvarchar(50)"), MaxLength(50), Required]
        [MapperSetting(BindField = "HttpContentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// 自定义请求头（json格式）
        /// </summary>
        [Column("headers", TypeName = "varchar(max)")]
        [MapperSetting(BindField = "HttpHeaders")]
        public string Headers { get; set; }

        /// <summary>
        /// 数据内容（json格式）
        /// </summary>
        [Column("body", TypeName = "varchar(max)")]
        [MapperSetting(BindField = "HttpBody")]
        public string Body { get; set; }
    }
}
