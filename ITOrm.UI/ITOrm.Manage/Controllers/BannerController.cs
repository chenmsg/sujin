using ITOrm.Host.BLL;
using ITOrm.Host.Models;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Const;
using ITOrm.Utility.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ITOrm.Manage.Controllers
{
    public class BannerController : Controller
    {
        public static BannerBLL bannerDao = new BannerBLL();
        // GET: Banner
        public ActionResult Index(int pageIndex = 1, int State = -1 )
        {
            #region where 条件
            StringBuilder where = new StringBuilder();
            where.Append("1=1");
          
            if (State != -1)
            {
                where.AppendFormat(" and State={0}", State);
            }
         
            #endregion
            int totalCount = 0;
            var list = bannerDao.GetPaged(10, pageIndex, out totalCount, where.ToString(), null, "order by id desc");
            return View(new ResultModel<Banner>(list, totalCount));

        }
        [HttpGet]
        public ActionResult Edit(int Id=0)
        {
            Banner kv = new Banner();
            if (Id > 0)
            {
                kv = bannerDao.Single(Id);
            }
            return View(kv);
        }

        [HttpPost]
        public ActionResult Edit(int Id = 0, string Title="",string WapUrl ="",string ImgUrl="", int State=0,string StartTime = "", string EndTime="",int Sort=0 )
        {
            Banner kv = new Banner();

            if (Id > 0)
            {
                kv = bannerDao.Single(Id);
            }
            JObject data = new JObject();
    
            kv.Title = Title;
            kv.WapURL = WapUrl;
            kv.ImgUrl = ImgUrl;
            kv.Sort = Sort;
            kv.State = State;
            kv.StartTime = Convert.ToDateTime( StartTime);
            kv.EndTime = Convert.ToDateTime(EndTime);
            bool flag = false;

            if (kv.ID > 0)
            {
                flag = bannerDao.Update(kv);
            }
            else
            {
                flag = bannerDao.Insert(kv) > 0;
            }
            int state = flag ? 0 : -100;
            string msg = flag ? "操作成功" : "操作失败";
            string url = "/Banner/";
            MemcachHelper.Delete(Constant.list_banner_key);//清理缓存
            return new RedirectResult($"/Prompt?state={state}&msg={msg}&url={url}");

        }

    }
}