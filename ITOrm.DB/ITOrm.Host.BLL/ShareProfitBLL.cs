using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Host.BLL
{
    public partial class ShareProfitBLL
    {
        /// <summary>
        /// 分润
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="VipType"></param>
        /// <param name="PlatForm"></param>
        /// <returns></returns>
        public ResultModel ShareProfit(int PayId)
        {
            var list = ITOrm.Utility.Helper.DapperHelper.ExecuteProcedure<ResultModel>("proc_ShareProfit", new { PayId });
            return list.FirstOrDefault();
        }
    }
}
