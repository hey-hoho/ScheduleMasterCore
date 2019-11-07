using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Core.Interface
{
    public interface ITaskService
    {
        List<TaskEntity> QueryAll();

        ListPager<TaskEntity> QueryPager(ListPager<TaskEntity> pager);

        ListPager<SystemLogEntity> QueryLogPager(ListPager<SystemLogEntity> pager);

        int DeleteLog(int? task, int? category, DateTime? startdate, DateTime? enddate);

        TaskEntity QueryById(int id);

        List<TaskGuardianEntity> QueryTaskGuardians(int taskId);

        List<TaskReferenceEntity> QueryTaskReferences(int taskId);

        ApiResponseMessage AddTask(TaskEntity model, List<int> guardians, List<int> nexts);

        ApiResponseMessage EditTask(TaskInfo model);

        ApiResponseMessage TaskStart(TaskEntity task);

        ApiResponseMessage PauseTask(int id);

        ApiResponseMessage ResumeTask(int id);

        ApiResponseMessage RunOnceTask(int id);

        ApiResponseMessage StopTask(int id);

        ApiResponseMessage DeleteTask(int id);
    }
}
