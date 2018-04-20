using ITOrm.Host.Models;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class BankTreatyApplyBLL
    {
       
        public int Init(int UserId,int UbkId, string BankCard, string Mobile, int Platform, int ChannelType)
        {

            BankTreatyApply model =Single(" UbkId=@UbkId and ChannelType=@ChannelType", new { UbkId, ChannelType });
            bool flag = false;
            if (model != null) { flag = true;  }
            else { model = new BankTreatyApply(); }
            model.BankCard = BankCard;
            model.Mobile = Mobile;
            if (flag)//修改
            {
                Update(model);
                return model.ID;
            }
            else//新增
            {
                model.Platform = Platform;
                model.UbkID = UbkId;
                model.ChannelType = ChannelType;
                model.UserId = UserId;
                return Insert(model);
            }
        }

        public bool UpdateTreatycode(int Id,string Treatycode,string Smsseq,int State)
        {
            BankTreatyApply model = Single(Id);
            model.UTime = DateTime.Now;
            model.Treatycode = Treatycode;
            model.Smsseq = Smsseq;
            model.State = State;
            return Update(model);
        }

        public bool QueryTreatycodeIsOpen(int UbkID,int ChannelType)
        {
            int cnt = Count("UbkID=@UbkID and ChannelType=@ChannelType and state=2", new { UbkID, ChannelType });
            return cnt > 0;
        }

    }
}
