using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Core.Interface
{
    public interface IScheduleService
    {
        List<ScheduleEntity> QueryAll();

        ListPager<ScheduleEntity> QueryPager(ListPager<ScheduleEntity> pager);

        ListPager<SystemLogEntity> QueryLogPager(ListPager<SystemLogEntity> pager);

        int DeleteLog(Guid? sid, int? category, DateTime? startdate, DateTime? enddate);

        ScheduleEntity QueryById(Guid sid);

        List<ScheduleKeeperEntity> QueryTaskGuardians(Guid sid);

        List<ScheduleReferenceEntity> QueryTaskReferences(Guid sid);

        ApiResponseMessage AddTask(ScheduleEntity model, List<int> guardians, List<Guid> nexts);

        ApiResponseMessage EditTask(TaskInfo model);

        ApiResponseMessage TaskStart(ScheduleEntity task);

        ApiResponseMessage PauseTask(Guid sid);

        ApiResponseMessage ResumeTask(Guid sid);

        ApiResponseMessage RunOnceTask(Guid sid);

        ApiResponseMessage StopTask(Guid sid);

        ApiResponseMessage DeleteTask(Guid sid);
    }
}
