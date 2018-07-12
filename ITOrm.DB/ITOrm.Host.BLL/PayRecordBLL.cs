using ITOrm.Host.Models;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Const;
using ITOrm.Utility.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class PayRecordBLL
    {
        public static YeepayUserBLL yeepayUserDao = new YeepayUserBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static MasgetUserBLL masgetUserDao = new MasgetUserBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static KeyValueBLL keyValueDao = new KeyValueBLL();
        public int Init(int UserId,decimal Amount,int Platform,string IP,string BankCard )
        {
            var yeepayUser = yeepayUserDao.Single("UserId=@UserId",new { UserId });
            ToolPay tp = new ToolPay(Amount, yeepayUser.Rate1, yeepayUser.Rate2, yeepayUser.Rate3, yeepayUser.Rate4, yeepayUser.Rate5);
            PayRecord model = new PayRecord();
            model.UserId = UserId;
            model.Amount = Amount;
            model.Platform = Platform;
            model.Ip = IP;
            model.BankCard = BankCard;
            model.Fee =tp.PayFee;
            model.Rate =tp.Rate1;
            model.Fee3 = tp.Rate3;
            model.DrawState = 0;//未发起
            model.WithDrawAmount = model.Amount - model.Fee;//结算金额
            model.ActualAmount = tp.ActualAmount;
            return Insert(model);
        }

        public int Init(int UbkID, decimal Amount, int Platform, string IP,int ChannelType)
        {
            var ubk = userBankCardDao.Single(UbkID);
            var users = usersDao.Single(ubk.UserId);
            //提现卡
            var ubkDraw = userBankCardDao.Single(" TypeId=0 and UserId=@UserId",new { ubk.UserId});

            int TypeId = (int)Logic.KeyValueType.支付通道管理;

            //获得通道列表
            var listChannelPay = MemcachHelper.Get<List<KeyValue>>(Constant.list_keyvalue_key + TypeId, DateTime.Now.AddDays(7), () =>
            {
                return keyValueDao.GetQuery(" typeid=@TypeId ", new { TypeId }, "order by Sort desc,CTime desc");
            });


            //var kv = keyValueDao.Single("KeyId=@ChannelType and TypeId=@TypeId", new { ChannelType , TypeId });

            var kv = listChannelPay.Find(m=>m.KeyId==ChannelType && m.TypeId==TypeId);
            int payType = 0;
            payType = (kv != null && kv.Value2 == "1") ? 1 : 0;//确定通道  积分类型

            JObject data = JObject.Parse(kv.Value);
            decimal BasicRate1 = data["Rate1"].ToDecimal();
            decimal BasicRate3 = data["Rate3"].ToDecimal();

            Logic.VipType vip = (Logic.VipType)users.VipType;
            decimal[] r = Constant.GetRate(payType, vip);
            //ToolPay tp = new ToolPay(Amount, r[0], 0, r[1], 0, 0);
            ToolPay tp = new ToolPay(Amount, r[0], r[1], BasicRate1, BasicRate3);


            //ToolPay tp = null;
            //switch ((Logic.ChannelType)ChannelType)
            //{
            //    case Logic.ChannelType.易宝:
            //        var yeepayUser = yeepayUserDao.Single("UserId=@UserId", new { ubk.UserId });
            //        tp=new ToolPay(Amount, yeepayUser.Rate1, yeepayUser.Rate2, yeepayUser.Rate3, yeepayUser.Rate4, yeepayUser.Rate5);
            //        break;
            //    case Logic.ChannelType.荣邦科技积分:
            //        var mUser = masgetUserDao.Single(" UserId=@UserId and TypeId=@ChannelType ", new { ubk.UserId, ChannelType });
            //        tp = new ToolPay(Amount, mUser.Rate1, 0, mUser.Rate3, 0, 0);
            //        break;
            //    case Logic.ChannelType.荣邦科技无积分:
            //        var mUser2 = masgetUserDao.Single(" UserId=@UserId and TypeId=@ChannelType ", new { ubk.UserId, ChannelType });
            //        tp = new ToolPay(Amount, mUser2.Rate1, 0, mUser2.Rate3, 0, 0);
            //        break;
            //    default:
            //        break;
            //}


            PayRecord model = new PayRecord();
            model.UserId = ubk.UserId;
            model.Amount = Amount;
            model.Platform = Platform;
            model.Ip = IP;
            model.BankCard = ubk.BankCard;
            model.Fee = tp.PayFee;
            model.Rate = tp.Rate1;
            model.Fee3 = tp.Rate3;
            model.DrawState = 0;//未发起
            model.WithDrawAmount = model.Amount - model.Fee;//结算金额
            model.ActualAmount = tp.ActualAmount;
            model.DrawBankCard = ubkDraw.BankCard;
            model.BankCode = ubk.BankCode;
            model.PayerName = users.RealName;
            model.PayerPhone = ubk.Mobile;
            model.ChannelType = ChannelType;
            model.Income = tp.Income;
            model.DrawIncome = tp.Rate3 - BasicRate3;//结算收益
            return Insert(model);
        }


        public bool UpdateState(int Id,int State ,string meesgage)
        {

            PayRecord pay = Single(Id);
            pay.UTime = DateTime.Now;
            pay.PayTime = DateTime.Now;
            pay.State = State;
            pay.Message =string.IsNullOrEmpty(pay.Message)? meesgage:pay.Message;
            return Update(pay);
        }

        public bool UpdateDrawState(int Id, int DrawState)
        {
            PayRecord pay = Single(Id);
            pay.UTime = DateTime.Now;
            pay.HandleTime = DateTime.Now;
            pay.DrawState = DrawState;
            return Update(pay);
        }
    }
}
