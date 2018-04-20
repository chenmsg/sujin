using ITOrm.Host.Models;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class BankBLL
    {
       
        public string QueryBankName(string BankCode)
        {
            Bank bank = Single(" BankCode=@BankCode", new { BankCode });
            if (bank != null)
            {
                return bank.BankName;
            }
            return "";
        }


        public string QueryBankCode(string BankName)
        {
            Bank bank = Single(" BankName=@BankName", new { BankName });
            if (bank != null)
            {
                return bank.BankCode;
            }
            return "";
        }

    }
}
