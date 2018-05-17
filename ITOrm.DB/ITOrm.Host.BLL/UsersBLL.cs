using ITOrm.Host.Models;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Host.BLL
{
    public partial class UsersBLL
    {
        /// <summary>
        /// 付费升级Vip
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="VipType"></param>
        /// <param name="PlatForm"></param>
        /// <returns></returns>
        public ResultModel UpgradeVip(int UserID,int VipType,int PlatForm)
        {
            var list = ITOrm.Utility.Helper.DapperHelper.ExecuteProcedure<ResultModel>("proc_UpgradeVip", new { UserID,VipType,PlatForm });
            return list.FirstOrDefault();
        }

        /// <summary>
        /// 检查刷卡升级vip
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="PlatForm"></param>
        /// <returns></returns>
        public ResultModel CheckGiveVip(int UserID, int PlatForm)
        {
            var list = ITOrm.Utility.Helper.DapperHelper.ExecuteProcedure<ResultModel>("proc_CheckGiveVip", new { UserID, PlatForm });
            return list.FirstOrDefault();
        }
    }
}
