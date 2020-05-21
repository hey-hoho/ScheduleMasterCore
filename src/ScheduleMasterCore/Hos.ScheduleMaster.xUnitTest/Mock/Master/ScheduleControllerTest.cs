using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Web.Controllers;
using Hos.ScheduleMaster.Web.Extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Hos.ScheduleMaster.xUnitTest.Mock.Master
{

    public class ScheduleControllerTest
    {
        private ScheduleController _controller;
        public ScheduleControllerTest()
        {
            _controller = MockController.CreateMvcController<ScheduleController>();
        }

        [Fact]
        public void GetList()
        {
            JsonNetResult result = (JsonNetResult)_controller.QueryPager(null);
            Assert.True(result.Data != null);
        }

        [Fact]
        public void GetDetail()
        {
            Guid sid = Guid.Parse("0b342982-efc9-41a9-b3cb-1385b517fcd1");
            ViewResult result = (ViewResult)_controller.Detail(sid);
            Assert.Equal(sid, (result.Model as ScheduleContext).Schedule.Id);
        }
    }
}
