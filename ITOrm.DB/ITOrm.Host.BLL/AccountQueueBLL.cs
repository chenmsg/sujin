using ITOrm.Host.Models;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class AccountQueueBLL
    {

        public ResultModel AccountQueueHandle(int accountQueueId)
        {
            var list= ITOrm.Utility.Helper.DapperHelper.ExecuteProcedure<ResultModel>("proc_AccountQueueHandle", new { accountQueueId });
            return list.FirstOrDefault();
        }
    }
}
