using System;

namespace ITOrm.Core.PetaPoco
{
    // For explicit pocos, marks property as a column and optionally supplies column name
    [AttributeUsage(AttributeTargets.Property)]
    public class Column : Attribute
    {
        public Column() { }
        public Column(string name) { Name = name; }
        public string Name { get; set; }
    }
}
