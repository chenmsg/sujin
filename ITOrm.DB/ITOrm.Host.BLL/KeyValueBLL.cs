using ITOrm.Host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITOrm.Utility.Helper;
using ITOrm.Utility.StringHelper;
using ITOrm.Utility.Cache;
using ITOrm.Utility.Const;
using Newtonsoft.Json.Linq;

namespace ITOrm.Host.BLL
{
    public partial class KeyValueBLL
    {
        public string GetAuditingVersion(int cid)
        {
            var serverVersion = "";
            int TypeId = (int)Logic.KeyValueType.平台版本号;
            //通过cid查询APP版本升级信息
            var list = MemcachHelper.Get<List<KeyValue>>(Constant.list_keyvalue_key + TypeId, DateTime.Now.AddDays(7), () =>
            {
                return GetQuery(10, " state<>-1 and typeid=@TypeId ", new { TypeId }, "order by Sort desc,CTime desc");
            });
            list = list.FindAll(m => m.KeyId == cid).OrderByDescending(m => m.Sort).ThenByDescending(m => m.CTime).ToList();

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var data = JObject.Parse(item.Value);
                    if (data["IsAuditing"].ToInt() == 0)
                    {
                        serverVersion = data["version"].ToString();
                    }
                    break;
                }
            }
            return serverVersion;
        }
    }   
}
