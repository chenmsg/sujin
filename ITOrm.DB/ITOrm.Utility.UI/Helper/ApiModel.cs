using System;
using System.Collections.Generic;

namespace ITOrm.Core.Utility.Helper
{
    public class JsonCommModelList<T>
    {
        public int BackStatus { get; set; }

        public string Msg { get; set; }

        public List<T> Data { get; set; }

        public int TotalCount { get; set; }

        public int PageIndex { get; set; }
    }

    public class JsonCommModel<T>
    {
        public int BackStatus { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public int TotalCount { get; set; }

        public int PageIndex { get; set; }
    }
}
