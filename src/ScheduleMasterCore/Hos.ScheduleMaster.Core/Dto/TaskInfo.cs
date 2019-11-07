using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hos.ScheduleMaster.Core.Dto
{
    public class TaskInfo
    {
        public TaskInfo()
        {
            this.Guardians = new List<int>();
            this.Nexts = new List<int>();
            this.Params = new List<TaskParam>();
        }

        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        public bool RunMoreTimes { get; set; }

        [MaxLength(500)]
        public string Remark { get; set; }

        [MaxLength(50)]
        public string CronExpression { get; set; }

        [Required, MaxLength(200)]
        public string AssemblyName { get; set; }

        [Required, MaxLength(200)]
        public string ClassName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(2000)]
        public string CustomParamsJson { get; set; }

        public bool RunNow { get; set; }

        /// <summary>
        /// 报警联系人id
        /// </summary>
        public List<int> Guardians { get; set; }

        /// <summary>
        /// 后置任务id
        /// </summary>
        public List<int> Nexts { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public List<TaskParam> Params
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskParam>>(CustomParamsJson);
            }
            set
            {
                this.CustomParamsJson = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }
    }

    public class TaskParam
    {
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }
        public string ParamRemark { get; set; }
    }
}
