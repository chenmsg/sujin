using System;
using System.Text;
using System.Net.Mail;
using ITOrm.Core.Helper;
using ITOrm.Core.Security;
using ITOrm.Core.Utility.Extensions;

namespace ITOrm.Core.Utility.Helper
{
    /// <summary>
    /// Email助手
    /// </summary>
    public class EmailHelper
    {
        #region 发送邮件

        /// <summary>
        /// 发送邮件,无符件,使用HTML编码
        /// </summary>
        /// <param name="to">接收邮件用户列表,以分号连接</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <returns>是否发送成功</returns>
        public static bool SendMail(string to, string subject, string body)
        {
            return SendMail(to, subject, body, string.Empty, true);
        }

        /// <summary>
        /// 发送邮件,有符件,使用HTML编码
        /// </summary>
        /// <param name="to">接收邮件用户列表,以分号连接</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="attach">符件路径 如:C:\\附件.rar</param>
        /// <returns>是否发送成功</returns>
        public static bool SendMail(string to, string subject, string body, string attach)
        {
            return SendMail(to, subject, body, attach, true);
        }

        /// <summary>
        /// 使用System.Net.Mail的发送邮件.
        /// </summary>
        /// <param name="to">接收邮件用户列表,以分号连接</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="attach">符件</param>
        /// <param name="useHtmlFormat">是否为HTML编码发送</param>
        /// <returns>是否发送成功</returns>
        private static bool SendMail(string to, string subject, string body, string attach, bool useHtmlFormat)
        {
            string from = ConfigHelper.GetAppSettings("FromAddress");
            //邮件发送者的名称
            string FromName = ConfigHelper.GetAppSettings("FromName");
            string sendEmailPassword = ConfigHelper.GetAppSettings("FromPwd");

            MailMessage mailMsg = new MailMessage();
            string[] mails = to.Split(';');
            foreach (string mail in mails)
            {
                if (!String.IsNullOrEmpty(mail))
                {
                    mailMsg.To.Add(mail);
                }
            }
            mailMsg.SubjectEncoding = Encoding.Default;
            mailMsg.BodyEncoding = Encoding.Default;
            mailMsg.From = new MailAddress(from, FromName);
            mailMsg.Subject = subject;
            mailMsg.Body = body;
            if (attach != null && attach != "")
            {
                char[] delim = new char[] { ',' };
                foreach (string sSubstr in attach.Split(delim))
                {
                    //Attachment("C:\\附件.rar");
                    //System.Net.Mail.Attachment MyAttachment = new System.Net.Mail.Attachment(sSubstr);
                    //mailMsg.Attachments.Add(MyAttachment);
                    Attachment attachment = new Attachment(sSubstr, "text/plain");
                    mailMsg.Attachments.Add(attachment);

                }
            }
            if (useHtmlFormat)
                mailMsg.IsBodyHtml = true;
            else
                mailMsg.IsBodyHtml = false;
            SmtpClient client = new SmtpClient();
            //smtp.163.com
            client.Host = ConfigHelper.GetAppSettings("FromSmtp");
            //端口通常是25
            client.Port = Convert.ToInt32(ConfigHelper.GetAppSettings("FromSmtpProt"));
            client.UseDefaultCredentials = false;
            //不需要做超时设置,否则经常发不了邮件,net有自已的超时机制.不设只随时都能发送成功的
            //client.Timeout = 60;
            #region Email
            client.Credentials = new System.Net.NetworkCredential(from, sendEmailPassword);
            #endregion
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(mailMsg);
                return true;
            }
            catch (Exception ex)
            {
                Exception source = ex;
                while (source.InnerException != null)
                    source = source.InnerException;
                //XTrace.WriteLog("邮件发送失败！邮件标题：" + subject + "。错误消息：" + source.Message);
                return false;
            }
        }

        /// <summary>
        /// 使用System.Net.Mail的发送邮件.
        /// </summary>
        /// <param name="to">接收邮件用户列表,以分号连接</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="attach">符件</param>
        /// <param name="useHtmlFormat">是否为HTML编码发送</param>
        /// <param name="from">发送者邮件地址</param>
        /// <param name="fromName">发送者称谓</param>
        /// <param name="sendEmailPassword">发送者邮件密码</param>
        /// <param name="fromSmtp">邮件服务器Smtp地址，不加http://</param>
        /// <param name="fromSmtpProt">邮件服务器Smtp端口</param>
        /// <param name="timeOut">超时时间 单位 称</param>
        /// <returns></returns>
        public static bool SendMail(string to, string subject, string body, string attach, bool useHtmlFormat, string from, string fromName, string sendEmailPassword, string fromSmtp, int fromSmtpProt)
        {
            MailMessage mailMsg = new MailMessage();
            string[] mails = to.Split(';');
            foreach (string mail in mails)
            {
                if (!String.IsNullOrEmpty(mail))
                {
                    mailMsg.To.Add(mail);
                }
            }
            mailMsg.SubjectEncoding = Encoding.Default;
            mailMsg.BodyEncoding = Encoding.Default;
            mailMsg.From = new MailAddress(from, fromName);
            mailMsg.Subject = subject;
            mailMsg.Body = body;
            if (attach != null && attach != "")
            {
                char[] delim = new char[] { ',' };
                foreach (string sSubstr in attach.Split(delim))
                {
                    //Attachment("C:\\附件.rar");
                    System.Net.Mail.Attachment MyAttachment = new System.Net.Mail.Attachment(sSubstr);
                    mailMsg.Attachments.Add(MyAttachment);
                }
            }
            if (useHtmlFormat)
                mailMsg.IsBodyHtml = true;
            else
                mailMsg.IsBodyHtml = false;
            SmtpClient client = new SmtpClient();
            //mail.163.com
            client.Host = fromSmtp;
            //端口通常是25
            client.Port = fromSmtpProt;
            client.UseDefaultCredentials = false;
            //client.Timeout = timeOut;
            #region Email
            client.Credentials = new System.Net.NetworkCredential(from, sendEmailPassword);
            #endregion
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(mailMsg);
                return true;
            }
            catch (Exception ex)
            {
                Exception source = ex;
                while (source.InnerException != null)
                    source = source.InnerException;
                //XTrace.WriteLog("邮件发送失败！邮件标题：" + subject + "。错误消息：" + source.Message);
                return false;
            }
        }
        #endregion
    }
}
