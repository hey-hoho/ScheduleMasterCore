using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Core
{
    public class ScheduleContext
    {
        public ScheduleEntity Schedule { get; set; }

        public ScheduleHttpOptionEntity HttpOption { get; set; }

        public List<ScheduleParam> Params
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScheduleParam>>(Schedule.CustomParamsJson);
            }
        }

        public List<KeyValuePair<string, string>> Keepers { get; set; }

        public Dictionary<Guid, string> Children { get; set; }

        public List<string> Executors { get; set; }
    }

    public class ScheduleParam
    {
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }
        public string ParamRemark { get; set; }
    }
}
