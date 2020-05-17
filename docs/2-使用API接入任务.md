

为了方便业务系统更好的接入调度系统，创建任务不仅可以在控制台中实现，系统也提供了WebAPI供业务系统使用代码接入，这种方式对延时任务来说尤其重要。

<br />

### API Server 对接流程

对于开放接口来说，使用签名验证已经是必不可少的一环，这是保证系统安全性的重要手段。看一下核心对接流程：

- 在控制台中创建好专用的API对接用户账号。

- 使用对接账号的用户名设置为http header中的`ms_auth_user`值。

- 使用经过哈希运算过的秘钥设置为http header中的`ms_auth_secret值`，计算规则：按{用户名}{hash(密码)}{用户名}的格式拼接得到字符串str，然后再对str做一次hash运算即得到最终秘钥，hash函数是小写的32位MD5算法。

- 使用form格式发起http调用，如果非法用户会返回401-Unauthorized。


代码示例：
```c#
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("ms_auth_user", "admin");
    client.DefaultRequestHeaders.Add("ms_auth_secret", SecurityHelper.MD5($"admin{SecurityHelper.MD5("111111")}}admin"));
```

> 签名验证这块设计的比较简单，具体源码逻辑可以参考`Hos.ScheduleMaster.Web.Filters.AccessControlFilter`。

<br />

### API返回格式

所有接口采用统一的返回格式，字段如下：

| 参数名称  | 参数类型 | 说明 |
| ------------- | ------------- | ------------- |
| Success  | bool  | 是否成功 |
| Status  | int  | 结果状态，0-请求失败 1-请求成功 2-登录失败 3-参数异常 4-数据异常 |
| Message  | string  | 返回的消息 |
| Data  | object  | 返回的数据 |

<br />

### 创建程序集任务

- 接口地址：`http://yourip:30000/api/task/create`

- 请求类型：`POST`

- 参数格式：`application/x-www-form-urlencoded`

- 返回结果：创建成功返回任务id

- 参数列表：

| 参数名称  | 参数类型 | 是否必填 | 说明 |
| ------------- | ------------- | ------------- | ------------- |
| MetaType  | int  | 是 | 任务类型，这里固定是1 |
| Title  | string  | 是 | 任务名称 |
| RunLoop  | bool  | 是 | 是否按周期执行 |
| CronExpression  | string  | 否 | cron表达式，如果RunLoop为true则必填 |
| AssemblyName  | string  | 是 | 程序集名称 |
| ClassName  | string  | 是 | 执行类名称，包含完整命名空间 |
| StartDate  | DateTime  | 是 | 任务开始时间 |
| EndDate  | DateTime  | 否 | 任务停止时间，为空表示不限停止时间 |
| Remark  | string  | 否 | 任务描述说明 |
| Keepers  | List&lt;int&gt;  | 否 | 监护人id |
| Nexts  | List&lt;guid&gt;  | 否 | 子级任务id |
| Executors  | List&lt;string&gt;  | 否 | 执行节点名称 |
| RunNow  | bool  | 否 | 创建成功是否立即启动 |
| Params  | List&lt;ScheduleParam&gt;  | 否 | 自定义参数列表，也可以通过CustomParamsJson字段直接传json格式字符串 |

ScheduleParam：

| 参数名称  | 参数类型 | 是否必填 | 说明 |
| ------------- | ------------- | ------------- | ------------- |
| ParamKey  | string  | 是 | 参数名称 |
| ParamValue  | string  | 是 | 参数值 |
| ParamRemark  | string  | 否 | 参数说明 |


代码示例：

```c#
    HttpClient client = new HttpClient();
    List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
    args.Add(new KeyValuePair<string, string>("MetaType", "1"));
    args.Add(new KeyValuePair<string, string>("RunLoop", "true"));
    args.Add(new KeyValuePair<string, string>("CronExpression", "33 0/8 * * * ?"));
    args.Add(new KeyValuePair<string, string>("Remark", "By Xunit Tester Created"));
    args.Add(new KeyValuePair<string, string>("StartDate", DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")));
    args.Add(new KeyValuePair<string, string>("Title", "程序集接口测试任务"));
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
```

> 要提一下的是，使用API创建任务的方式不支持上传程序包，所以在任务需要启动时要确保程序包已通过其他方式上传，否则会启动失败。

<br />

### 创建HTTP任务

- 接口地址：`http://yourip:30000/api/task/create`

- 请求类型：`POST`

- 参数格式：`application/x-www-form-urlencoded`

- 返回结果：创建成功返回任务id

- 参数列表：

| 参数名称  | 参数类型 | 是否必填 | 说明 |
| ------------- | ------------- | ------------- | ------------- |
| MetaType  | int  | 是 | 任务类型，这里固定是2 |
| Title  | string  | 是 | 任务名称 |
| RunLoop  | bool  | 是 | 是否按周期执行 |
| CronExpression  | string  | 否 | cron表达式，如果RunLoop为true则必填 |
| StartDate  | DateTime  | 是 | 任务开始时间 |
| EndDate  | DateTime  | 否 | 任务停止时间，为空表示不限停止时间 |
| Remark  | string  | 否 | 任务描述说明 |
| HttpRequestUrl  | string  | 是 | 请求地址 |
| HttpMethod  | string  | 是 | 请求方式，仅支持GET\POST\PUT\DELETE |
| HttpContentType  | string  | 是 | 参数格式，仅支持application/json和application/x-www-form-urlencoded |
| HttpHeaders  | string  | 否 | 自定义请求头，ScheduleParam列表的json字符串 |
| HttpBody  | string  | 是 | 如果是json格式参数，则是对应参数的json字符串；如果是form格式参数，则是对应ScheduleParam列表的json字符串。 |
| Keepers  | List&lt;int&gt;  | 否 | 监护人id |
| Nexts  | List&lt;guid&gt;  | 否 | 子级任务id |
| Executors  | List&lt;string&gt;  | 否 | 执行节点名称 |
| RunNow  | bool  | 否 | 创建成功是否立即启动 |

代码示例：

```c#
    HttpClient client = new HttpClient();
    List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
    args.Add(new KeyValuePair<string, string>("MetaType", "2"));
    args.Add(new KeyValuePair<string, string>("RunLoop", "true"));
    args.Add(new KeyValuePair<string, string>("CronExpression", "22 0/8 * * * ?"));
    args.Add(new KeyValuePair<string, string>("Remark", "By Xunit Tester Created"));
    args.Add(new KeyValuePair<string, string>("StartDate", DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")));
    args.Add(new KeyValuePair<string, string>("Title", "Http接口测试任务"));
    args.Add(new KeyValuePair<string, string>("HttpRequestUrl", "http://localhost:56655/api/1.0/value/jsonpost"));
    args.Add(new KeyValuePair<string, string>("HttpMethod", "POST"));
    args.Add(new KeyValuePair<string, string>("HttpContentType", "application/json"));
    args.Add(new KeyValuePair<string, string>("HttpHeaders", "[]"));
    args.Add(new KeyValuePair<string, string>("HttpBody", "{ \"Posts\": [{ \"PostId\": 666, \"Title\": \"tester\", \"Content\":\"testtesttest\" }], \"BlogId\": 111, \"Url\":\"qweqrrttryrtyrtrtrt\" }"));
    HttpContent reqContent = new FormUrlEncodedContent(args);
    var response = await client.PostAsync("http://localhost:30000/api/Task/Create", reqContent);
    var content = await response.Content.ReadAsStringAsync();
    Debug.WriteLine(content);
```

<br />

### 创建延时任务

- 接口地址：`http://yourip:30000/api/delaytask/create`

- 请求类型：`POST`

- 参数格式：`application/x-www-form-urlencoded`

- 返回结果：创建成功返回任务id

- 参数列表：

| 参数名称  | 参数类型 | 是否必填 | 说明 |
| ------------- | ------------- | ------------- | ------------- |
| SourceApp  | string  | 是 | 来源 |
| Topic  | string  | 是 | 主题 |
| ContentKey  | string  | 是 | 业务关键字 |
| DelayTimeSpan  | int  | 是 | 延迟相对时间 |
| DelayAbsoluteTime  | DateTime  | 是 | 延迟绝对时间 |
| NotifyUrl  | string  | 是 | 回调地址 |
| NotifyDataType  | string  | 是 | 回调参数格式，仅支持application/json和application/x-www-form-urlencoded |
| NotifyBody  | string  | 是 | 回调参数，json格式字符串 |

代码示例：

```c#
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
    }
```