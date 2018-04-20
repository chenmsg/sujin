using System;

namespace ITOrm.Core.Dapper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WriteAttribute : Attribute
    {
        public WriteAttribute(bool write)
        {
            Write = write;
        }
        public bool Write { get; private set; }
    }
}
