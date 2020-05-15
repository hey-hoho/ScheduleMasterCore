using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


namespace Hos.ScheduleMaster.xUnitTest
{
    public class ApiHandleTest
    {
        [Fact]
        public void CreateAssemblyTask()
        {
            //Common.QuartzManager.StartWithRetry(new Core.Models.ScheduleView
            //{
            //    Schedule = new Core.Models.ScheduleEntity
            //    {
            //        AssemblyName = "Hos.ScheduleMaster.Demo",
            //        ClassName = "Hos.ScheduleMaster.Demo.Simple",
            //        CreateTime = DateTime.Now,
            //        CreateUserId = 1,
            //        CreateUserName = "hh",
            //        CronExpression = "0/1 * * * * ?",
            //        Id = Guid.NewGuid(),
            //        RunMoreTimes = true,
            //        StartDate = DateTime.Now.AddMinutes(-3),
            //        Status = 0,
            //        Title = "测试任务"
            //    }
            //}, (sid, time) => { });
        }

        [Fact]
        public void CreateHttpTask()
        {

        }
    }
}
