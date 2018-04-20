using ITOrm.Host.Models;
using ITOrm.Utility.Const;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class PayCashierBLL
    {
        public int Init(int ChannelType,int LogId,int UbkId,int UserId,int PayRecordId)
        {
            PayCashier payc = new PayCashier();
            payc.ChannelType = ChannelType;
            payc.LogId = LogId;
            payc.UbkId = UbkId;
            payc.UserId = UserId;
            payc.PayRecordId = PayRecordId;
            return Insert(payc);
        }
    }
}
