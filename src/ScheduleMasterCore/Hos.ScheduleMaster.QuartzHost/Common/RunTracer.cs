using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.Common
{
    public class RunTracer
    {
        private SmDbContext _dbContext;

        private Stopwatch _stopwatch;

        private Guid _traceId;

        private readonly string _dateNum = DateTime.Today.ToString("yyyyMMdd");

        public RunTracer(SmDbContext dbContext)
        {
            _dbContext = dbContext;
            _stopwatch = new Stopwatch();
        }

        public async Task Begin(Guid traceId, string sid)
        {
            _traceId = traceId;

            await _dbContext.Database.ExecuteSqlRawAsync(
                $"insert into scheduletraces values('{traceId.ToString()}','{sid}','{ConfigurationCache.NodeSetting.IdentityName}',{_dbContext.GetDbNowDateTime},'0001-01-01',0,0)");

            bool isUpdate = false;
            if (_dbContext.TraceStatisticss.Any(x => x.DateNum == Convert.ToInt32(_dateNum)))
            {
                isUpdate = true;
            }
            else
            {
                try
                {
                    await _dbContext.Database.ExecuteSqlRawAsync(
                        $"insert into tracestatistics values ({_dateNum},{DateTime.Today.ToTimeStamp().ToString()},0,0,1,{_dbContext.GetDbNowDateTime})");
                }
                catch (Exception)
                {
                    isUpdate = true;
                }
            }
            if (isUpdate)
            {
                await _dbContext.Database.ExecuteSqlRawAsync(
                    $"update tracestatistics set other=other+1,lastupdatetime={_dbContext.GetDbNowDateTime} where datenum={_dateNum}");
            }

            _stopwatch.Restart();
        }

        public async Task<double> Complete(ScheduleRunResult result)
        {
            if (_traceId == Guid.Empty) return 0;

            _stopwatch.Stop();

            await _dbContext.Database.ExecuteSqlRawAsync(
                $"update scheduletraces set result={result.GetHashCode().ToString()},elapsedtime={Math.Round(_stopwatch.Elapsed.TotalSeconds, 3).ToString()},endtime={_dbContext.GetDbNowDateTime} where traceid='{_traceId.ToString()}'");

            if (result == ScheduleRunResult.Success)
            {
                await _dbContext.Database.ExecuteSqlRawAsync(
                    $"update tracestatistics set success=success+1,other=other-1,lastupdatetime={_dbContext.GetDbNowDateTime} where datenum={_dateNum}");
            }
            else if (result == ScheduleRunResult.Failed)
            {
                await _dbContext.Database.ExecuteSqlRawAsync(
                    $"update tracestatistics set fail=fail+1,other=other-1,lastupdatetime={_dbContext.GetDbNowDateTime} where datenum={_dateNum}");
            }

            return Math.Round(_stopwatch.Elapsed.TotalMilliseconds, 3);
        }
    }
}
