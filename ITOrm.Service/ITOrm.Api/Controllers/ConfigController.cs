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
using ITOrm.Utility.StringHelper;
using ITOrm.Utility.Log;

namespace ITOrm.Api.Controllers
{
    public class ConfigController : Controller
    {
        public KeyValueBLL keyValueDao = new KeyValueBLL();
        public UsersBLL usersDao = new UsersBLL();
        public BannerBLL bannerDao = new BannerBLL();
        #region 获得最新App版本信息
        public string GetVersion(int cid)
        {
            int TypeId = (int)Logic.KeyValueType.平台版本号;
            //通过cid查询APP版本升级信息
            var list = MemcachHelper.Get<List<KeyValue>>(Constant.list_keyvalue_key+TypeId, DateTime.Now.AddDays(7), () =>
            {
                return keyValueDao.GetQuery(10, " state<>-1 and typeid=@TypeId ", new { TypeId }, "order by Sort desc,CTime desc");
            });
            list = list.FindAll(m=>m.KeyId==cid).OrderByDescending(m=>m.Sort).ThenByDescending(m=>m.CTime).ToList();
            JObject data = new JObject();
            if (list != null&&list.Count>0)
            {
                bool flag = false;
                foreach (var item in list)
                {
                    var temp= JObject.Parse(item.Value);
                    if (temp["IsAuditing"].ToInt() == 1)
                    {
                        flag = true;
                        data = JObject.Parse(item.Value);
                        data["CTime"] = item.CTime;
                        data["Platform"] = item.KeyId;
                        data["PlatformTxt"] = ((Logic.Platform)item.KeyId).ToString();
                        break;
                    }
                }
                if (!flag)//未找到最新版本
                {
                    return ApiReturnStr.getError(0, "暂无新版本");
                }
            }
            else
            {
                return ApiReturnStr.getError(0, "暂无新版本");
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
        public string GetVipTypeIntroduce(int cid=0,int VipType=0)
        {
            
            if (VipType < 0)
            {
                return ApiReturnStr.getError(-100,"参数错误");
            }
            var version = TQuery.GetString("version");
            if (VipType == 1&& version=="1.0.0"&&cid==3)
            {
                VipType = 4;
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

        #region App轮播图
        public string BannerList(int UserId)
        {
            List<Banner> listBanner = MemcachHelper.Get<List<Banner>>(Constant.list_banner_key, DateTime.Now.AddDays(7), () =>
            {
               return  bannerDao.GetQuery(10, " State=1 AND GETDATE() BETWEEN StartTime AND EndTime", null, "ORDER BY Sort DESC,ID DESC");
            });
             
            JArray list = new JArray();
            if (listBanner != null && listBanner.Count > 0)
            {
                foreach (var item in listBanner)
                {
                    JObject data = new JObject();
                    data["ID"] = item.ID;
                    data["Title"] = item.Title;
                    data["WapURL"] = item.WapURL;
                    data["ImgUrl"] = ITOrm.Utility.Const.Constant.StaticHost+ item.ImgUrl;
                    list.Add(data);
                }
            }
            return ApiReturnStr.getApiDataList(list);
        }
        #endregion

        #region 首页按钮数据

        public string GetIndexData(int cid=0,int UserId=0,string version="")
        {
            var serverVersion = keyValueDao.GetAuditingVersion(cid);


            JObject data = new JObject();
            JArray list = new JArray();
            JObject obj1 = new JObject();
            obj1["Title"] = "邀请好友";
            obj1["icon"] = Constant.StaticHost+ "upload/btn/01.png";
            obj1["WapUrl"] = "QRcode";
            list.Add(obj1);

            JObject obj2 = new JObject();
            obj2["Title"] = "邀请收益";
            obj2["icon"] = Constant.StaticHost + "upload/btn/02.png"; 
            obj2["WapUrl"] = (cid == (int)Logic.Platform.iOS && version == serverVersion) ? Constant.CurrentApiHost : "InviteIncome";
            list.Add(obj2);


            JObject obj3 = new JObject();
            obj3["Title"] = "新手指引";
            obj3["icon"] = Constant.StaticHost + "upload/btn/03.png";
            obj3["WapUrl"] = "https://mp.weixin.qq.com/s/wPYnEFtQZOuWYnERcsQfGQ";
            list.Add(obj3);


            JObject obj4 = new JObject();
            obj4["Title"] = "火爆上线";
            obj4["icon"] = Constant.StaticHost + "upload/btn/04.png";
            obj4["WapUrl"] = (cid == (int)Logic.Platform.iOS && version == serverVersion) ?Constant.CurrentApiHost: "HuoBao";
            list.Add(obj4);

            JObject obj5 = new JObject();
            obj5["Title"] = "办卡攻略";
            obj5["icon"] = Constant.StaticHost + "upload/btn/05.png";
            obj5["WapUrl"] =Constant.CurrentApiHost+ "bankcard.html";
            list.Add(obj5);


            JObject obj6 = new JObject();
            obj6["Title"] = "收款攻略" ;
            obj6["icon"] = Constant.StaticHost + "upload/btn/06.png";
            obj6["WapUrl"] = Constant.CurrentApiHost + "Swipe.html";
            list.Add(obj6);

            data["btnList"] = list;

            return ApiReturnStr.getApiData(data);
        }
        #endregion

        #region 审核隐藏
        public string AppAuditingHide(int cid, string version,int UserId)
        {
            var serverVersion = keyValueDao.GetAuditingVersion(cid);
            if (cid == 3 && version == serverVersion)
            {
                return ApiReturnStr.getError(0, "hidden");
            }
            return ApiReturnStr.getError(-100, "show");
        }
        #endregion


    }
}