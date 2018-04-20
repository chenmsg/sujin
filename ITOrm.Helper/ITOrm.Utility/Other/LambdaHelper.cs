using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Utility.Other
{
    public class LambdaHelper
    {
        public static string ResloveName(LambdaExpression expression)
        {
            var exp = expression.Body as MemberExpression;
            string expStr = exp.ToString();
            return expStr.Substring(expStr.IndexOf(".") + 1);
        }
    }
}
