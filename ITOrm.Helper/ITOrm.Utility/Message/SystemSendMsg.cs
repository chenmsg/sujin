using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Utility.Const;
using ITOrm.Utility.Log;
using ITOrm.Core.Helper;
using System.Net;
using System.IO;
using System.Web;

namespace ITOrm.Utility.Message
{
    public static class SystemSendMsg
    {


        public static ReturnMsgModel Send(Logic.EnumSendMsg type, string mobile, decimal amount= 0M)
        {
            var result = new ReturnMsgModel();
            result.Merchant = Constant.Merchant;
            result.backState = false;
            result.relationId = "";
            result.Mobile = mobile;
            switch (type)
            {
                case Logic.EnumSendMsg.注册短信:
                    result.code =  Constant.IsDebug ? "111111" : GetCode(6);
                    result.content = $"欢迎注册速金派，验证码：{result.code}。";
       
                    break;
                case Logic.EnumSendMsg.忘记密码短信:
                    result.code = Constant.IsDebug ? "111111" : GetCode(6);
                    result.content = $"您正在进行找回密码操作，验证码：{result.code}。";
                    break;
                case Logic.EnumSendMsg.自动处理程序出错:
                    result.code = Constant.IsDebug ? "111111" : GetCode(6);
                    result.content = $"自动处理程序出错,请尽快处理，出错时间:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                    break;
                case Logic.EnumSendMsg.腾付通收银短信:
                    result.code = Constant.IsDebug ? "111111" : GetCode(6);
                    result.content = $"您正在进行收款交易￥{amount.ToString("F2")}，验证码：{result.code},请勿泄漏给他人。";
                    break;
                default:
                    break;
            }
            BackModel back = null;
            if (Constant.IsDebug)
            {
                back = new BackModel() { State = "0", MsgState = "发送成功" };
            }
            else
            {
                back = sendensms(mobile, result.content);
            }
            result.backState = back.State == "0";
            result.msg = back.MsgState;
            return result;
        }

        public class BackModel
        {
            public string State
            {
                get;
                set;
            }
            public string MsgID
            {
                get;
                set;
            }
            public string MsgState
            {
                get;
                set;
            }
            public string Reserve
            {
                get;
                set;
            }
        }


        public static string GetCode(int num)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Random r = new Random();
            for (int i = 0; i < num; i++)
            {
                sb.Append(r.Next(0, 10));
            }
            //"验证码：" +
            return sb.ToString();
        }

        public readonly static string qianming = "【速金派】";
        //盛世云商短信通道
        public static string ensmsUrl = ConfigHelper.GetAppSettings("ensmsUrl");
        public static string ensmsname = ConfigHelper.GetAppSettings("ensmsname");
        public static string ensmspwd = ConfigHelper.GetAppSettings("ensmspwd");

        public static BackModel sendensms(string tel, string msg)
        {
            msg = qianming + msg;
            BackModel backmodel = new BackModel();
            backmodel.State = "1";
            backmodel.MsgID = "0";
            backmodel.MsgState = "发送失败";
            backmodel.Reserve = "";
            string ret = string.Empty;
            try
            {
              

                Logs.WriteLog("短信接口对应手机号：" + tel + ",提交信息：" + msg, "d:\\Log\\SystemSendMsg", "SendSms");
                //BackModel backmodel = new BackModel();
                string postStrTpl = "username={0}&pwd={1}&dt={2}&msg={3}&mobiles={4}&code={5}";
                Int64 dt = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                string pwdmd5 = Encryption.SecurityHelper.GetMD5String(ensmsname + ensmspwd + dt);
                UTF8Encoding encoding = new UTF8Encoding();

                var body = string.Format(postStrTpl, ensmsname, pwdmd5, dt, HttpUtility.UrlEncode(msg), tel, "1028");
                StringBuilder requestStringUri = new StringBuilder();
                requestStringUri.Append(ensmsUrl);
                requestStringUri.AppendFormat("?{0}", body);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestStringUri.ToString());
                request.Method = "GET";
                request.KeepAlive = false;
                using (WebResponse response = request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream()
                        , Encoding.GetEncoding("utf-8")
                        );
                    ret = reader.ReadToEnd();
                    Logs.WriteLog("短信接口对应手机号：" + tel + ",返回信息：" + ret, "d:\\Log\\SystemSendMsg", "SendSms");
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                backmodel.MsgState = ex.Message;
                Logs.WriteLog("短信接口对应手机号：" + tel + ",返回信息：" + ex.Message, "d:\\Log\\SystemSendMsg", "SendSms");
            }
            if (ret == "0")
            {
                backmodel.State = "0";
                backmodel.MsgState = "发送成功";
            }
            return backmodel;
        }
    }


    public class ReturnMsgModel
    {
        /// <summary>
        /// 发送状态
        /// </summary>
        public bool backState { get; set; }
        /// <summary>
        /// 提示消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public string relationId { get; set; }

        /// <summary>
        /// 发送内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        ///短信服务商
        /// </summary>
        public int Merchant { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

    }
}
