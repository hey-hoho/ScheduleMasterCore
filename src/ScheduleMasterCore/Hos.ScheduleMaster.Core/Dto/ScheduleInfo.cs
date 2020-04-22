using Hos.ScheduleMaster.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Dto
{
    public class ScheduleInfo: Models.ScheduleEntity
    {
        public ScheduleInfo()
        {
            this.Keepers = new List<int>();
            this.Nexts = new List<Guid>();
            this.Params = new List<ScheduleParam>();
        }

        public bool RunNow { get; set; }

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


}
