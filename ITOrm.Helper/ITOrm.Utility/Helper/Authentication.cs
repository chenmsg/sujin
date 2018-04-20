using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Utility.Helper
{
    public class Authentication
    {
        public static ReturnAuthenticationModel Validate(string IdCard,string RealName)
        {
            var result = new ReturnAuthenticationModel();
            result.backState = true;
            result.msg = "认证成功";
            return result;
        }
    }

    public class ReturnAuthenticationModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool backState { get; set; }
        public string msg { get; set; }

    }
}
