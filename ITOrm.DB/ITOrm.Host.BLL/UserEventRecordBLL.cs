using ITOrm.Host.Models;
using ITOrm.Utility.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace ITOrm.Host.BLL
{
    public partial class UserEventRecordBLL
    {

        /// <summary>
        /// 用户登录日志记录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="UserId"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool UserLogin(int cid=0, string mobile="",string password="",string ip="",int UserId=0,int State=0, string version = "",string guid="")
        {
            var model = CreateObject(cid,"Users", "Login",ip,State,UserId);
            model.Data = $"{{mobile:{mobile},password:{password},version:{version},guid:{guid}}}";
            int result= Insert(model);
            return result>0;
        }

        /// <summary>
        /// 检查用户是否可以登录
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ReturnCheckLogin UserCheckLogin(int UserId)
        {
            var model = new ReturnCheckLogin();
            model.backState = true;
            model.msg = "允许登录";
            int expires =Constant.login_locking_expires-1;

            

            //如果锁定时间范围内 有进行过找回密码，那么绕开限制
            var forgetCount = Count(" CTime>DATEADD(MINUTE,@expires,GETDATE()) and [Action]='Forget' and Cmd='UpdatePassword' and State=1 ",new { expires});
            if (forgetCount<=0)
            {
                //查询最近成功的登录日志
                var lastLoginUER = Single(" [Action]='Users' and Cmd='Login' and UserId=@UserId and CTime>DATEADD(MINUTE,@expires,GETDATE()) and State=1 Order by CTime desc", new { UserId, expires });
                //取出最近成功登录的日志ID
                int successId = (lastLoginUER != null && lastLoginUER.ID > 0) ? lastLoginUER.ID : 0;
                //查询登录成功之后 并且在锁定时间范围内的 登录失败个数 
                var result = Count(" ID>@successId and  [Action]='Users' and Cmd='Login' and UserId=@UserId and CTime>DATEADD(MINUTE,@expires,GETDATE()) and State=0 ", new { UserId, expires, successId });
                if (result >= Constant.login_locking_num)
                {
                    model.backState = false;
                    model.msg = string.Format("由于您密码错误次数过多，账户将锁定{0}小时，或找回密码", expires);
                }
            }
            return model;
        }

        /// <summary>
        /// 注册日志记录
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="ip"></param>
        /// <param name="UserId"></param>
        /// <param name="State"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="mcode"></param>
        /// <param name="regGuid"></param>
        /// <param name="baseUserId"></param>
        /// <returns></returns>
        public bool UserRegister(int cid = 0, string ip = "", int UserId = 0, int State = 0, string mobile = "", string password = "", string mcode = "", string regGuid = "", int baseUserId = 0,string version="")
        {
            var model = CreateObject(cid, "Users", "Register", ip, State, UserId);
            model.Data = $"{{mobile:{mobile},password:{password},mcode:{mcode},regGuid:{regGuid},baseUserId:{baseUserId},version:{version}}}";
            int result = Insert(model);
            return result > 0;
        }


        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ip"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool UserUpdatePassword(int cid=0, int UserId=0, string ip = "", string oldPwd = "", string newPwd = "",int State=0,string version="")
        {
            var model = CreateObject(cid,"Users", "UpdatePassword", ip, State, UserId);
            model.Data = $"{{oldPwd:{oldPwd},newPwd:{newPwd},version:{version}}}";
            int result = Insert(model);
            return result > 0;
        }


        /// <summary>
        /// 用户找回密码记录
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="State"></param>
        /// <param name="ip"></param>
        /// <param name="forgetGuid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UserForget(int cid=0,int UserId = 0,int State = 0,string ip="",string forgetGuid ="",string password="", string version = "")
        {
            var model = CreateObject(cid,"Forget", "UpdatePassword", ip, State, UserId);
            model.Data = $"{{forgetGuid:{forgetGuid},password:{password},version:{version}}}";
            int result = Insert(model);
            return result > 0;
        }


        /// <summary>
        /// 实名认证
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="ip"></param>
        /// <param name="IdCard"></param>
        /// <param name="RealName"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool RealNameAuthentication(int cid=0,int UserId = 0, string ip = "" , string IdCard = "", string RealName = "", int State = 0, string version = "")
        {
            var model = CreateObject(cid,"Users", "RealNameAuthentication", ip, State, UserId);
            model.Data = $"{{IdCard:{IdCard},RealName:{RealName},version:{version}}}";
            int result = Insert(model);
            return result > 0;
        }

        /// <summary>
        /// 收款记录
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="ip"></param>
        /// <param name="IdCard"></param>
        /// <param name="RealName"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool UserReceiveApi2(int cid = 0, int UserId = 0, string ip = "", int State = 0, string version = "",decimal Amount=0,int BankID=0,int PayType=0)
        {
            var model = CreateObject(cid, "Yeepay", "ReceiveApi2", ip, State, UserId);
            model.Data = $"{{Amount:{Amount},BankID:{BankID},PayType:{PayType},version:{version}}}";
            int result = Insert(model);
            return result > 0;
        }


        /// <summary>
        /// 请求激活银行卡快捷验证
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="ip"></param>
        /// <param name="State"></param>
        /// <param name="version"></param>
        /// <param name="BankID"></param>
        /// <param name="ChannelType"></param>
        /// <returns></returns>
        public bool BankCardActivate(int cid = 0, int UserId = 0, string ip = "", int State = 0, string version = "", int BankID = 0, int ChannelType = 0)
        {
            var model = CreateObject(cid, "Users", "BankCardActivate", ip, State, UserId);
            model.Data = $"{{ChannelType:{ChannelType},BankID:{BankID},version:{version}}}";
            int result = Insert(model);
            return result > 0;
        }


        /// <summary>
        /// 确认开通快捷协议
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="ip"></param>
        /// <param name="State"></param>
        /// <param name="version"></param>
        /// <param name="BankID"></param>
        /// <param name="ChannelType"></param>
        /// <returns></returns>
        public bool BankCardSubmitActivateCode(int cid = 0, int UserId = 0, string ip = "", int State = 0, string version = "", int BankID = 0, int ChannelType = 0, string Code = "")
        {
            var model = CreateObject(cid, "Users", "BankCardSubmitActivateCode", ip, State, UserId);
            model.Data = $"{{ChannelType:{ChannelType},BankID:{BankID},version:{version},Code:{Code}}}";
            int result = Insert(model);
            return result > 0;
        }

        /// <summary>
        /// 绑卡
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="UserId"></param>
        /// <param name="ip"></param>
        /// <param name="mobile"></param>
        /// <param name="bankcard"></param>
        /// <param name="bankcode"></param>
        /// <param name="typeid"></param>
        /// <param name="cvn2"></param>
        /// <param name="expiresYear"></param>
        /// <param name="expiresMouth"></param>
        /// <param name="OpeningBank"></param>
        /// <param name="OpeningSerialBank"></param>
        /// <param name="BankID"></param>
        /// <returns></returns>
        public bool UserBankBind(int cid = 0, int UserId = 0,string ip="",string mobile = "", string bankcard = "", string bankcode = "", int typeid = 0, string cvn2 = "", string expiresYear = "", string expiresMouth = "", string OpeningBank = "", string OpeningSerialBank = "", int BankID = 0)
        {
            var model = CreateObject(cid, "Users", "BankBind", ip, 0, UserId);
            model.Data = $"{{mobile:{mobile},bankcard:{bankcard},bankcode:{bankcode},typeid:{typeid},cvn2:{cvn2},expiresYear:{expiresYear},expiresMouth:{expiresMouth},OpeningBank:{OpeningBank},OpeningSerialBank:{OpeningSerialBank},BankID:{BankID}}}";
            int result = Insert(model);
            return result > 0;
        }


        public bool UserEventInit(int cid = 0, int UserId = 0, string ip = "",int State=0,string action="", string cmd="",string data="")
        {
            var model = CreateObject(cid, action, cmd, ip, State, UserId);
            model.Data =data;
            int result = Insert(model);
            return result > 0;
        }

        private UserEventRecord CreateObject(int cid,string Action,string Cmd,string ip,int State,int UserId)
        {
            UserEventRecord model = new UserEventRecord();
            model.PlatForm = cid;
            model.Action = Action;
            model.Cmd = Cmd;
            model.CTime = DateTime.Now;
            model.IP = ip;
            model.State = State;
            model.UserId = UserId;
            model.Data = "";
            return model;
        }
    }

    public class ReturnCheckLogin
    {
        public bool backState { get; set; }
        public string msg { get; set; }
        public DateTime Exprise { get; set; }
    }
}
