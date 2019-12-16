using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hos.ScheduleMaster.Web.Extension
{
    /// <summary>
    /// 用json.net重写一个ControllerResult
    /// </summary>
    public class JsonNetResult : ActionResult
    {
        // 构造函数
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                //忽略掉循环引用         
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //设置时间格式
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                //不使用驼峰样式的key
                ContractResolver = new DefaultContractResolver()
            };
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("ActionContext");
            }
            HttpResponse response = context.HttpContext.Response;
            response.ContentType = "application/json";
            if (this.Data != null)
            {
                response.WriteAsync(JsonConvert.SerializeObject(Data, Settings));
            }
        }

        public object Data { get; set; }

        public JsonSerializerSettings Settings { get; private set; }

    }

    /// <summary>
    /// 基于JsonNetResult直接在controller中返回json的扩展方法
    /// </summary>
    public static class JsonNetResultExtension
    {
        public static JsonNetResult JsonNet(this Controller controller, bool success, string msg = "", string url = "", object data = null)
        {
            return JsonNet(new { Success = success, Message = msg, Url = url, Data = data });
        }

        public static JsonNetResult JsonNet(this Controller controller, object data)
        {
            return JsonNet(data);
        }

        /// <summary>
        /// 创建JsonNetResult对象，输出json数据到客户端
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="encoding"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        private static JsonNetResult JsonNet(object data)
        {
            return new JsonNetResult()
            {
                Data = data
            };
        }
    }
}