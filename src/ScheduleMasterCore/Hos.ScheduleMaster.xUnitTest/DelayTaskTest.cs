using Hos.ScheduleMaster.QuartzHost.DelayedTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task TestCallback()
        {
            //初始化容器
            DelayPlanManager.Init();
            NotifyPlan plan = new NotifyPlan
            {
                Key = "84ecb87b-736e-4bc4-8a5f-98c258373aad",
                NotifyBody = "{ \"Posts\": [{ \"PostId\": 666, \"Title\": \"tester\", \"Content\":\"testtesttest\" }], \"BlogId\": 111, \"Url\":\"qweqrrttryrtyrtrtrt\" }",
                NotifyDataType = "application/json",
                NotifyUrl = "http://localhost:56655/api/1.0/value/delaypost",
            };
            await DelayPlanManager.NotifyExecutedEvent(plan);
        }
    }
}
