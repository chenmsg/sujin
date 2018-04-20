using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Host.BLL
{
    public partial class SendMsgBLL
    {
        public bool ValidateRegisterCnt(string Mobile)
        {
            return  Count("Mobile=@Mobile  and  State=2 and DateDiff(dd,CTime,getdate())=0 and  Service='reg'  ", new { Mobile }) >4;
        }
        public bool ValidateForgetCnt(string Mobile)
        {
            return Count("Mobile=@Mobile  and  State=2 and DateDiff(dd,CTime,getdate())=0 and  Service='forget'  ", new { Mobile }) > 4;
        }
    }
}
