using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Host.Models;
namespace ITOrm.Host.BLL
{
    public partial class YeepayLogBLL
    {
        public  int Init(int TypeId,int UserId,int Platfrom,int KeyId=0,int ChannelType=0)
        {
            YeepayLog model = new YeepayLog();
            model.Code = "";
            model.CTime = DateTime.Now;
            model.UTime = DateTime.Now;
            model.IP = ITOrm.Utility.Client.Ip.GetClientIp();
            model.KeyId = KeyId;
            model.Msg = "";
            model.Platfrom = Platfrom;
            model.State = 0;
            model.TypeId = TypeId;
            model.UserId = UserId;
            model.ChannelType = ChannelType;
            return Insert(model);
        }

        public bool UpdateState(int Id,string code,string message,int state)
        {
            var model = Single(Id);
            model.Code = code;
            model.Msg = message;
            if (state != 1 && state!=0 && state!=10 && state!=5)
            {
                state = -1;
            }
            model.State = state;
            return Update(model);
        }
    }
}
