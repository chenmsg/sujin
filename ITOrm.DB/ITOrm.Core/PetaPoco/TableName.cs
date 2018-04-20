using System;

namespace ITOrm.Core.PetaPoco
{
    // Specify the table name of a poco
    [AttributeUsage(AttributeTargets.Class)]
    public class TableName : Attribute
    {
        public TableName(string tableName)
        {
            Value = tableName;
        }
        public string Value { get; private set; }
    }
}
