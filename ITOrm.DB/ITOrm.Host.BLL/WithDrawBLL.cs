using ITOrm.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class WithDrawBLL
    {
        public int Init(int UserId,int PayId,decimal Amount,int Platform ,string BankCard)
        {
            WithDraw model = new WithDraw();
            model.UserId = UserId;
            model.Amount = Amount;
            model.Platform = Platform;
            model.PayId = PayId;
            model.IP = ITOrm.Utility.Client.Ip.GetClientIp() ;
            model.ReceiverBankCardNo = BankCard;
            return Insert(model);
        }

        public bool UpdateState(int Id, int State, string meesgage)
        {

            WithDraw draw = Single(Id);
            draw.UTime = DateTime.Now;
            draw.State = State;
            draw.Message = meesgage;
            return Update(draw);
        }

    }
}
