using System;

namespace ITOrm.Core.PetaPoco
{
    // Poco's marked [Explicit] require all column properties to be marked
    [AttributeUsage(AttributeTargets.Class)]
    public class ExplicitColumns : Attribute
    {
    }
}
