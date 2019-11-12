using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hos.ScheduleMaster.Core.Common
{
    public class MailKitHelper
    {
        /// <summary>
        ///发送邮件
        /// </summary>
        /// <param name="toAddressList">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachments">附件</param>
        /// <returns></returns>
        public static void SendMail(List<KeyValuePair<string, string>> toAddressList, string title, string content, List<KeyValuePair<string, byte[]>> attachments = null)
        {
            string server = ConfigurationCache.GetField<string>(ConfigurationCache.Email_SmtpServer);
            int port = ConfigurationCache.GetField<int>(ConfigurationCache.Email_SmtpPort);
            string account = ConfigurationCache.GetField<string>(ConfigurationCache.Email_FromAccount);
            string pwd = ConfigurationCache.GetField<string>(ConfigurationCache.Email_FromAccountPwd);

            var mailMessage = new MimeMessage();
            var fromMailAddress = new MailboxAddress("ScheduleMaster", account);
            mailMessage.From.Add(fromMailAddress);
            var toMailAddress = toAddressList.Select(x => new MailboxAddress(x.Key, x.Value));
            mailMessage.To.AddRange(toMailAddress);

            var bodyBuilder = new BodyBuilder() { HtmlBody = content };
            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    bodyBuilder.Attachments.Add(item.Key, item.Value);
                }
            }
            mailMessage.Body = bodyBuilder.ToMessageBody();
            mailMessage.Subject = title;
            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
            {
                smtpClient.Timeout = 10 * 1000;   //设置超时时间
                smtpClient.Connect(server, port, MailKit.Security.SecureSocketOptions.Auto);//连接到远程smtp服务器
                smtpClient.Authenticate(account, pwd);
                smtpClient.Send(mailMessage);//发送邮件
                smtpClient.Disconnect(true);
            }
        }
    }
}
