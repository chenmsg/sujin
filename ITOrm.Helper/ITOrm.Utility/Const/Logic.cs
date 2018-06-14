using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Utility.Const
{
    public class Logic
    {
        public enum EnumSendMsg
        {
            注册短信 = 10,
            忘记密码短信 = 11,
                自动处理程序出错 = 12,
                腾付通收银短信=13
        }

        public enum Platform
        {
            未知=0,
            系统=1,
            Android=2,
            iOS=3
        }

        public enum PayType
        {
            积分=0,
            无积分=1
        }

        public enum ChannelType
        {
            易宝=0,
            荣邦科技积分=1,
            荣邦科技无积分 = 2,
            腾付通=3,
            荣邦3=4,
            米刷=5
        }

        public enum VipType
        {
            Boss = 0,
            顶级代理=1,
            SVIP = 2,
            VIP = 3,
            普通用户 = 4
        }

        public enum KeyValueType
        {
            平台版本号=1,
            支付通道管理=2,
            支付类型管理=3
        }

        public enum YeepayLogState
        {
            处理失败=-1,
            发起请求=0,
            请求成功=1,
            等待回调=5,
            处理成功=10
        }


        public enum TimedTaskType
        {
            通道开启=1
        }

        //资金表
        public enum InOrOut
        {
            扣减=-1,
            冻结=0,
            增加=1
        }
        public enum AccountType
        {
            //资金增加
            收款分润=100,
            开通SVIP分润=101,
            开通VIP分润 = 102,
            升级SVIP分润=103,
            开通顶级代理  = 104,


            升级会员 =150,
            //资金冻结
            提现申请=200,

            //资金扣减
            提现到账=301
        }

    }

    
}
