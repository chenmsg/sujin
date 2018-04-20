using System;
using System.Reflection;

namespace ITOrm.Core.PetaPoco
{
    // Optionally provide and implementation of this to Database.Mapper
    public interface IMapper
    {
        void GetTableInfo(Type t, ref string tableName, ref string primaryKey);
        bool MapPropertyToColumn(PropertyInfo pi, ref string columnName, ref bool resultColumn);
        Func<object, object> GetValueConverter(PropertyInfo pi, Type SourceType);
    }
}
