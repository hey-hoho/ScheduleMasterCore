using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Services.RemoteCaller
{
    [ServiceMapTo(typeof(ScheduleServiceClient))]
    public class ScheduleServiceClient : ServerClient
    {
        public ScheduleServiceClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<bool> Start(Guid sid)
        {
            return await PostRequest("/api/quartz/start", sid);
        }

        public async Task<bool> Stop(Guid sid)
        {
            return await PostRequest("/api/quartz/stop", sid);
        }

        public async Task<bool> Pause(Guid sid)
        {
            return await PostRequest("/api/quartz/pause", sid);
        }

        public async Task<bool> Resume(Guid sid)
        {
            return await PostRequest("/api/quartz/resume", sid);
        }

        public async Task<bool> RunOnce(Guid sid)
        {
            return await PostRequest("/api/quartz/runonce", sid);
        }
    }
}
