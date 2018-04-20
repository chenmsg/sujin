using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Utility.Helper
{
    public class DapperHelper
    {
        public static List<T> ExecuteProcedure<T>(string ProcName, object param = null)
        {
            return ITOrm.Core.Dapper.Context.DapperHelper.ExecuteProcedure<T>(ProcName,param);
        }

        /// <summary>
        /// 执行返回第一行第一列值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlContent"></param>
        /// <returns></returns>
        public static T ExecScalarSql<T>(string sqlContent)
        {
            return ITOrm.Core.Dapper.Context.DapperHelper.ExecScalarSql<T>(sqlContent);
        }
    }
}
