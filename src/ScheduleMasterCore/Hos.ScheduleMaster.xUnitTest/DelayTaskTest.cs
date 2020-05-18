using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.QuartzHost.DelayedTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Hos.ScheduleMaster.xUnitTest
{
    public class DelayTaskTest
    {
        [Fact]
        public void Run()
        {
            //初始化容器
            DelayPlanManager.Init();
            Debug.WriteLine($"延时队列初始化完成时间：{DateTime.Now}");

            //构造消费者
            System.Threading.Thread consumer = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    DelayPlanManager.Read();
                    System.Threading.Thread.Sleep(1000);
                }
            });
            consumer.IsBackground = true;
            consumer.Start();

            var t = DateTime.Now.AddSeconds(30);
            Func<DelayQueueSlot, Task> callback = (result) =>
             {
                 var np = result as NotifyPlan;
                 //模拟业务
                 Debug.WriteLine($"[{DateTime.Now}]ID：{np.Key}，地址：{np.NotifyUrl}，延迟秒数：{np.TimeSpan}");

                 Assert.Equal(DateTime.Now, t);

                 return Task.CompletedTask;
             };

            Debug.WriteLine($"预期时间{t}");
            DelayPlanManager.Insert(new NotifyPlan
            {
                NotifyUrl = "",
                Key = Guid.NewGuid().ToString(),
                Callback = callback
            }, t);


            //模拟生产端写入任务
            Task[] tasks = new Task[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = new Task(() =>
                {
                    for (int k = 0; k < 200; k++)
                    {
                        int rndNum = new Random().Next(20, 500);
                        DelayPlanManager.Insert(new NotifyPlan
                        {
                            NotifyUrl = "",
                            Key = Guid.NewGuid().ToString(),
                            Callback = callback
                        }, DateTime.Now.AddSeconds(rndNum));
                    }
                }, TaskCreationOptions.LongRunning);
                tasks[i].Start();
            }
            Task.WaitAll(tasks);

        }

        [Fact]
        public async Task ApiCreate()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("ms_auth_user", "admin");
            client.DefaultRequestHeaders.Add("ms_auth_secret", SecurityHelper.MD5($"admin96e79218965eb72c92a549dd5a330112admin"));
            for (int i = 0; i < 5; i++)
            {
                int rndNum = new Random().Next(20, 500);
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
                args.Add(new KeyValuePair<string, string>("SourceApp", "TestApp"));
                args.Add(new KeyValuePair<string, string>("Topic", "TestApp.Trade.TimeoutCancel"));
                args.Add(new KeyValuePair<string, string>("ContentKey", i.ToString()));
                args.Add(new KeyValuePair<string, string>("DelayTimeSpan", rndNum.ToString()));
                args.Add(new KeyValuePair<string, string>("DelayAbsoluteTime", DateTime.Now.AddSeconds(rndNum).ToString("yyyy-MM-dd HH:mm:ss")));
                args.Add(new KeyValuePair<string, string>("NotifyUrl", "http://localhost:56655/api/1.0/value/delaypost"));
                args.Add(new KeyValuePair<string, string>("NotifyDataType", "application/json"));
                args.Add(new KeyValuePair<string, string>("NotifyBody", "{ \"Posts\": [{ \"PostId\": 666, \"Title\": \"tester\", \"Content\":\"testtesttest\" }], \"BlogId\": 111, \"Url\":\"qweqrrttryrtyrtrtrt\" }"));
                HttpContent reqContent = new FormUrlEncodedContent(args);
                var response = await client.PostAsync("http://localhost:30000/api/DelayTask/Create", reqContent);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                Assert.True(response.IsSuccessStatusCode && content.Contains("\"Success\":true"));
            }
        }
    }
}
