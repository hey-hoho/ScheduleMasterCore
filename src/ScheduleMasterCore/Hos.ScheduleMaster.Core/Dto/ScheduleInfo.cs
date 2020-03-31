using Hos.ScheduleMaster.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Dto
{
    public class ScheduleInfo
    {
        public ScheduleInfo()
        {
            this.Keepers = new List<int>();
            this.Nexts = new List<Guid>();
            this.Params = new List<ScheduleParam>();
        }

        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        public int MetaType { get; set; }

        public bool RunLoop { get; set; }

        [MaxLength(500)]
        public string Remark { get; set; }

        [MaxLength(50)]
        public string CronExpression { get; set; }

        [MaxLength(200)]
        public string AssemblyName { get; set; }

        [MaxLength(200)]
        public string ClassName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(2000)]
        public string CustomParamsJson { get; set; }

        public bool RunNow { get; set; }

        public string CreateUserName { get; set; }

        [MaxLength(500)]
        public string HttpRequestUrl { get; set; }

        [MaxLength(10)]
        public string HttpMethod { get; set; }

        [MaxLength(50)]
        public string HttpContentType { get; set; }

        [MaxLength(2000)]
        public string HttpHeaders { get; set; }

        [MaxLength(2000)]
        public string HttpBody { get; set; }

        /// <summary>
        /// 报警联系人id
        /// </summary>
        public List<int> Keepers { get; set; }

        /// <summary>
        /// 后置任务id
        /// </summary>
        public List<Guid> Nexts { get; set; }

        /// <summary>
        /// 执行worker
        /// </summary>
        public List<string> Executors { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public List<ScheduleParam> Params
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScheduleParam>>(CustomParamsJson);
            }
            set
            {
                this.CustomParamsJson = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }
    }

    public class ScheduleParam
    {
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }
        public string ParamRemark { get; set; }
    }
}
