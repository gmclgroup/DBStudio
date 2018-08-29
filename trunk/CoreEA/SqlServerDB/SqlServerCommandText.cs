using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.SqlServerDB
{
    class SqlServerCommandText:CommandTextBase
    {
        internal SqlServerCommandText():
            base (new SqlServerRobot())
        {

        }
    }
}
