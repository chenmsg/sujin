using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Core.Helper;
namespace ITOrm.Utility.Const
{
   public static class Constant
    {
        /// <summary>
        /// 是否测试环境
        /// </summary>
        public  static bool IsDebug { get {
                var str=ConfigHelper.GetAppSettings("IsDebug");
                return str=="debug";
            } }

        /// <summary>
        /// 是否免签
        /// </summary>
        public static bool IsSign
        {
            get
            {
                var str = ConfigHelper.GetAppSettings("IsSign");
                return str == "true";
            }
        }


        /// <summary>
        /// Debug字符串  为区分缓存
        /// </summary>
        public static string Debug
        {
            get { return ConfigHelper.GetAppSettings("IsDebug"); }
        }


        /// <summary>
        /// appliction 路径
        /// </summary>
        public static string CurrentApiDic {
            get {
                var str = ConfigHelper.GetAppSettings("CurrentApiDic");
                return str;
            }
        }
        /// <summary>
        /// 短信通道
        /// </summary>
        public static int Merchant
        {
            get
            {
                return 1001;
            }
        }

        /// <summary>
        /// 注册的手机验证码key
        /// </summary>
        public static string reg_mobile_code = Debug+"-reg-mobile-code-";
        /// <summary>
        /// 腾付通验证码
        /// </summary>

        public static string teng_mobile_code = Debug + "-teng-mobile-code-";

        /// <summary>
        /// 找回密码的手机验证码key
        /// </summary>
        public static string forget_mobile_code = Debug + "-forget-mobile-code-";

        /// <summary>
        /// 注册的图形验证码key
        /// </summary>
        public static string reg_img_code = Debug + "-reg-img-code-";

        /// <summary>
        /// 忘记密码的图形验证码key
        /// </summary>
        public static string forget_img_code = Debug + "-forget-img-code-";

        /// <summary>
        /// 忘记密码用于修改密码的令牌
        /// </summary>
        public static string forget_token = Debug + "-forget-token-";

        /// <summary>
        /// 地区码缓存
        /// </summary>
        public static string list_area_key = Debug + "-list-area-baseid-";

        /// <summary>
        /// 支付通道列表缓存
        /// </summary>
        public static string list_channelpay_key = Debug + "-list-channelpay";

        /// <summary>
        /// app版本管理缓存
        /// </summary>
        public static string list_appversion_key = Debug + "-list-appversion";

        /// <summary>
        /// keyValue缓存
        /// </summary>
        public static string list_keyvalue_key = Debug + "-list-keyvalue-";



        /// <summary>
        /// 银行列表缓存
        /// </summary>
        public static string list_bank_key = Debug + "-list-bank";

        /// <summary>
        /// api登录参数缓存
        /// </summary>
        public static string list_api_channel_key = Debug + "-list-api-channel-";

        /// <summary>
        /// 银行卡限额
        /// </summary>
        public static string list_bank_quota_key = Debug + "-list-bank-quota";


        /// <summary>
        /// 银行卡卡Bin
        /// </summary>
        public static string list_bank_bin_key = Debug + "-list-bank-bin";

        /// <summary>
        /// 图片验证码过期时间
        /// </summary>
        public static int img_code_expires = 600;

        /// <summary>
        /// 短信验证码过期时间
        /// </summary>
        public static int mobile_code_expires = 300;
    
        /// <summary>
        /// 业务令牌有效期
        /// </summary>
        public static int token_expires = 300;

        /// <summary>
        /// 登录锁定期
        /// </summary>
        public static int login_locking_expires = 60 * 60 * 3;

        /// <summary>
        /// 登录错误次数
        /// </summary>
        public static int login_locking_num = 5;

        /// <summary>
        /// 登录Key  Guid
        /// </summary>
        public static string login_key = Debug + "login-appkey-";
        /// <summary>
        /// 当前API站点域名
        /// </summary>
        public static string CurrentApiHost
        {
            get
            {

                return ConfigHelper.GetAppSettings("CurrentApiHost");
            }
        }


        //积分
        public static decimal[] fee1Rate1 = new decimal[] { 0.0041M, 0.0043M, 0.0050M };
        public static decimal[] fee1Rate3 = new decimal[] { 1M, 2M, 2M };

        //无积分
        public static decimal[] fee2Rate1 = new decimal[] { 0.0030M, 0.0039M, 0.0047M };
        public static decimal[] fee2Rate3 = new decimal[] { 0.5M, 2M, 2M };


        public static decimal[] GetRate(int payType, Logic.VipType vipType)
        {
            decimal Rate1 = 0M;
            decimal Rate3 = 0M;
            switch (vipType)
            {
                case Logic.VipType.顶级用户:

                    Rate1 = payType == 0 ? Constant.fee1Rate1[0] : Constant.fee2Rate1[0];
                    Rate3 = payType == 0 ? Constant.fee1Rate3[0] : Constant.fee2Rate3[0];
                    break;
                case Logic.VipType.Vip用户:

                    Rate1 = payType == 0 ? Constant.fee1Rate1[1] : Constant.fee2Rate1[1];
                    Rate3 = payType == 0 ? Constant.fee1Rate3[1] : Constant.fee2Rate3[1];
                    break;
                case Logic.VipType.普通用户:
                    Rate1 = payType == 0 ? Constant.fee1Rate1[2] : Constant.fee2Rate1[2];
                    Rate3 = payType == 0 ? Constant.fee1Rate3[2] : Constant.fee2Rate3[2];
                    break;
                default:
                    break;
            }
            return new decimal[] {Rate1,Rate3 };
        }
    }
}
