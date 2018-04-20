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
    public class BankQuotaController : Controller
    {

        public static ViewBankQuotaBLL viewBankQuotaDao = new ViewBankQuotaBLL();
        public static BankQuotaBLL bankQuotaDao = new BankQuotaBLL();
        // GET: BankQuota
        public ActionResult Index(int pageIndex=1,int ChannelType=-1,int State=-200)
        {
            #region where 条件
            StringBuilder where = new StringBuilder();
            where.Append("1=1");
            
            if (State != -200)
            {
                where.AppendFormat(" and State={0}", State);
            }
            if (ChannelType != -1)
            {
                where.AppendFormat(" and ChannelType={0}", ChannelType);
            }
            #endregion
            int rowCnt = 0;
            var list = viewBankQuotaDao.GetPaged(10,pageIndex,out rowCnt, where.ToString(), null, "order by id desc");

            return View(new ResultModel<ViewBankQuota>(list, rowCnt));
        }

        [HttpGet]
        public ActionResult Edit(int Id = 0)
        {
            ViewBankQuota model = new ViewBankQuota();
            if (Id > 0)
            {
                model = viewBankQuotaDao.Single(Id);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int Id = 0, int ChannelType = 0, int BankId = 0, decimal SingleQuota = 0, decimal DayQuota = 0, decimal MouthQuota = 0, int State = 0)
        {
            BankQuota model = new BankQuota();

            if (Id > 0)
            {
                model = bankQuotaDao.Single(Id);

                model.UTime = DateTime.Now;
            }
            bool flag = false;
            model.ChannelType = ChannelType;
            model.BankId = BankId;
            model.SingleQuota = SingleQuota;
            model.DayQuota = DayQuota;
            model.MouthQuota = MouthQuota;
            model.State = State;
            if (model.ID > 0)
            {
                flag = bankQuotaDao.Update(model);
            }
            else
            {
                flag = bankQuotaDao.Insert(model) > 0;
            }
            int state = flag ? 0 : -100;
            string msg = flag ? "操作成功" : "操作失败";
            string url = "/bankquota/";
            MemcachHelper.Delete(Constant.list_bank_quota_key);//清理缓存
            return new RedirectResult($"/Prompt?state={state}&msg={msg}&url={url}");

        }


    }
}