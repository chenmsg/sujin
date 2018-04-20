using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Payment.Yeepay
{
    public class Enums
    {
        public enum YeepayType
        {
            子商户注册 = 100,
            子商户审核 = 101,
            子商户信息查询 = 102,
            修改银行卡信息 = 103,
            修改结算信息 = 104,
            修改商户基本信息 = 105,

            子商户费率查询 = 200,

            设置费率1 = 201,
            设置费率2 = 202,
            设置费率3 = 203,
            设置费率4 = 204,
            设置费率5 = 205,

            子商户限额查询 = 250,

            收款接口 = 300,
            交易查询 = 301,
            结算接口 = 400,
            结算记录查询 = 401,
            结算手续费查询 = 402,

            可用余额查询 = 500,
            垫资额度查询 = 501,

            代理商返佣转账 = 600
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public enum AuditMerchant
        {
            SUCCESS = 1,
            FAILED = -1
        }

        public enum BankCardType
        {

            信用卡 = 1,
            借记卡 = 2
        }

        public enum PayRecordState
        {
            未支付 = 0,
            支付失败 = -1,
            支付发起中 = 1,
            结算中 = 5,
            结算成功 = 10
        }
        public enum WithDrawState
        {
            未发起 = 0,
            已接受 = 1,
            处理中 = 2,
            打款成功 = 10,
            打款失败 = -1,
            已退款 = -2,
            已撤销 = -3,
            结算失败 = -4
        }

        public enum AuditStatus
        {
            待审核=1,
            初审通过=2,
            复审通过=3,
            初审拒绝=4,
            复审拒绝=5
        }
    }
}
