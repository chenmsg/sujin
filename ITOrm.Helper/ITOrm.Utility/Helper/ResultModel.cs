using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace ITOrm.Utility.Helper
{
    public class ResultModel<T>
    {
        public ResultModel(List<T> t,int totalCount)
        {
            this.list = t;
            this.totalCount = totalCount;
        }
        public List<T> list;
        public int totalCount { get; set; }
    }
    public class ResultModelData<T>
    {
        public int backState { get; set; }
        public string message { get; set; }
        public T Data;
    }

    public class ResultModel
    {
        public ResultModel()
        {

        }
        public ResultModel(JArray list, int totalCount)
        {
            this.list = list;
            this.totalCount = totalCount;
        }
        public ResultModel(JObject data)
        {
            this.data = data;
            this.totalCount = totalCount;
        }
        public JArray list;
        public int totalCount { get; set; }

        public JObject data { get; set; }
        public int backState { get; set; }
        public string message { get; set; }
    }
}
