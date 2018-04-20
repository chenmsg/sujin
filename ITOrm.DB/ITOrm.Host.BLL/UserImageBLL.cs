using ITOrm.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Host.BLL
{
    public partial class UserImageBLL
    {
        public string GetUrl(int Id)
        {
            UserImage model = Single(Id);
            return model != null && model.ID > 0 ? model.Url : "";
        }

        public string GetUrlAndUpdateState(int Id,int State)
        {
            UserImage model = Single(Id);
            if (model != null && model.ID > 0&& State!=model.State)
            {
                model.State = State;
                Update(model);
            }
            return model != null && model.ID > 0 ? model.Url : "";
        }

        public bool UpdateState(int Id,int State)
        {
            UserImage model = Single(Id);
            if (State == model.State) return true;
            model.State = State;
            return Update(model);
        }
    }
}
