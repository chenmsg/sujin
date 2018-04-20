using System;
using System.ComponentModel;
using System.Data;

namespace ITOrm.Core.Dapper
{
    /// <summary>
    /// Not intended for direct usage
    /// </summary>
    [Obsolete("Not intended for direct usage", false)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public static class TypeHandlerCache<T>
    {
        /// <summary>
        /// Not intended for direct usage
        /// </summary>
        [Obsolete("Not intended for direct usage", true)]
        public static T Parse(object value)
        {
            return (T)handler.Parse(typeof(T), value);

        }

        /// <summary>
        /// Not intended for direct usage
        /// </summary>
        [Obsolete("Not intended for direct usage", true)]
        public static void SetValue(IDbDataParameter parameter, object value)
        {
            handler.SetValue(parameter, value);
        }

        internal static void SetHandler(ITypeHandler handler)
        {
            TypeHandlerCache<T>.handler = handler;
        }

        private static ITypeHandler handler;
    }
}
