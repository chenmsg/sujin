using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Utility.Helper
{
    /// <summary>
    /// 提前计算手续费
    /// </summary>
    public class ToolPay
    {
        public ToolPay(decimal Amount, decimal Rate1, decimal Rate2, decimal Rate3, decimal Rate4, decimal Rate5)
        {
            this.Amount = Amount;
            this.Rate1 = Rate1;
            this.Rate2 = Rate2;
            this.Rate3 = Rate3;
            this.Rate4 = Rate4;
            this.Rate5 = Rate5;
        }


        public ToolPay(decimal Amount, decimal Rate1,decimal Rate3, decimal BasicRate1, decimal BasicRate3)
        {
            this.Amount = Amount;
            this.Rate1 = Rate1;
            this.Rate3 = Rate3;
            this.BasicRate1 = BasicRate1;
            this.BasicRate3 = BasicRate3;
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 交易费率
        /// </summary>
        public decimal Rate1 { get; set; }
        /// <summary>
        /// T1
        /// </summary>
        public decimal Rate2 { get; set; }
        /// <summary>
        /// T0自助结算基本费率
        /// </summary>
        public decimal Rate3 { get; set; }
        /// <summary>
        /// T0自助结算工作日额外费率
        /// </summary>
        public decimal Rate4 { get; set; }
        /// <summary>
        /// T0自助结算非工作日额外费率
        /// </summary>
        public decimal Rate5 { get; set; }
        /// <summary>
        /// 交易手续费
        /// </summary>
        public decimal PayFee { get { return (Amount * Rate1).Rounding(); } }
        /// <summary>
        /// 收款基本费率
        /// </summary>
        public decimal BasicFee { get { return Rate3; } }
        /// <summary>
        /// 额外手续费
        /// </summary>
        public decimal ExTargetFee
        {
            get
            {
                //此处应该区别当前时间是否是工作日来分别计算手续费
                //return (Amount * Rate5).Rounding();
                return (Amount * Rate4).Rounding();
            }
        }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal ActualAmount
        {
            get
            {
                return Amount - PayFee - BasicFee - ExTargetFee;
            }
        }

        /// <summary>
        /// 签署交易费率
        /// </summary>
        public decimal BasicRate1 { get; set; }
        /// <summary>
        /// 签署结算费率
        /// </summary>
        public decimal BasicRate3 { get; set; }

        public decimal Income
        {
            get
            {
                return ((Amount - (Amount * BasicRate1).Rounding() - BasicRate3) - ActualAmount);
            }
        }
    }
}
