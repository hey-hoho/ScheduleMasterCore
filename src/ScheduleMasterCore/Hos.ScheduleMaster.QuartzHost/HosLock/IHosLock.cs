using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.HosLock
{
    public interface IHosLock : IDisposable
    {

        bool TryGetLock(string key);
    }
}
