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

        //public int SystemAccount(decimal Amount)
        //{
        //    AccountQueue model = new AccountQueue();
        //    model.Amount = Amount;
        //    model.CTime = DateTime.Now;
        //    model.InOrOut = 1;
        //    model.Platform = 1;
        //    model.TypeId=
        //}
    }
}
