using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using System.Data.SqlClient;
using System.Security.Permissions;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Access
{
    /// <summary>
    /// 
    /// </summary>
    [SqlClientPermission(SecurityAction.Demand)]
    [DatabaseAssembler(typeof(AccessDatabaseAssembler))]
    public class AccessDatabase : Database
    {
		/// <summary>
        /// 
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
        public AccessDatabase(string connectionString)
			: base(connectionString, SqlClientFactory.Instance)
		{

		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryCommand"></param>
        protected override void DeriveParameters(System.Data.Common.DbCommand discoveryCommand)
        {
            SqlCommandBuilder.DeriveParameters((SqlCommand)discoveryCommand);
        }
    }
}
