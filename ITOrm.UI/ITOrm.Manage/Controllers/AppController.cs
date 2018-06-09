using ITOrm.Host.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Utility.Helper;
using ITOrm.Host.Models;
using Newtonsoft.Json.Linq;
using ITOrm.Payment.Yeepay;
using Newtonsoft.Json;
using ITOrm.Manage.Filters;
using ITOrm.Utility.Const;
using ITOrm.Utility.Cache;
using System.Text;

namespace ITOrm.Manage.Controllers
{
    public class AppController : Controller
    {
        public KeyValueBLL keyValueDao = new KeyValueBLL();
        // GET: App
        [AdminFilter]
        public ActionResult Version(int pageIndex =1,int Platform = -1)
        {
           
            int totalCount = 0;
            StringBuilder where = new StringBuilder();
            where.Append(" 1=1 and typeid=1 ");
            if (Platform != -1)
            {
                where.AppendFormat(" and KeyId={0}", Platform);
            }
            var listkeyValue = keyValueDao.GetPaged(10, pageIndex, out totalCount, where.ToString(), null," order by CTime desc");
 
            return View(new ResultModel<KeyValue>(listkeyValue, totalCount));
        }

        [HttpGet]
        public ActionResult Edit(int Id = 0)
        {
            KeyValue kv = new KeyValue();
            kv.KeyId = 2;
            if (Id > 0)
            {
                kv = keyValueDao.Single(Id);
            }
            return View(kv);
        }
        [HttpPost]
        public ActionResult Edit(int Id = 0,int Platform=0,string update="",string version="",string download="",int isupgrade = 0,int IsAuditing=0)
        {
            KeyValue kv = new KeyValue();

            if (Id > 0)
            {
                 kv = keyValueDao.Single(Id);
                kv.UTime = DateTime.Now;
            }
            kv.TypeId =(int)Logic.KeyValueType.平台版本号;
            kv.KeyId = Platform;
            JObject data = new JObject();
            data["update"] = update;
            data["version"] = version;
            data["download"] = download;
            data["isupgrade"] = isupgrade;
            data["IsAuditing"] = IsAuditing;
            kv.Value = data.ToString();
            bool flag = false;

            if (kv.ID > 0)
            {
                flag = keyValueDao.Update(kv);
            }
            else
            {
                flag = keyValueDao.Insert(kv) > 0;
            }
            int state = flag ?0: -100;
            string msg= flag ? "操作成功" : "操作失败";
            string url = "/app/version";
            MemcachHelper.Delete(Constant.list_keyvalue_key+ (int)Logic.KeyValueType.平台版本号);//清理缓存
            return new RedirectResult($"/Prompt?state={state}&msg={msg}&url={url}");
            
        }
    }
}