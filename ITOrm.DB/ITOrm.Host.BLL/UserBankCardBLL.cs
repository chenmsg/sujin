using ITOrm.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Utility.Helper;
using ITOrm.Utility.StringHelper;

namespace ITOrm.Host.BLL
{
    public partial class UserBankCardBLL
    {
        public ResultModel ValidateBank(UserBankCard entity)
        {
            ResultModel result = new ResultModel();
            result.backState = 0;
            result.message = "验证成功";
            if (entity == null)
            {
                result.backState = -100;
                result.message = "卡记录对象为空";
                return result;
            }
            if (!(entity.BankCard.Length > 15 && entity.BankCard.Length < 21))
            {
                result.backState = -100;
                result.message = "银行卡号有误，请核对您的卡号信息";
                return result;
            }
            if (!TypeParse.IsMobile(entity.Mobile))
            {
                result.backState = -100;
                result.message = "预留手机号格式验证失败";
                return result;
            }
            if (string.IsNullOrEmpty(entity.CVN2))
            {
                result.backState = -100;
                result.message = "CVN2未填写";
                return result;
            }
            if (string.IsNullOrEmpty(entity.ExpiresYear)||string.IsNullOrEmpty(entity.ExpiresMouth))
            {
                result.backState = -100;
                result.message = "有效期年月未填写";
                return result;
            }
            return result;

        }
        public  ResultModel ValidateBank(int id)
        {
            var model = Single(id);
            return ValidateBank(model);
        }
    }
}
