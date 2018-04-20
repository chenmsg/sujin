using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITOrm.Utility.Const;
using ITOrm.Utility.Helper;
using ITOrm.Utility.ITOrmApi;
using ITOrm.Utility.Cache;

namespace ITOrm.Api.Controllers
{
    public class ConfigController : Controller
    {
        public KeyValueBLL keyValueDao = new KeyValueBLL();
        public UsersBLL usersDao = new UsersBLL();

        #region 获得最新App版本信息
        public string GetVersion(int cid)
        {
            int TypeId = (int)Logic.KeyValueType.平台版本号;
            //通过cid查询APP版本升级信息
            var list = MemcachHelper.Get<List<KeyValue>>(Constant.list_keyvalue_key+TypeId, DateTime.Now.AddDays(7), () =>
            {
                return keyValueDao.GetQuery(1, " state<>-1 and typeid=@TypeId and keyId=@cid", new { cid, TypeId }, "order by Sort desc,CTime desc");
            });
            JObject data = new JObject();
            foreach (var item in list)
            {
                data = JObject.Parse(item.Value);
                data["CTime"] = item.CTime;
                data["Platform"] = item.KeyId;
                data["PlatformTxt"] = ((Logic.Platform)item.KeyId).ToString();
            }
            return ApiReturnStr.getApiData(data);
        }
        #endregion

        #region 得到支付类型列表
        public string GetPayType(int UserId)
        {
            int TypeId = (int)Logic.KeyValueType.支付类型管理;
            var listKeyValue = MemcachHelper.Get<List<KeyValue>>(Constant.list_keyvalue_key+ TypeId, DateTime.Now.AddDays(7), () =>
            {
                return keyValueDao.GetQuery("typeid=@TypeId ", new { TypeId }, "order by Sort desc,CTime desc");
            });
            Users user = usersDao.Single(UserId);

            Logic.VipType vip = (Logic.VipType)user.VipType;
            JArray list = new JArray();

            
            foreach (var item in listKeyValue)
            {
                JObject m = JObject.Parse(item.Value);

                JObject data = new JObject();
                int PayType = m["PayType"].ToInt();
                data["PayType"] = PayType;
                data["PayName"] = m["PayName"].ToString();
                data["Quota"] = m["Quota"].ToString();
                data["WithDraw"] = m["WithDraw"].ToString();
                data["Time"] = m["Time"].ToString();
                data["Remark"] = m["Remark"].ToString();

                decimal[] r = Constant.GetRate(PayType, vip);
                data["Fee"] = $"{r[0].perCent()}+{r[1].ToString("F1")}元/笔";
                list.Add(data);
            }
            return ApiReturnStr.getApiDataList(list);
        }
        #endregion

        #region 得到不同会员类型的费率信息
        public string GetVipTypeIntroduce(int VipType)
        {
            if (VipType < 0||VipType>2)
            {
                return ApiReturnStr.getError(-100,"参数错误");
            }
            int TypeId = (int)Logic.KeyValueType.支付类型管理;
            var listKeyValue = MemcachHelper.Get<List<KeyValue>>(Constant.list_keyvalue_key + TypeId, DateTime.Now.AddDays(7), () =>
            {
                return keyValueDao.GetQuery("typeid=@TypeId ", new { TypeId }, "order by Sort desc,CTime desc");
            });
            
            Logic.VipType vip = (Logic.VipType)VipType;
            JArray list = new JArray();
            foreach (var item in listKeyValue)
            {
                JObject m = JObject.Parse(item.Value);
                JObject data = new JObject();
                int PayType = m["PayType"].ToInt();
                data["PayName"] = m["PayName"].ToString();
                data["Quota"] = m["Quota"].ToString();
                decimal[] r = Constant.GetRate(PayType, vip);
                data["Fee"] = $"{r[0].perCent()}+{r[1].ToString("F1")}元/笔";
                list.Add(data);
            }
            return ApiReturnStr.getApiDataList(list);
        }
        #endregion

    }
}