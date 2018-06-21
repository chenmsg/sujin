using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Payment.Masget
{
    public class Enums
    {



        public enum MasgetType
        {
            快速进件 = 100,
            子商户秘钥下载 = 101,
            商户通道入驻接口 = 102,
            申请开通快捷协议 = 103,
            确认开通快捷协议 = 104,
            查询快捷协议 = 105,
            订单支付= 106,
            查询交易订单=107,
            确认支付=108,
            修改同名进出商户费率=109,
            修改协议信息 = 110
        }
    }
}
