using ITOrm.Core.Helper;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ITOrm.Utility.Const;
using ITOrm.Utility.Client;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Helper;

namespace ITOrm.Payment.Const
{
    public class SelectOptionChannel
    {
        public static BankQuotaBLL bankQuotaDao = new BankQuotaBLL();
        public static ViewBankQuotaBLL viewBankQuotaDao = new ViewBankQuotaBLL();
        public static KeyValueBLL keyValueDao = new KeyValueBLL();
        public static UsersBLL usersDao = new UsersBLL();
        public static UserBankCardBLL userBankCardDao = new UserBankCardBLL();
        public static DebarBankChannelBLL debarBankChannelDao = new DebarBankChannelBLL();
        /// <summary>
        /// 选择最优通道
        /// 1.利润优先
        /// 2.满足额度
        /// 3.满足运营时间
        /// </summary>
        /// <param name="BankId"></param>
        /// <param name="PayType"></param>
        public static ResultModelData<int> Optimal(int UserId, decimal Amount = 0, string BankCode = "", int PayType = 0, string mobile = "", int BankID = 0)
        {


            ResultModelData<int> result = new ResultModelData<int>();
            //var bin= ITOrm.Utility.Helper.BankCardBindHelper.BankBinto(BankCard);
            //if (bin == null || string.IsNullOrEmpty(bin.BankCode))
            //{
            //    result.backState = -100;
            //    result.message = "银行卡卡Bin识别失败";
            //    return result;
            //}
            //获得通道支持的银行
            List<ViewBankQuota> listBankQuota = MemcachHelper.Get<List<ViewBankQuota>>(Constant.list_bank_quota_key, DateTime.Now.AddDays(7), () =>
           {
               return viewBankQuotaDao.GetQuery(" state=0 ", null, " order by id asc ");
           });
            //筛选支持的BankCode的通道  并且 限额满足 并按限额排序
            var listBank = listBankQuota.FindAll(m => m.BankCode == BankCode && Amount <= m.SingleQuota).OrderByDescending(m => m.SingleQuota).ToList();


            int TypeId = (int)Logic.KeyValueType.支付通道管理;
            //获得通道列表
            var listChannelPay = MemcachHelper.Get<List<KeyValue>>(Constant.list_keyvalue_key + TypeId, DateTime.Now.AddDays(7), () =>
            {
                return keyValueDao.GetQuery(" typeid=@TypeId  ", new { TypeId }, "order by Sort desc,CTime desc");
            });
            bool isallnotChannel = true;//标记有积分通道，是否全部关闭 true 是全部关闭
            foreach (var item in listChannelPay)
            {
                if (item.State == 0 && item.KeyId != 2)
                {
                    isallnotChannel = false;
                }
            }
            //只获得可用通道
            listChannelPay = listChannelPay.FindAll(m => m.State == 0);
            if (mobile.Substring(0, 3) == "177")//17号段的用户排除荣邦通道 
            {
                listChannelPay = listChannelPay.FindAll(m => m.KeyId != 1 && m.KeyId != 4);
            }
            //获得卡ID排除通道的配置 begin
            var listDebarBankChannel = MemcachHelper.Get<List<DebarBankChannel>>(Constant.debarbankchannel_key, DateTime.Now.AddDays(7), () =>
           {
               return debarBankChannelDao.GetQuery(" 1=1  ", null, "order by CTime desc");
           });
            var itemDebarBankChannel = listDebarBankChannel.FindAll(m => m.BankID == BankID);
            if (itemDebarBankChannel != null && itemDebarBankChannel.Count > 0)
            {
                foreach (var item in itemDebarBankChannel)
                {
                    listChannelPay = listChannelPay.FindAll(m => m.KeyId !=item.ChannelID );
                }
            }
            //获得卡ID排除通道的配置 end

            Users user = usersDao.Single(UserId);
            var rate = Constant.GetRate(PayType, (Logic.VipType)user.VipType);


            Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
            //遍历通道  取得可用通道
            foreach (var item in listChannelPay)
            {
                
                JObject data = JObject.Parse(item.Value);
                DateTime StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")+" "+ data["StartTime"]);
                DateTime EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + data["EndTime"]);
                decimal BasicRate1 =data["Rate1"].ToDecimal();
                decimal BasicRate3 = data["Rate3"].ToDecimal();
                //判断通道是否是积分类型 并且满足时间范围 并且该通道支持改银行
                if (item.Value2 == PayType.ToString() && DateTime.Now>StartTime && DateTime.Now < EndTime && listBank.FindIndex(m=>m.ChannelType==item.KeyId)>-1 )
                {
                    ToolPay tp = new ToolPay(Amount,rate[0],rate[1], BasicRate1, BasicRate3);
                    dic.Add(item.KeyId.ToString(), tp.Income);
                }
            }
            if (dic.Count > 0)//计算最优利润
            {
                decimal maxIncom = 0M;
                int optimalChannelType = 0;
                foreach (var item in dic)
                {
                    if (maxIncom < item.Value || maxIncom==0M)
                    {
                        maxIncom = item.Value;
                        optimalChannelType = Convert.ToInt32( item.Key);
                    }
                }
                result.backState = 0;
                result.message = $"匹配到最佳通道,收益：{maxIncom}";
                result.Data = optimalChannelType;
                return result;
            }

            if (isallnotChannel)
            {
                result.backState = -100;
                result.message = "通道额度已用尽，请明日再试";
                return result;
            }

            //未匹配成功，默认选第一条通道
            foreach (var item in listChannelPay)
            {
                if (item.Value2 == PayType.ToString())
                {
                    result.backState = -100;
                    result.message = "未匹配到通道,请修改金额不超限额或选择支持的银行试试";
                    result.Data = item.KeyId;


                    JObject data = JObject.Parse(item.Value);
                    DateTime StartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + data["StartTime"]);
                    DateTime EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + data["EndTime"]);
                    if (DateTime.Now < StartTime)
                    {
                        result.message = "未到通道开启时间";
                    }
                    if (DateTime.Now > EndTime)
                    {
                        result.message = "通道已关闭，请明天再试";
                    }
                    break;
                }
            }
            return result;
        }



        /// <summary>
        /// 选择最优通道
        /// 1.利润优先
        /// 2.满足额度
        /// 3.满足运营时间
        /// </summary>
        /// <param name="BankId"></param>
        /// <param name="PayType"></param>
        public static ResultModelData<int> Optimal( decimal Amount = 0, int BankId=0, int PayType = 0)
        {
            var model = userBankCardDao.Single(BankId);
            return Optimal(model.UserId, Amount, model.BankCode, PayType,model.Mobile,BankId);
        }


    }
}
