using ITOrm.Utility.Cache;
using ITOrm.Utility.Client;
using ITOrm.Utility.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Utility.Helper
{
    public class BankCardBindHelper
    {
        public static returnBankCardBind Bind(int TypeId, string realName,string IdCard,string BankCard,string Mobile,string CVN2="",string ExpiresYear="",string ExpiresMouth="")
        {
            var model = new returnBankCardBind();
            model.backState = true;
            model.realName = realName;
            model.IdCard = IdCard;
            model.bankCard = BankCard;
            model.mobile = Mobile;
            model.typeId = TypeId;
            model.cvn2 = CVN2;
            model.expiresYear = ExpiresYear;
            model.expiresMouth = ExpiresMouth;
            model.bankName = "工商银行";
            return model;
        }

        public static bool ValidateBank(string bankName, string bankCard)
        {
            var banksbin = MemcachHelper.Get<string>(Constant.list_bank_bin_key, 60 * 24 * 7, () =>
            {
                return HttpHelper.HttpGetHTML("http://api.sujintech.com/html/banksbin.txt");
            });
            if (string.IsNullOrEmpty(banksbin))
            {
                banksbin = HttpHelper.HttpGetHTML("http://api.sujintech.com/html/banksbin.txt");
            }
            var banks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BankBin>>(banksbin);
            bool stop = false;
            string bankN = "";
            foreach (var p in banks)
            {
                foreach (var pattern in p.Patterns)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(bankCard, pattern.Reg, System.Text.RegularExpressions.RegexOptions.Multiline))
                    {
                        bankN = p.BankName;
                        //rstForBin.Code = 1;
                        //rstForBin.Data = new { bankName = p.BankName, bankCode = p.BankCode, cardType = pattern.CardType };
                        //rstForBin.Message = p.BankCode;
                        stop = true;
                        break;
                    }
                }
                if (stop)
                {
                    break;
                }
            }
            return bankN == bankName;
        }


        public static BankBin BankBinto( string bankCard)
        {
            var banksbin = MemcachHelper.Get<string>(Constant.list_bank_bin_key, 60 * 24 * 7, () =>
            {
                return HttpHelper.HttpGetHTML("http://api.sujintech.com/html/banksbin.txt");
            });
            if (string.IsNullOrEmpty(banksbin))
            {
                banksbin = HttpHelper.HttpGetHTML("http://api.sujintech.com/html/banksbin.txt");
            }
            var banks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BankBin>>(banksbin);
            bool stop = false;
            BankBin bin= new BankBin();
            foreach (var p in banks)
            {
                foreach (var pattern in p.Patterns)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(bankCard, pattern.Reg, System.Text.RegularExpressions.RegexOptions.Multiline))
                    {
                        bin.BankName = p.BankName;
                        bin.BankCode = p.BankCode;
                        //rstForBin.Code = 1;
                        //rstForBin.Data = new { bankName = p.BankName, bankCode = p.BankCode, cardType = pattern.CardType };
                        //rstForBin.Message = p.BankCode;
                        stop = true;
                        break;
                    }
                }
                if (stop)
                {
                    break;
                }
            }
            return bin;
        }
    }

    public class BankBin
    {
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public IList<Patterns> Patterns { get; set; }
    }
    public class Patterns
    {
        public string Reg { get; set; }
        public string CardType { get; set; }
    }

    public class returnBankCardBind
    {
        public bool backState { get; set; }
        public string msg { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public string relationId { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankCard { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string realName { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// CVN2
        /// </summary>
        public string cvn2 { get; set; }
        /// <summary>
        /// 到期年
        /// </summary>
        public string expiresYear { get; set; }
        /// <summary>
        /// 到期月
        /// </summary>
        public string expiresMouth { get; set; }

        /// <summary>
        /// 卡类型 0结算卡 1支付卡
        /// </summary>
        public int typeId { get; set; }

        public string bankName { get; set; }
    }
}
