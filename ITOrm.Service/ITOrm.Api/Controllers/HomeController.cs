using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.Mvc;
using ITOrm.Utility.ITOrmApi;

using ITOrm.Host.BLL;
using System.Data.SqlClient;
using ITOrm.Utility.Serializer;
using ITOrm.Host.Models;
using ITOrm.Core.Memcached.Impl;
using Memcached.ClientLibrary;
namespace ITOrm.Api.Controllers
{
    public class HomeController : Controller
    {

        public string Index()
        {




            //int top = 20;
            //////var list = bll.GetList();

            //DateTime beforDT = System.DateTime.Now;
            //var lisstop = bll.GetList(top);
            ////ViewModulesBLL bllm = new ViewModulesBLL();
            ////耗时巨大的代码  
            //DateTime afterDT = System.DateTime.Now;
            //TimeSpan ts = afterDT.Subtract(beforDT);
            //Console.WriteLine("DateTime总共花费{0}ms.", ts.TotalMilliseconds);



            //DateTime beforDT1 = System.DateTime.Now;
            //var lisstop1 = bll.GetListTest(top);
            ////ViewModulesBLL bllm = new ViewModulesBLL();
            ////耗时巨大的代码  
            //DateTime afterDT1 = System.DateTime.Now;
            //TimeSpan ts1 = afterDT1.Subtract(beforDT1);
            //Console.WriteLine("DateTime总共花费{0}ms.", ts1.TotalMilliseconds);

            //DateTime beforDT3 = System.DateTime.Now;
            //var result3 = bll.GetListByID (1);
            //DateTime afterDT3 = System.DateTime.Now;
            //TimeSpan ts3 = afterDT3.Subtract(beforDT3);
            //Console.WriteLine("DateTime总共花费{0}ms.", ts3.TotalMilliseconds);

            //string aa = "";









            //var result_modules = bllm.Single(13);
            //var cnt = bll.GetCnt(1);
            //var listpage = bll.GetPaged();

            //Clump.Host.BLL.ViewModulesBLL mbll = new ViewModulesBLL();
            //var mresult = mbll.Single(10);

            //var mlist = mbll.GetList("主菜单");


            //object obj = new { ModuleName = "主菜单" };
            //IDictionary<string, object> dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(obj);
            //object aa = dictionary as object;

            //var mlistp = mbll.GetPaged(dictionary);


            return "苏津科技接口";


            
            //int a = 0;
            //int b = 0;
            //int c = a / b;
            //AdminUserBll bll = new AdminUserBll();
            //var admin= bll.Login("chenxin","chenxin");
            //return ApiReturnStr.getApiData(admin);
            //using (SqlConnection connection = new SqlConnection(_strConn))
            //{
            //    connection.Open();
            //    var brand = connection.Query<Brands>(_strSQL).ToList();
            //    foreach (var item in brand)
            //    {
            //        Console.Write(item.BrandID + " " + item.BrandName);
            //    }
            //}
            //var conn = GetOpenConnection();
            //var brand = conn.Query<Brand>(_strSQL);
         
            //foreach (var item in brand)
            //{
            //    Console.Write(item.BrandID + " " + item.BrandName);
            //}
            //var b = new Brand();
            //b.BrandID = 1;
            //b.BrandName = "cc";
            //conn.Update<Brand>(b);

            //string username = "15110167786";
            //string password = "111111";

            //    GeneralAccount admin = new GeneralAccount();
            //    admin = GeneralAccount.Single(" UserName = @UserName and PassWord = @PassWord and Category = 1 and IsDel=0 ", new { UserName = username, PassWord = password.MD5_32() });
            //    return ApiReturnStr.getApiData<GeneralAccount>(admin);
            
        }
       
    }

}