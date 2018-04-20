﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITOrm.Core.Dapper
{
    /// <summary>
    /// Implement this interface to pass an arbitrary db specific set of parameters to Dapper
    /// </summary>
    public partial interface IDynamicParameters
    {
        /// <summary>
        /// Add all the parameters needed to the command just before it executes
        /// </summary>
        /// <param name="command">The raw command prior to execution</param>
        /// <param name="identity">Information about the query</param>
        void AddParameters(IDbCommand command, Identity identity);
    }
}
