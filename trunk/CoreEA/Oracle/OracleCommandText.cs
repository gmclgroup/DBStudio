using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.Oracle
{
    class OracleCommandText : CommandTextBase
    {
        internal OracleCommandText() :
            base(new OracleRobot())
        {
        }

        public override string GetDropTableCmdStrWithCascade(string tableName)
        {
            //cascade constraints
            return string.Format("DROP TABLE {0} cascade constraints", tableName);
        }
    }
}
