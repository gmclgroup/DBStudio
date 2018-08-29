using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.Sqlite
{
    class SqliteCommandText:CommandTextBase
    {
        internal SqliteCommandText() :
            base (new SqliteRobot())
        {

        }

        public sealed override string GetRenameTableCmdStr(string oldTableName, string newTableName)
        {
            return string.Format("ALTER TABLE {0} RENAME TO {1}", oldTableName, newTableName);
        }

        public sealed override string GetRenameColumnCmdStr(string tableName, string oldColumnName, string newColumnName)
        {
            throw new NotImplementedException();
        }

        public sealed override string GetDropColumnCmdStr(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        public sealed override string GetCreateIndexCmdStr(string newIndexName, string tableName, string columnName)
        {
            throw new NotImplementedException();
        }
        public sealed override string GetDropIndexCmdStr(string tableName, string indexName)
        {
            throw new NotImplementedException();
        }

    }
}
