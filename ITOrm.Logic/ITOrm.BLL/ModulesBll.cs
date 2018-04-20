using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITOrm.EF.Models;
using ITOrm.EF.Models.Specification;
using ITOrm.EF.Models.Host;
namespace ITOrm.BLL
{
    public class ModulesBll
    {
        /// <summary>
        /// 后台根据请求控制器与action查询模块实体
        /// </summary>
        /// <param name="controllers"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Modules Single(string controllers,string action)
        {
            Modules model = new Modules();
            ISpecification<Modules> spec = SpecificationBuilder.Create<Modules>();
            string path = "/"+controllers+"/"+action;
            spec.Equals(m => m.LinkPath, path);
            //spec.Equals(m => m.UserName, username);
            model = Modules.Single(spec);
            return model;
        }
    }
}
