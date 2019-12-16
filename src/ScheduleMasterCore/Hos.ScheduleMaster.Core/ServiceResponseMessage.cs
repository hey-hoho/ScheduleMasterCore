using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hos.ScheduleMaster.Core
{
    public class ServiceResponseMessage
    {
        public ServiceResponseMessage() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        public ServiceResponseMessage(ResultStatus status, string msg = "", object data = null)
        {
            this.Status = status;
            this.Message = msg;
            this.Data = data;
        }

        /// <summary>
        /// 成功标记
        /// </summary>
        public bool Success { get => (int)Status == 1; set { } }

        /// <summary>
        /// 返回状态码
        /// </summary>
        public ResultStatus Status { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回的数据
        /// </summary>
        public object Data { get; set; }
    }

    public enum ResultStatus
    {
        /// <summary>
        /// 服务异常
        /// </summary>
        [Description("服务异常")]
        ServiceError = -2,

        /// <summary>
        /// 非法请求
        /// </summary>
        [Description("非法请求")]
        Illegal = -1,

        /// <summary>
        /// 请求失败
        /// </summary>
        [Description("请求失败")]
        Failed = 0,

        /// <summary>
        /// 请求成功
        /// </summary>
        [Description("请求成功")]
        Success = 1,

        /// <summary>
        /// 登录失败
        /// </summary>
        [Description("登录失败")]
        UnAuthorize = 2,

        /// <summary>
        /// 参数异常
        /// </summary>
        [Description("参数异常")]
        ParamError = 3,

        /// <summary>
        /// 数据异常
        /// </summary>
        [Description("数据异常")]
        DataException = 4,

        /// <summary>
        /// 验证失败
        /// </summary>
        [Description("验证失败")]
        ValidateError = 5
    }
}
