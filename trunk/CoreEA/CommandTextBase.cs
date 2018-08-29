using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEA
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandTextBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="robot"></param>
        internal CommandTextBase(BaseRobot robot)
        {
            currentRobot = robot;
        }

        private BaseRobot currentRobot;
        /// <summary>
        /// Father of this command text 
        /// </summary>
        internal BaseRobot CurrentRobot
        {
            get
            {
                return currentRobot;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldTableName"></param>
        /// <param name="newTableName"></param>
        /// <returns></returns>
        public virtual string GetRenameTableCmdStr(string oldTableName, string newTableName)
        {
            return string.Format("sp_rename '{0}', '{1}'", oldTableName, newTableName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="oldColumnName"></param>
        /// <param name="newColumnName"></param>
        /// <returns></returns>
        public virtual string GetRenameColumnCmdStr(string tableName, string oldColumnName, string newColumnName)
        {
            return string.Format("SP_RENAME '{0}[{1}]' , '[{2}]',  'COLUMN'", tableName, oldColumnName, newColumnName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual string GetDropColumnCmdStr(string tableName, string columnName)
        {
            return string.Format("ALTER TABLE [{0}]  Drop COLUMN {1}", tableName, columnName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newIndexName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual string GetCreateIndexCmdStr(string newIndexName, string tableName, string columnName)
        {
            return string.Format("CREATE UNIQUE INDEX {0} ON [{1}] ({2}) ", newIndexName, tableName, columnName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public virtual string GetDropIndexCmdStr(string tableName, string indexName)
        {
            return string.Format("DROP INDEX {0}.[{1}]", tableName, indexName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual string GetDropTableCmdStr(string tableName)
        {
            return string.Format("DROP TABLE {0}", tableName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual string GetDropTableCmdStrWithCascade(string tableName)
        {
            //cascade constraints
            return string.Format("DROP TABLE {0} cascade", tableName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual string GetResetIdentityColumn(string tableName, string columnName)
        {
           return string.Format("ALTER TABLE {0} ALTER COLUMN {1} IDENTITY(1,1)",
                tableName, columnName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetFixedTableName(string tableName)
        {
            return currentRobot.GetMaskedTableName(tableName);
        }
    }
}
