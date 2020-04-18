using Hos.ScheduleMaster.Base;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Hos.ScheduleMaster.QuartzHost.Common;
using Hos.ScheduleMaster.Core;
using System.Threading;

namespace Hos.ScheduleMaster.QuartzHost.HosSchedule
{
    public class HttpSchedule : IHosSchedule
    {
        public ScheduleEntity Main { get; set; }
        public Dictionary<string, object> CustomParams { get; set; }
        public List<KeyValuePair<string, string>> Keepers { get; set; }
        public Dictionary<Guid, string> Children { get; set; }
        public TaskBase RunnableInstance { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public void CreateRunnableInstance(ScheduleView view)
        {
            RunnableInstance = new HttpTask() { HttpOption = view.HttpOption };
        }

        public Type GetQuartzJobType()
        {
            return typeof(RunnableJob.HttpJob);
        }

        public void Dispose()
        {
            RunnableInstance.Dispose();
            RunnableInstance = null;
        }
    }
    public class HttpTask : TaskBase
    {
        public ScheduleHttpOptionEntity HttpOption { get; set; }

        public override void Run(TaskContext context)
        {
            if (HttpOption == null) return;
            context.WriteLog("即将请求：" + HttpOption.RequestUrl);

            var response = DoRequest();
            if (response == null)
            {
                throw new Exception("无响应：" + HttpOption.RequestUrl);
            }
            if (response.IsSuccessful)
            {
                context.WriteLog($"请求结束，响应码：{response.StatusCode.GetHashCode().ToString()}，响应内容：{(response.ContentType.Contains("text/html") ? "html文档" : response.Content)}");
            }
            else
            {
                throw response.ErrorException;
            }
        }

        private IRestResponse DoRequest()
        {
            var client = new RestClient(HttpOption.RequestUrl);
            var request = new RestRequest(GetRestSharpMethod(HttpOption.Method));
            var headers = HosScheduleFactory.ConvertParamsJson(HttpOption.Headers);
            foreach (var item in headers)
            {
                request.AddHeader(item.Key, item.Value.ToString());
            }
            request.AddHeader("content-type", HttpOption.ContentType);
            request.Timeout = 10000;
            int config = ConfigurationCache.GetField<int>("Http_RequestTimeout");
            if (config > 0)
            {
                request.Timeout = config * 1000;
            }
            string requestBody = string.Empty;
            if (HttpOption.ContentType == "application/json")
            {
                requestBody = HttpOption.Body.Replace("\r\n", "");
            }
            else if (HttpOption.ContentType == "application/x-www-form-urlencoded")
            {
                var formData = HosScheduleFactory.ConvertParamsJson(HttpOption.Body);
                requestBody = string.Join('&', formData.Select(x => $"{x.Key}={System.Net.WebUtility.UrlEncode(x.Value.ToString())}"));
                if (request.Method == Method.GET && formData.Count > 0)
                {
                    client.BaseUrl = new Uri($"{HttpOption.RequestUrl}?{requestBody}");
                }
            }
            if (request.Method != Method.GET)
            {
                request.AddParameter(HttpOption.ContentType, requestBody, ParameterType.RequestBody);
            }
            IRestResponse response = client.Execute(request);
            return response;
        }

        private Method GetRestSharpMethod(string method)
        {
            switch (method)
            {
                case "POST": return Method.POST;
                case "PUT": return Method.PUT;
                case "DELETE": return Method.DELETE;
            }
            return Method.GET;
        }
    }
}
