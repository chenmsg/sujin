using ITOrm.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Host.BLL
{
    public partial  class YeepayLogParasBLL
    {
        public int Init(int LogID, string msg,int InOrOut)
        {
            YeepayLogParas model = new YeepayLogParas();
            model.LogID = LogID;
            model.CTime = DateTime.Now;
            model.InOrOut = InOrOut;
            model.Msg = msg;
            return Insert(model);
        }

    }
}
