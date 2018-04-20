using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Payment.Teng
{
    public class Enums
    {

        public enum TengType
        {
            支付接口=300,
            支付结果查询=301,
            代付申请=302,
            代付结果查询=303,
            生成收银台 = 304,
            发送收银短信 = 305,
            验证收银短信=306
        }
    }
}
