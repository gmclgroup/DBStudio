using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Access
{
    class AccessDatabaseAssembler : IDatabaseAssembler
    {

        #region IDatabaseAssembler Members

        public Database Assemble(string name, System.Configuration.ConnectionStringSettings connectionStringSettings, Microsoft.Practices.EnterpriseLibrary.Common.Configuration.IConfigurationSource configurationSource)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
