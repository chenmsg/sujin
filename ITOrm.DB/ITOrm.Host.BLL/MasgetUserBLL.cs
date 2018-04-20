using ITOrm.Host.Models;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class MasgetUserBLL
    {
        /// <summary>
        /// 初始化用户
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Appid"></param>
        /// <param name="Session"></param>
        /// <param name="Secretkey"></param>
        /// <param name="CompanyId"></param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public int Init(int UserId,string Appid,string Session,string Secretkey,string CompanyId, int Platform,int TypeId,decimal Rate1,decimal Rate3)
        {
            MasgetUser mUser = new MasgetUser();
            mUser.UserId = UserId;
            mUser.Appid = Appid;
            mUser.Secretkey = Secretkey;
            mUser.Session = Session;
            mUser.Platform = Platform;
            mUser.CompanyId = CompanyId;
            mUser.TypeId = TypeId;
            mUser.Rate1 = Rate1;
            mUser.Rate3 = Rate3;
            mUser.RateState1 = 1;
            mUser.RateState3 = 1;
            mUser.State = 0;
            return Insert(mUser);

        }

        public bool UpdateState(int UserId,int TypeId,int State)
        {
            MasgetUser mUser = new MasgetUser();
            mUser = Single("UserId=@UserId and TypeId=@TypeId ", new { UserId , TypeId });
            mUser.State = State;
            mUser.UTime = DateTime.Now;
            return Update(mUser);
        }

        /// <summary>
        /// 查询是否开户
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="TypeId"></param>
        /// <returns></returns>
        public bool QueryIsExist(int UserId,int TypeId)
        {
            int cnt = Count("UserId=@UserId and TypeId=@TypeId", new { UserId,TypeId });
            return cnt > 0;
        }

        /// <summary>
        /// 查询是否入驻
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="TypeId"></param>
        /// <returns></returns>
        public bool QueryIsOpen(int UserId, int TypeId)
        {
            int cnt = Count("UserId=@UserId and TypeId=@TypeId and State=1", new { UserId, TypeId });
            return cnt > 0;
        }

    }
}
