using System;

namespace ITOrm.Core.PetaPoco
{
    // Specific the primary key of a poco class
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKey : Attribute
    {
        public PrimaryKey(string primaryKey)
        {
            Value = primaryKey;
        }

        public string Value { get; private set; }
    }
}
