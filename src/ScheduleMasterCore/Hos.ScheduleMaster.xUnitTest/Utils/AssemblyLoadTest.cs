using Hos.ScheduleMaster.QuartzHost.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Hos.ScheduleMaster.xUnitTest.Utils
{
    public class AssemblyLoadTest
    {
        [Fact]
        public void LoadTest()
        {
            Guid sid = Guid.Parse("3c915575-7171-4d95-b08e-9f082b3f58b0");

            var loadContext = AssemblyHelper.LoadAssemblyContext(sid, "Hos.ScheduleMaster.Demo");

            Assert.True(loadContext != null);

            var instance = AssemblyHelper.CreateTaskInstance(loadContext, sid, "Hos.ScheduleMaster.Demo", "Hos.ScheduleMaster.Demo.Simple");

            Assert.True(instance != null);
        }
    }
}
