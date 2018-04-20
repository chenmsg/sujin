using System;
using System.Data;

namespace ITOrm.Core.Dapper
{
    /// <summary>
    /// Handles variances in features per DBMS
    /// </summary>
    public class FeatureSupport
    {
        private static readonly FeatureSupport
            @default = new FeatureSupport(false),
            postgres = new FeatureSupport(true);

        /// <summary>
        /// Gets the feature set based on the passed connection
        /// </summary>
        public static FeatureSupport Get(IDbConnection connection)
        {
            string name = connection == null ? null : connection.GetType().Name;
            if (string.Equals(name, "npgsqlconnection", StringComparison.InvariantCultureIgnoreCase)) return postgres;
            return @default;
        }
        private FeatureSupport(bool arrays)
        {
            Arrays = arrays;
        }
        /// <summary>
        /// True if the db supports array columns e.g. Postgresql
        /// </summary>
        public bool Arrays { get; private set; }
    }
}
