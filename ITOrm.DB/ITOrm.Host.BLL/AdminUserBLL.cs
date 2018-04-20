using ITOrm.Host.DAL;
using ITOrm.Host.Models;
using ITOrm.Core.Utility.Castle;
using System.Collections.Generic;

namespace ITOrm.Host.BLL
{
    public partial class AdminUserBLL
    {

        
        /// <summary>
        /// 根据PID获取列表
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<AdminUser> GetListByPID(int Sex)
        {
            return dal.GetQuery(" Sex=@Sex ", new { Sex = Sex });
        }

        public List<AdminUser> GetList(int top)
        {
            
            return dal.GetQuery(top," 1=1 ","","");
        }


        public bool UpdateName(int id,string name="")
        {
            var model = dal.Single(id);
            if (model != null)
            {
                model.Name = name;
               return dal.Update(model);
            }
            return false;
        }


        public List<AdminUser> GetListByName(string name="")
        {
           return dal.GetQuery(" name like @name ", new  { Name ="%"+ name+"%" });
        }

    }
}
