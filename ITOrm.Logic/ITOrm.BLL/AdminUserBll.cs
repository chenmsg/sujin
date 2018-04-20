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
    public class AdminUserBll
    {

        public AdminUser Login(string username, string password)
        {
            AdminUser model = new AdminUser();
            ISpecification<AdminUser> spec = SpecificationBuilder.Create<AdminUser>();
            spec.Equals(m => m.Password, ITOrm.Utility.Encryption.SecurityHelper.GetMD5String(password));
            spec.Equals(m=>m.UserName,username);
            model = AdminUser.Single(spec);
            return model;
        }
    }
}
