using ITOrm.Core.Helper;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Payment.Yeepay;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ITOrm.Utility.Const;
using ITOrm.Utility.Log;
using Newtonsoft.Json;
using ITOrm.Utility.Message;
using Newtonsoft.Json.Linq;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Helper;
using ITOrm.Payment.Masget;
using ITOrm.Payment.Teng;
using ITOrm.Payment.MiShua;
using ITOrm.Payment.Const;

namespace ITOrm.AutoService
{
    public class Operation
    {
        private enum RunNing
        {
            运行中 = 1,
            暂停等待中 = 2,
            已暂停 = 3,
            运行等待中 = 4,
        }

        private RunNing runState = RunNing.已暂停;
        private Thread _Thread = null;

        public bool IsRunning
        {
            get { return _Thread != null; }
        }

        Button _StartButton;


        //图片自动删除
        private TextBox _tbxImggeSuccess;
        private TextBox _tbxImggeFail;


        //自动审核
        private TextBox _tbxAuditSuccess;
        private TextBox _tbxAuditFail;


        //自动设置费率
        private TextBox _tbxSetRateSuccess;
        private TextBox _tbxSetRateFail;

        //自动设置费率
        private TextBox _tbxWithDrawApiSuccess;
        private TextBox _tbxWithDrawApiFail;

        //定时任务
        private TextBox _tbxTimedTaskSuccess;
        private TextBox _tbxTimedTaskFail;

        //资金队列
        private TextBox _tbxAccountQueueSuccess;
        private TextBox _tbxAccountQueueFail;

        //结算检查
        private TextBox _tbxWithDrawSuccess;
        private TextBox _tbxWithDrawFail;

        //开始时间
        TextBox _StartTime;

        private void AddOne(TextBox o)
        {
            o.Text = (Convert.ToInt32(o.Text) + 1).ToString();
        }




        public void Start(
            Button StartButton
            //图片自动删除
            , TextBox tbxImggeSuccess
            , TextBox tbxImggeFail

            //自动审核
            , TextBox tbxAuditSuccess
            , TextBox tbxAuditFail

            //自动设置费率
            , TextBox tbxSetRateSuccess
            , TextBox tbxSetRateFail

            //自动结算
            , TextBox tbxWithDrawApiSuccess
            , TextBox tbxWithDrawApiFail

            //定时任务
            , TextBox tbxTimedTaskSuccess
            , TextBox tbxTimedTaskFail

            //资金队列
            , TextBox tbxAccountQueueSuccess
            , TextBox tbxAccountQueueFail
            
            //结算检查
            , TextBox tbxWithDrawSuccess
            , TextBox tbxWithDrawFail
            //开始时间
            , TextBox StartTime
            )
        {
            _StartButton = StartButton;
            //图片自动删除
            _tbxImggeSuccess = tbxImggeSuccess;
            _tbxImggeFail = tbxImggeFail;

            //自动审核
            _tbxAuditSuccess = tbxAuditSuccess;
            _tbxAuditFail = tbxAuditFail;

            //自动设置费率
            _tbxSetRateSuccess = tbxSetRateSuccess;
            _tbxSetRateFail = tbxSetRateFail;

            //自动结算
            _tbxWithDrawApiSuccess = tbxWithDrawApiSuccess;
            _tbxWithDrawApiFail = tbxWithDrawApiFail;

            //定时任务
            _tbxTimedTaskSuccess = tbxTimedTaskSuccess;
            _tbxTimedTaskFail = tbxTimedTaskFail;

            //资金队列
            _tbxAccountQueueSuccess = tbxAccountQueueSuccess;
            _tbxAccountQueueFail = tbxAccountQueueFail;
            //结算检查
            _tbxWithDrawSuccess = tbxWithDrawSuccess;
            _tbxWithDrawFail = tbxWithDrawFail;
            //开始时间
            _StartTime = StartTime;

            _StartTime.Text = DateTime.Now.ToString();
            switch (runState)
            {
                case RunNing.已暂停:
                    runState = RunNing.运行中;
                    _StartButton.Text = "运行中……（可暂停）";
                    if (_Thread == null)
                    {
                        _Thread = new Thread(new ThreadStart(Run));
                        _Thread.IsBackground = true;
                        _Thread.Start();
                    }
                    break;
                case RunNing.运行中:
                    runState = RunNing.暂停等待中;
                    _StartButton.Text = RunNing.暂停等待中.ToString();
                    _StartButton.Enabled = false;
                    break;
                case RunNing.运行等待中:
                    _Thread.Abort();
                    break;
                case RunNing.暂停等待中:
                    break;
            }
        }

        private void Run()
        {
            Log objL = new Log();
            try
            {



                while (runState == RunNing.运行中)
                {

                    //处理图片 
                    UserImageHandle();
                    //处理审核
                    AuditHandle();
                    //处理费率
                    SetFeeHandle();
                    //结算
                    WithDrawApiHandle();
                    //定时任务
                    TimedTaskHandle();
                    //资金队列
                    AccountQueueHandle();
                    //结算检查
                    WithDrawHandle();
                    Thread.Sleep(1000);
                    if (runState == RunNing.暂停等待中)
                    {
                        this.runState = RunNing.已暂停;
                        _StartButton.Text = "开始";
                        _StartButton.Enabled = true;
                        break;
                    }
                    runState = RunNing.运行等待中;
                    int i = ConfigInfo.BatchWaitMilliSecond;
                    while (i > 0 && runState == RunNing.运行等待中)
                    {
                        _StartButton.Text = "运行等待中……" + i / 1000 + "秒（可暂停）";
                        Thread.Sleep(1000);
                        i = i - 1000;
                    }
                    runState = RunNing.运行中;
                    _StartButton.Text = "运行中……（可暂停）";
                    _StartButton.Enabled = true;
                }
            }
            catch (ThreadAbortException)
            {
                runState = RunNing.已暂停;
                _StartButton.Text = "开始";
            }
            catch (Exception e)
            {
                objL.fLog(e.ToString());

                string[] ss = ConfigInfo.GetMobile.Split(',');
                foreach (var s in ss)
                {
                    //发送短信
                    var resultMsg = SystemSendMsg.Send(Logic.EnumSendMsg.自动处理程序出错, s);
                    SendMsg model = new SendMsg();
                    model.TypeId = (int)Logic.EnumSendMsg.自动处理程序出错;
                    model.Context = resultMsg.content;
                    model.CTime = DateTime.Now;
                    model.IP = ITOrm.Utility.Client.Ip.GetClientIp();
                    model.Merchant = resultMsg.Merchant;
                    model.Mobile = s;
                    model.Platform = (int)Logic.Platform.系统;
                    model.Service = "sys_auto";
                    model.RelationId = resultMsg.relationId;
                    model.State = resultMsg.backState ? 2 : 1;
                    model.UTime = DateTime.Now;
                    int result = sendMsgDao.Insert(model);

                    //SMShelper.SendMsg(s, ConfigInfo.merchantCode + "存管自动处理程序异常停止！");
                }
                runState = RunNing.已暂停;
                _StartButton.Text = "开始";
            }
            finally
            {
                _Thread = null;
            }
        }
        public static UsersBLL usersDao = new UsersBLL();
        public static UserImageBLL userImageDao = new UserImageBLL();
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static SendMsgBLL sendMsgDao = new SendMsgBLL();
        public static PayRecordBLL payRecordDao = new PayRecordBLL();
        public static TimedTaskBLL timedTaskDao = new TimedTaskBLL();
        public static KeyValueBLL keyValueDao = new KeyValueBLL();
        public static AccountQueueBLL accountQueueDao = new AccountQueueBLL();
        public static ViewPayRecordBLL viewPayRecordDao = new ViewPayRecordBLL();

        private List<UserImage> listUserImage = null;
        private List<YeepayUser> listAudit = null;
        private List<YeepayUser> listSetFee = null;
        private List<PayRecord> listPayRecord = null;
        private List<TimedTask> listTimedTask = null;
        private List<AccountQueue> listAccountQueue = null;
        private List<ViewPayRecord> listViewPayRecord = null;

        #region 处理图片
        private bool UserImageHandle()
        {
            bool flag = false;
            if (listUserImage != null && listUserImage.Count > 0)
            {
                while (listUserImage.Count > 0)
                {
                    var item = listUserImage[0];
                    item.State = -1;
                    item.UTime = DateTime.Now;
                    bool uf = userImageDao.Update(item);
                    Logs.WriteLog($"Id:{item.ID}", "d:\\Log\\自动处理", "自动删除图片"+(uf?"成功":"失败"));
                    //移除数据
                    if (uf)
                    {
                        int num = Convert.ToInt32(_tbxImggeSuccess.Text);
                        num++;
                        _tbxImggeSuccess.Text = num.ToString();
                     
                    }
                    else
                    {
                    
                        int num = Convert.ToInt32(_tbxImggeFail.Text);
                        num++;
                        _tbxImggeFail.Text = num.ToString();
                    }
                    listUserImage.Remove(item);
                    flag = uf;
                    Thread.Sleep(ConfigInfo.theadTime);
                }
            }
            else
            {
                listUserImage = userImageDao.GetQuery(10, " State=0 and DATEDIFF(DAY,CTime,GETDATE())>30 ", null, "order by id asc");
                if (listUserImage!=null&&listUserImage.Count > 0)
                {
                    return UserImageHandle();
                }
            }
            return flag;
        }
        #endregion

        #region 商户审核
        private bool AuditHandle()
        {
            bool flag = false;
            if (listAudit != null && listAudit.Count > 0)
            {
                while (listAudit.Count > 0)
                {
                    var item = listAudit[0];
                    //审核
                    var result= YeepayDepository.AuditMerchant(item.UserId, (int)Logic.Platform.系统,ITOrm.Payment.Yeepay.Enums.AuditMerchant.SUCCESS, "审核成功");
                    Logs.WriteLog($"处理ID:{item.ID},UserId:{item.UserId},结果json:{JsonConvert.SerializeObject(result)}", "d:\\Log\\自动处理", string.Format("商户审核{0}", result.backState==0?"成功":"失败"));
                    if (result.backState == 0)
                    {
                        int num = Convert.ToInt32(_tbxAuditSuccess.Text);
                        num++;
                        _tbxAuditSuccess.Text = num.ToString();
                 
                    }
                    else
                    {
                        int num = Convert.ToInt32(_tbxAuditFail.Text);
                        num++;
                        _tbxAuditFail.Text = num.ToString();
                    }
                    listAudit.Remove(item);
                    Thread.Sleep(ConfigInfo.theadTime);
                }
            }
            else
            {
                listAudit = yeepayUserDao.GetQuery(10, " IsAudit=0 ", null, "order by id asc");
                if (listAudit != null && listAudit.Count > 0)
                {
                    return AuditHandle();
                }
            }
            return flag;
        }
        #endregion

        #region 商户设置费率
        private bool SetFeeHandle()
        {
            bool flag = false;
            if (listSetFee != null && listSetFee.Count > 0)
            {
                while (listSetFee.Count > 0)
                {
                    var item = listSetFee[0];
                    var user = usersDao.Single(item.UserId);
                    Logic.VipType vip = (Logic.VipType)user.VipType;
                    decimal rate1 = 0.0050M;
                    decimal rate3 = 2M;

                    var r= Constant.GetRate((int)Logic.PayType.积分,vip);
                    rate1 = r[0];
                    rate3 = r[1];
                    //审核
                    var result1 = YeepayDepository.FeeSetApi(item.UserId, (int)Logic.Platform.系统, ITOrm.Payment.Yeepay.Enums.YeepayType.设置费率1, rate1.ToString("F4"));
                    if (vip == Logic.VipType.顶级代理)
                    {
                        var result3 = YeepayDepository.FeeSetApi(item.UserId, (int)Logic.Platform.系统, ITOrm.Payment.Yeepay.Enums.YeepayType.设置费率3, rate3.ToString("F0"));
                    }
                    var result4 = YeepayDepository.FeeSetApi(item.UserId, (int)Logic.Platform.系统, ITOrm.Payment.Yeepay.Enums.YeepayType.设置费率4, "0");
                    var result5 = YeepayDepository.FeeSetApi(item.UserId, (int)Logic.Platform.系统, ITOrm.Payment.Yeepay.Enums.YeepayType.设置费率5, "0");
                    Logs.WriteLog($"处理ID:{item.ID},UserId:{item.UserId},设置费率1:{JsonConvert.SerializeObject(result1)},设置费率4:{JsonConvert.SerializeObject(result4)},设置费率5:{JsonConvert.SerializeObject(result5)}", "d:\\Log\\自动处理", string.Format("商户设置费率{0}",(result1.backState == 0 && result4.backState == 0 && result5.backState == 0)?"成功":"失败"));
                    if (result1.backState == 0&& result4.backState==0 && result5.backState==0)
                    {
                        int num = Convert.ToInt32(_tbxSetRateSuccess.Text);
                        num++;
                        _tbxSetRateSuccess.Text = num.ToString();
                       
                    }
                    else
                    {
                        int num = Convert.ToInt32(_tbxSetRateFail.Text);
                        num++;
                        _tbxSetRateFail.Text = num.ToString();
                    }
                    listSetFee.Remove(item);
                    Thread.Sleep(ConfigInfo.theadTime);
                }
            }
            else
            {
                listSetFee = yeepayUserDao.GetQuery(10, " IsAudit=1 and DATEDIFF(ss,UTime,GETDATE())>3 AND (RateState1=0 OR RateState4=0 OR RateState5=0)  ", null, "order by id asc");
                if (listSetFee != null && listSetFee.Count > 0)
                {
                    return SetFeeHandle();
                }
            }
            return flag;
        }

        #endregion

        #region 自动结算
        public bool WithDrawApiHandle()
        {
            bool flag = false;
            if (listPayRecord != null && listPayRecord.Count > 0)
            {
                while (listPayRecord.Count > 0)
                {
                    var item = listPayRecord[0];
                    //结算
                    var result = YeepayDepository.WithDrawApi(item.ID,(int)Logic.Platform.系统);
                    if (result.backState != 0)//再次提交一次
                    {
                        Thread.Sleep(500);
                        result = YeepayDepository.WithDrawApi(item.ID, (int)Logic.Platform.系统);
                    }
                    Logs.WriteLog($"处理ID:{item.ID},UserId:{item.UserId},结果json:{JsonConvert.SerializeObject(result)}", "d:\\Log\\自动处理", string.Format("自动结算{0}", (result.backState == 0 ) ? "成功" : "失败"));
                    if (result.backState == 0 )
                    {
                        int num = Convert.ToInt32(_tbxWithDrawApiSuccess.Text);
                        num++;
                        _tbxWithDrawApiSuccess.Text = num.ToString();

                    }
                    else
                    {
                        int num = Convert.ToInt32(_tbxWithDrawApiFail.Text);
                        num++;
                        _tbxWithDrawApiFail.Text = num.ToString();
                    }
                    listPayRecord.Remove(item);
                    Thread.Sleep(ConfigInfo.theadTime);
                }
            }
            else
            {
                listPayRecord = payRecordDao.GetQuery(10, " State=10 AND DrawState=0 AND ChannelType=0 ", null, "order by id asc");
                if (listPayRecord != null && listPayRecord.Count > 0)
                {
                    return WithDrawApiHandle();
                }
            }
            return flag;
        }
        #endregion

        #region 定时任务
        private bool TimedTaskHandle()
        {
            bool flag = false;
            if (listTimedTask != null && listTimedTask.Count > 0)
            {
                while (listTimedTask.Count > 0)
                {
                    var item = listTimedTask[0];
                    //任务类型
                    Logic.TimedTaskType task = (Logic.TimedTaskType)item.TypeId;
                    switch (task)
                    {
                        case Logic.TimedTaskType.通道开启:
                            #region 开启通道 
                            int keyId = JObject.Parse(item.Value)["keyId"].ToInt();
                            KeyValue kv = keyValueDao.Single(keyId);
                            kv.UTime = DateTime.Now;
                            kv.State = 0;
                            kv.Remark = $"{DateTime.Now.ToString()}自动开启";
                            bool kvFlag = keyValueDao.Update(kv);
                            MemcachHelper.Delete(Constant.list_keyvalue_key + (int)Logic.KeyValueType.支付通道管理);//清理缓存
                            Logs.WriteLog($"处理ID:{item.ID},flag:{kvFlag}任务数据：{JsonConvert.SerializeObject(item)}", "d:\\Log\\自动处理", "定时任务");
                            #endregion
                            item.State = kvFlag ? 1 : -1;
                            break;
                        default:
                            Logs.WriteLog($"未处理ID:{item.ID},任务数据：{JsonConvert.SerializeObject(item)}", "d:\\Log\\自动处理", "定时任务");
                            item.State =  -1;
                            break;
                    }
                    #region 任务状态更新
                    item.UTime = DateTime.Now;
                    flag= timedTaskDao.Update(item);
                    #endregion

                    if (flag)
                    {
                        int num = Convert.ToInt32(_tbxTimedTaskSuccess.Text);
                        num++;
                        _tbxTimedTaskSuccess.Text = num.ToString();

                    }
                    else
                    {
                        int num = Convert.ToInt32(_tbxTimedTaskFail.Text);
                        num++;
                        _tbxTimedTaskFail.Text = num.ToString();
                    }
                    listTimedTask.Remove(item);
                    Thread.Sleep(ConfigInfo.theadTime);
                }
            }
            else
            {
                listTimedTask = timedTaskDao.GetQuery(10, " State=0 and getdate()>ExecTime ", null, "order by id asc");
                if (listTimedTask != null && listTimedTask.Count > 0)
                {
                    return TimedTaskHandle();
                }
            }
            return flag;
        }
        #endregion

        #region 资金队列
        private bool AccountQueueHandle()
        {
            bool flag = false;
            if (listAccountQueue != null && listAccountQueue.Count > 0)
            {
                while (listAccountQueue.Count > 0)
                {
                    var item = listAccountQueue[0];
                    //处理逻辑
                   var result= accountQueueDao.AccountQueueHandle(item.ID);
                    Logs.WriteLog($"处理数据：{JsonConvert.SerializeObject(result)}", "d:\\Log\\自动处理", "资金队列");
                    if (result.backState==0)
                    {
                        int num = Convert.ToInt32(_tbxAccountQueueSuccess.Text);
                        num++;
                        _tbxAccountQueueSuccess.Text = num.ToString();
                    }
                    else
                    {
                        int num = Convert.ToInt32(_tbxAccountQueueFail.Text);
                        num++;
                        _tbxAccountQueueFail.Text = num.ToString();
                    }
                    listAccountQueue.Remove(item);
                    Thread.Sleep(ConfigInfo.theadTime);
                }
            }
            else
            {
                listAccountQueue = accountQueueDao.GetQuery(10, " State=0  ", null, "order by id asc");
                if (listAccountQueue != null && listAccountQueue.Count > 0)
                {
                    return AccountQueueHandle();
                }
            }
            return flag;
        }
        #endregion

        #region 结算检查
        private bool WithDrawHandle()
        {
            bool flag = false;
            if (listViewPayRecord != null && listViewPayRecord.Count > 0)
            {
                while (listViewPayRecord.Count > 0)
                {
                    var item = listViewPayRecord[0];

                    Logic.ChannelType Channel = (Logic.ChannelType)item.ChannelType;
                    ResultModel result = new ResultModel();
                    result.backState = -100;
                    string msg = "";
                    switch (Channel)
                    {
                        case Logic.ChannelType.易宝:
                            msg = "支付失败";
                            var yeepayResult = YeepayDepository.TradeReviceQuery(item.RequestId.ToString(),(int)Logic.Platform.系统);
                            if (yeepayResult.backState == 0 && yeepayResult.tradeReceives.Count > 0 && yeepayResult.tradeReceives[0].status == "SUCCESS")
                            {
                                result.backState = 0;
                                msg = "支付成功";
                            }
                            break;
                        case Logic.ChannelType.荣邦科技积分:
                        case Logic.ChannelType.荣邦3:
                            msg = "支付失败";
                            var masgettResult = MasgetDepository.PaymentjournalGet(item.RequestId, (int)Logic.Platform.系统,Channel);
                            if (masgettResult.backState == 0 && masgettResult.data.respcode == 2)
                            {
                                result.backState = 0;
                                msg = "支付成功";
                            }
                            break;
                        case Logic.ChannelType.荣邦科技无积分:
                            break;
                        case Logic.ChannelType.腾付通:
                            var TengResult = TengDepository.PayDebitQuery(item.RequestId, (int)Logic.Platform.系统);
                            if (TengResult.backState == 0 && TengResult.status == "3")
                            {
                                result.backState = 0;
                                msg = "支付成功";
                            }
                            else
                            {
                                msg = TengResult.respMsg;
                            }
                            break;
                        case Logic.ChannelType.米刷:
                            var mishuaResult = MiShuaDepository.CheckDzero(item.RequestId, Logic.Platform.系统);
                            if (mishuaResult.backState == 0 && mishuaResult.Data.status == "00" && mishuaResult.Data.qfStatus == "SUCCESS")
                            {
                                result.backState = 0;
                                msg = "支付成功";
                            }
                            break;
                        default:
                            break;
                    }
                    bool f = false;
                    //Logs.WriteLog($"处理数据：{JsonConvert.SerializeObject(result)}", "d:\\Log\\自动处理", "资金队列");
                    //处理数据
                    f = payRecordDao.UpdateState(item.ID, result.backState == 0 ? 10 : -1, msg);
                    if (f)
                    {
                        //交易成功回调
                        UsersDepository.NoticeSuccess(item.ID, item.UserId);

                        int num = Convert.ToInt32(_tbxWithDrawSuccess.Text);
                        num++;
                        _tbxWithDrawSuccess.Text = num.ToString();
                    }
                    else
                    {
                        int num = Convert.ToInt32(_tbxWithDrawFail.Text);
                        num++;
                        _tbxWithDrawFail.Text = num.ToString();
                    }


                    listViewPayRecord.Remove(item);
                    Thread.Sleep(ConfigInfo.theadTime);
                }
            }
            else
            {
                //两小时前的数据被处理
                listViewPayRecord = viewPayRecordDao.GetQuery(10, " State not in(10,-1) and DATEDIFF(HOUR,CTime,GETDATE())>2  ", null, "order by id asc");
                if (listViewPayRecord != null && listViewPayRecord.Count > 0)
                {
                    return WithDrawHandle();
                }
            }
            return flag;
        }
        #endregion
    }
}
