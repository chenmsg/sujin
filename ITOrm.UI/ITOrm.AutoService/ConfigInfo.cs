using System.Configuration;

namespace ITOrm.AutoService
{
    public class ConfigInfo
    {

        //获取处理配置信息
        public static int BatchWaitMilliSecond = int.Parse(ConfigurationManager.AppSettings["BatchWaitMilliSecond"]);
        public static string GetMobile = ConfigurationManager.AppSettings["mobile"];
        public static  int theadTime =int.Parse( ConfigurationManager.AppSettings["theadTime"]) ;


    }
}
