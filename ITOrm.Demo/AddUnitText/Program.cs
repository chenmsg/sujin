using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Utility.Log;
using System.Net;
using System.Web.Security;
using ITOrm.Utility.Client;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace AddUnitText
{
    class Program
    {
        private static string key = "651E974AC1C945D7BA37145F88CCE93D";
        static void Main(string[] args)
        {


            //string json = "{\"versionNo\":1,\"price\":0.01,\"description\":\"测试\",\"mchNo\":\"100000\",\"tradeNo\":\"1234567890123457\",\"orderDate\":\"20161114205514\",\"notifyUrl\":\"http://localhost:8080/test\",\"orderType\":\"QRCODE_ALI\"}";
            //string result111 = ITOrm.Payment.MiShua.AES.Encrypt(json, "1234567812345678", "0102030405060708");
            //result111 += "1234";
            //string sasdfas= ITOrm.Utility.Encryption.SecurityHelper.GetMD5String(result111).ToUpper();


            //string safas = "Fa6E0O1QIyOqGhVFR/OMkh9Yh0elDfrHJH0Btu/G7LqIoiyNw1BmNhgOEZhS+hCq+0o5LOaKA787ck5Frc4CxOBD5ufCrLkgNOZifxoHZOJkL8GP4iCTubfX/T1aqDI/b+6I+IU+kgBzgqbwBEsCk7w9ZJ7If3xwfUZCCj/Oy/ot77oSoHI8Ezv6f253CjmIhjM33o6KgOEVF8ABFteHBNCcE8LtpBJELaiyiXtY5m746D+42XUngJy+a1w82gzLWxigAtU3xxwj2ehtq6aPA7jnhLY4F0g6PLXrD5P2wpM1vVDW479aeQnB+lkgeiThn8k3snSflmq+0ealLPvVSPl3xJH7gTkhs9ahyt7J/a1djaayjwoGQFpwdbMdwtru";
            //string asdfa= ITOrm.Payment.MiShua.AES.Decrypt(safas, "1234567812345678", "0102030405060708");

            //Console.WriteLine(result111);

            //Console.ReadKey();
            //return;
            //string[] strs = new string[]{
            //    "2016-09-08",
            //    "2016-09-19",
            //    "2016-09-22",
            //    "2016-09-23",
            //    "2016-09-24",
            //    "2016-09-26",
            //    "2016-09-27",
            //    "2016-09-28",
            //    "2016-09-29",
            //    "2016-10-29",
            //    "2016-11-13",
            //    "2016-11-22",
            //    "2016-12-02",
            //    "2016-12-03",
            //    "2017-01-04"
            //};
            var login = HttpHelper.HttpGetHTML("http://pub.anxin.com/api/wdzjtoken.aspx?username=wdzj&password=34c0e54563685e38c9b22a");
            JObject data = JObject.Parse(login);
            string token = data["data"]["token"].ToString();
            string ffff = "";
            int successNum = 0;
            int error = 0;
            DateTime startTime = DateTime.Parse("2018-01-01");
            DateTime endTime = DateTime.Now;//DateTime.Parse("2018-01-01"); //


 

            //foreach (var item in strs)
            //{
            //    try
            //    {
            //        string url = string.Format("http://pub.anxin.com/api/wdzjBorrow.aspx?token={0}&date={1}&pageSize=500&page=1&IsRefresh=1", token, item);
            //        var result = HttpHelper.HttpGetHTML(url);
            //        if (result.Substring(0, 2) != "失败")
            //        {
            //            Console.WriteLine("成功：" + url);
            //            successNum++;
            //        }
            //        else
            //        {
            //            Console.WriteLine("失败：" + url);
            //            error++;
            //        }
            //        JObject d = JObject.Parse(result);
            //        if (d["totalCount"].ToInt() > 500)
            //        {
            //            ffff += $"time:{startTime.ToString("yyyy-MM-dd")}";
            //        }
            //    }
            //    catch (Exception)
            //    {

            //        error++;
            //    }
            //    Thread.Sleep(2000);
            //}


            //while (true)
            //{
            //   string url = " http://testapi.sujintech.com/itapi/Users/CheckDevice?itormName=itormios&sign=49569bcefe1a361d66184b287b7ce377&UserId=100094&guid=A201CA03-2793-451D-9EA3-27E55442C3B0&version=1.0.0";
            //     var result = HttpHelper.HttpGetHTML(url);
            //    Console.WriteLine(result);
            //    Thread.Sleep(2000);
            //}

            string errordate = "";


            while (startTime < endTime)
            {
                try
                {
                    string url = string.Format("http://pub.anxin.com/api/wdzjBorrow.aspx?token={0}&date={1}&pageSize=500&page=1&IsRefresh=1", token, startTime.ToString("yyyy-MM-dd"));
                    var result = HttpHelper.HttpGetHTML(url);
                    if (result.Substring(0, 2) != "失败")
                    {
                        Console.WriteLine("成功：" + url);
                        successNum++;
                    }
                    else
                    {
                        errordate += startTime.ToString("yyyy-MM-dd");
                        Console.WriteLine("失败：" + url);
                        error++;
                    }
                    JObject d = JObject.Parse(result);
                    if (d["totalCount"].ToInt() > 500)
                    {
                        ffff += $"time:{startTime.ToString("yyyy-MM-dd")}";
                    }
                }
                catch (Exception)
                {

                    error++;
                }
                Thread.Sleep(5000);
                startTime = startTime.AddDays(1);
            }


            Console.WriteLine($"successNum:{successNum},error:{error},ffff:{ffff}");

            Console.WriteLine($"errordate:{errordate}");


            Console.ReadLine();
        }
        
        
    }
}










