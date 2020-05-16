using Hos.ScheduleMaster.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Hos.ScheduleMaster.xUnitTest
{
    public class ApiCreateTest
    {
        [Fact]
        public async Task CreateAssemblyTask()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("ms_auth_user", "admin");
            client.DefaultRequestHeaders.Add("ms_auth_secret", SecurityHelper.MD5($"admin96e79218965eb72c92a549dd5a330112admin"));
            for (int i = 0; i < 5; i++)
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
                args.Add(new KeyValuePair<string, string>("MetaType", "1"));
                args.Add(new KeyValuePair<string, string>("RunLoop", "true"));
                args.Add(new KeyValuePair<string, string>("CronExpression", "33 0/8 * * * ?"));
                args.Add(new KeyValuePair<string, string>("Remark", "By Xunit Tester Created"));
                args.Add(new KeyValuePair<string, string>("StartDate", DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")));
                args.Add(new KeyValuePair<string, string>("Title", $"程序集接口测试任务-{i}"));
                args.Add(new KeyValuePair<string, string>("AssemblyName", "Hos.ScheduleMaster.Demo"));
                args.Add(new KeyValuePair<string, string>("ClassName", "Hos.ScheduleMaster.Demo.Simple"));
                args.Add(new KeyValuePair<string, string>("CustomParamsJson", "[{\"ParamKey\":\"k1\",\"ParamValue\":\"1111\",\"ParamRemark\":\"r1\"},{\"ParamKey\":\"k2\",\"ParamValue\":\"2222\",\"ParamRemark\":\"r2\"}]"));
                args.Add(new KeyValuePair<string, string>("Keepers", "1"));
                args.Add(new KeyValuePair<string, string>("Keepers", "2"));
                //args.Add(new KeyValuePair<string, string>("Nexts", ""));
                //args.Add(new KeyValuePair<string, string>("Executors", ""));
                HttpContent reqContent = new FormUrlEncodedContent(args);
                var response = await client.PostAsync("http://localhost:30000/api/Task/Create", reqContent);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                Assert.True(response.IsSuccessStatusCode && content.Contains("\"Success\":true"));
            }
        }

        [Fact]
        public async Task CreateHttpTask()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("ms_auth_user", "admin");
            client.DefaultRequestHeaders.Add("ms_auth_secret", SecurityHelper.MD5($"admin96e79218965eb72c92a549dd5a330112admin"));
            for (int i = 0; i < 5; i++)
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
                args.Add(new KeyValuePair<string, string>("MetaType", "2"));
                args.Add(new KeyValuePair<string, string>("RunLoop", "true"));
                args.Add(new KeyValuePair<string, string>("CronExpression", "22 0/8 * * * ?"));
                args.Add(new KeyValuePair<string, string>("Remark", "By Xunit Tester Created"));
                args.Add(new KeyValuePair<string, string>("StartDate", DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")));
                args.Add(new KeyValuePair<string, string>("Title", $"Http接口测试任务-{i}"));
                args.Add(new KeyValuePair<string, string>("HttpRequestUrl", "http://localhost:56655/api/1.0/value/jsonpost"));
                args.Add(new KeyValuePair<string, string>("HttpMethod", "POST"));
                args.Add(new KeyValuePair<string, string>("HttpContentType", "application/json"));
                args.Add(new KeyValuePair<string, string>("HttpHeaders", "[]"));
                args.Add(new KeyValuePair<string, string>("HttpBody", "{ \"Posts\": [{ \"PostId\": 666, \"Title\": \"tester\", \"Content\":\"testtesttest\" }], \"BlogId\": 111, \"Url\":\"qweqrrttryrtyrtrtrt\" }"));
                HttpContent reqContent = new FormUrlEncodedContent(args);
                var response = await client.PostAsync("http://localhost:30000/api/Task/Create", reqContent);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                Assert.True(response.IsSuccessStatusCode && content.Contains("\"Success\":true"));
            }
        }
    }
}
