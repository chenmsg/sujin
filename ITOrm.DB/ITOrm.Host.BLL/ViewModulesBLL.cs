using ITOrm.Host.DAL;
using ITOrm.Host.Models;
using ITOrm.Core.Utility.Castle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Host.BLL
{
    public partial class ViewModulesBLL
    {



        /// <summary>
        /// 获取未删除的所有数据
        /// </summary>
        /// <returns></returns>
        public List<ViewModules> GetList(string ModuleName)
        {
            return dal.GetQuery(" ModuleName = @ModuleName ", new { ModuleName = ModuleName });
        }

        /// <summary>
        /// 根据PID获取列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<ViewModules> GetPaged(IDictionary<string, object> dictionary)
        {
            

            int totalCount=0;
            return dal.GetPaged(2, 1, out totalCount, " ModuleName = @ModuleName ", dictionary);
        }
    }
}
