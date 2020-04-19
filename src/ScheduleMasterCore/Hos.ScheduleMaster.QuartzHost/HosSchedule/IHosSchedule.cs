using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.HosSchedule
{
    public interface IHosSchedule
    {
        ScheduleEntity Main { get; set; }

        Dictionary<string, object> CustomParams { get; set; }

        List<KeyValuePair<string, string>> Keepers { get; set; }

        Dictionary<Guid, string> Children { get; set; }

        TaskBase RunnableInstance { get; set; }

        CancellationTokenSource CancellationTokenSource { get; set; }

        void CreateRunnableInstance(ScheduleContext context);

        Type GetQuartzJobType();

        void Dispose();
    }
}
