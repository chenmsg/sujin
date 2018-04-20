using System;

namespace ITOrm.Core.PetaPoco
{
    // For explicit pocos, marks property as a result column and optionally supplies column name
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultColumn : Column
    {
        public ResultColumn() { }
        public ResultColumn(string name) : base(name) { }
    }
}
