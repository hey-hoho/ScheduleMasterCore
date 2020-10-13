using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Hos.ScheduleMaster.QuartzHost.HosLock
{
    public class DatabaseLock : IHosLock
    {
        private bool _isLocked;

        private SmDbContext _dbContext;

        private string _key;

        public DatabaseLock(SmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            if (_isLocked)
            {
                System.Threading.Thread.Sleep(1000);
                _dbContext.Database.ExecuteSqlRaw($"update schedulelocks set status=0,lockedtime=null,lockednode=null where scheduleid='{_key}'");
            }
        }

        public bool TryGetLock(string key)
        {
            _key = key;
            _isLocked = _dbContext.Database.ExecuteSqlRaw($"update schedulelocks set status=1,lockedtime={_dbContext.GetDbNowDateTime},lockednode='{ConfigurationCache.NodeSetting.IdentityName}' where scheduleid='{key}' and status=0") > 0;
            return _isLocked;
        }
    }
}
