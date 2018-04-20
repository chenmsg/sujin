using System;

namespace ITOrm.Core.Dapper
{
    [AssemblyNeutral, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class AssemblyNeutralAttribute : Attribute { }
}
