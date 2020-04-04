using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.HosSchedule
{
    public class HttpSchedule : IHosSchedule
    {
        public ScheduleEntity Main { get; set; }
        public Dictionary<string, object> CustomParams { get; set; }
        public List<KeyValuePair<string, string>> Keepers { get; set; }
        public Dictionary<Guid, string> Children { get; set; }
        public TaskBase RunnableInstance { get; set; }

        public void CreateRunnableInstance(ScheduleView view)
        {
            RunnableInstance = new HttpTask() { HttpOption = view.HttpOption };
        }

        public Type GetQuartzJobType()
        {
            return typeof(RunnableJob.HttpJob);
        }

        public void Dispose()
        {

        }
    }
    public class HttpTask : TaskBase
    {
        public ScheduleHttpOptionEntity HttpOption { get; set; }

        public override void Run(TaskContext context)
        {
            if (HttpOption == null) return;
            context.WriteLog(HttpOption.RequestUrl);
        }
    }
}
