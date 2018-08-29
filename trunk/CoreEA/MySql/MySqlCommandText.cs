using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA.MySql
{
    class MySqlCommandText : CommandTextBase
    {
        internal MySqlCommandText() :
            base(new MySqlRobot())
        {
        }

        public sealed override string GetRenameColumnCmdStr(string tableName, string oldColumnName, string newColumnName)
        {
            return string.Format("alter table [{0}] rename column '{1}' to '{2}'", tableName, oldColumnName, newColumnName);
        }

        public sealed override string GetDropColumnCmdStr(string tableName, string columnName)
        {
            return string.Format("ALTER TABLE [{0}]  Drop COLUMN {1}", tableName, columnName);
        }

        public sealed override string GetCreateIndexCmdStr(string newIndexName, string tableName, string columnName)
        {
            return string.Format("CREATE UNIQUE INDEX {0} ON [{1}] ({2}) ", newIndexName, tableName, columnName);
        }

        public sealed override string GetDropIndexCmdStr(string tableName, string indexName)
        {
            return string.Format("DROP INDEX {0}.[{1}]", tableName, indexName);
        }

    }
}
