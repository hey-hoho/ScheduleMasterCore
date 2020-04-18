using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Hos.ScheduleMaster.Core.Common
{
    public class HttpRequest
    {
        public static KeyValuePair<HttpStatusCode, string> Send(string url, string method = "get", Dictionary<string, string> param = null, Dictionary<string, string> header = null)
        {
            Uri destination = new Uri(url);
            try
            {
                string postDataStr = string.Empty;
                if (param != null && param.Count > 0)
                {
                    if (param.Count == 1)
                    {
                        postDataStr = "\"" + param.First().Value + "\"";
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder("{");
                        foreach (var item in param)
                        {
                            sb.Append("\"" + item.Key + "\":\"" + item.Value + "\",");
                        }
                        postDataStr = sb.ToString().TrimEnd(',') + "}";
                    }
                }
                byte[] requestBytes = Encoding.GetEncoding("UTF-8").GetBytes(postDataStr);
                //   Build   the   request.   
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(destination);
                //webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentType = "application/json";
                webRequest.Method = method;
                webRequest.ContentLength = requestBytes.Length;
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        webRequest.Headers.Add(item.Key, item.Value);
                    }
                }
                if (method.ToLower().Equals("post"))
                {
                    //   Write   the   request   
                    Stream reqStream = webRequest.GetRequestStream();
                    reqStream.Write(requestBytes, 0, requestBytes.Length);
                    reqStream.Close();
                }
                //   Get   a   response   
                webRequest.Timeout = 30000;
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    if (webRequest.HaveResponse)
                    {
                        StreamReader stream = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                        string responseString = stream.ReadToEnd();
                        stream.Close();
                        return new KeyValuePair<HttpStatusCode, string>(webResponse.StatusCode, responseString);
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception($"WebExceptionStatus: {ex.Status.GetTypeCode().ToString()}，{ex.Message}");
            }
            return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.BadRequest, string.Empty);
        }
    }
}
