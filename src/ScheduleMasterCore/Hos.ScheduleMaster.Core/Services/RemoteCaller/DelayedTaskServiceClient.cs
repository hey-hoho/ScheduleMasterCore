using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Services.RemoteCaller
{
    [ServiceMapTo(typeof(DelayedTaskServiceClient))]
    public class DelayedTaskServiceClient : ServerClient
    {
        public DelayedTaskServiceClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }


        public async Task<bool> Start(Guid sid)
        {
            return await PostRequest("/api/delayedtask/insert", sid);
        }

        public async Task<bool> Remove(Guid sid)
        {
            return await PostRequest("/api/delayedtask/remove", sid);
        }

        public async Task<bool> Execute(Guid sid)
        {
            return await PostRequest("/api/delayedtask/execute", sid);
        }
    }
}
