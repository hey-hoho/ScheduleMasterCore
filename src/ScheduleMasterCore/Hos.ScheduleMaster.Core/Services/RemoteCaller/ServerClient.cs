using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Services.RemoteCaller
{
    [ServiceMapTo(typeof(ServerClient))]
    public class ServerClient
    {
        protected IHttpClientFactory _httpClientFactory;

        protected ServerNodeEntity _server;

        public ServerClient(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        protected HttpClient CreateClient()
        {
            if (_server == null)
            {
                throw new ArgumentException("no target worker that can send the request.");
            }
            HttpClient client = _httpClientFactory.CreateClient("workercaller");
            client.DefaultRequestHeaders.Add("sm_secret", _server.AccessSecret);
            client.BaseAddress = new Uri($"{_server.AccessProtocol}://{_server.Host}");
            return client;
        }

        public ServerNodeEntity Server
        {
            set
            {
                _server = value;
            }
        }

        public async Task<(bool success, string content)> Connect()
        {
            HttpClient client = CreateClient();
            client.DefaultRequestHeaders.Add("sm_connection", SecurityHelper.MD5(ConfigurationCache.NodeSetting.IdentityName));
            client.DefaultRequestHeaders.Add("sm_nameto", _server.NodeName);

            var response = await client.PostAsync("/api/server/connect", null);
            return (response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> StartUp()
        {
            HttpClient client = CreateClient();
            var response = await client.PostAsync("/api/server/startup", null);
            return await ClientResponse(response);
        }

        public async Task<bool> Shutdown()
        {
            HttpClient client = CreateClient();
            var response = await client.PostAsync("/api/server/shutdown", null);
            return await ClientResponse(response);
        }

        public async Task<bool> HealthCheck()
        {
            HttpClient client = CreateClient();
            try
            {
                var response = await client.GetAsync("/health");
                return await ClientResponse(response);
            }
            catch (Exception ex)
            {
                Log.LogHelper.Warn($"请求地址：{client.BaseAddress.ToString()}/health，响应消息：{(ex.InnerException ?? ex).Message}");
                return false;
            }
        }

        protected HttpContent CreateRequestContent(Guid sid)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
            args.Add(new KeyValuePair<string, string>("sid", sid.ToString()));
            return new FormUrlEncodedContent(args);
        }

        protected async Task<bool> ClientResponse(HttpResponseMessage response)
        {
            bool success = response.IsSuccessStatusCode;
            if (!success)
            {
                Log.LogHelper.Warn($"响应码：{response.StatusCode.GetHashCode().ToString()}，请求地址：{response.RequestMessage.RequestUri.AbsoluteUri}，响应消息：{await response.Content.ReadAsStringAsync()}");
            }
            return success;
        }

        protected async Task<bool> PostRequest(string router, Guid sid)
        {
            HttpClient client = CreateClient();
            var content = CreateRequestContent(sid);
            var response = await client.PostAsync(router, content);
            return await ClientResponse(response);
        }
    }
}
