using System;

namespace ITOrm.Core.PetaPoco
{
    // For non-explicit pocos, causes a property to be ignored
    [AttributeUsage(AttributeTargets.Property)]
    public class Ignore : Attribute
    {
    }
}
