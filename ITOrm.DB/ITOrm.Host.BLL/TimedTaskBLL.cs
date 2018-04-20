using ITOrm.Host.Models;
using ITOrm.Utility.Const;
using ITOrm.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ITOrm.Host.BLL
{
    public partial class TimedTaskBLL
    {

        public  int Init(Logic.TimedTaskType task,DateTime execTime,string json)
        {
            TimedTask model = new TimedTask();
            model.TypeId = (int)task;
            model.ExecTime = execTime;
            model.Value = json;
            return Insert(model);
        }

    }
}
