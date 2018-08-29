using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using CoreEA.Invalidation;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.Data.OleDb;
using CoreEA.LoginInfo;
using CoreEA.ConnSTR;
using CoreEA.SchemaInfo;
using CoreEA.Exceptions;
using CoreEA.GlobalDefine;
using System.Text.RegularExpressions;
namespace CoreEA.Sqlite
{
    internal sealed class SqliteRobot : BaseRobot
    {

        #region Temp Method

        /// <summary>
        /// Strip any parentheses from the string.
        /// </summary>
        /// <param name="value">The string to strip</param>
        /// <returns>The stripped string</returns>
        private string StripParens(string value)
        {
            Regex rx = new Regex(@"\(([^\)]*)\)");
            Match m = rx.Match(value);
            if (!m.Success)
                return value;
            else
                return StripParens(m.Groups[1].Value);
        }


        /// <summary>
        /// Discards the national prefix if exists (e.g., N'sometext') which is not
        /// supported in SQLite.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string DiscardNational(string value)
        {
            Regex rx = new Regex(@"N\'([^\']*)\'");
            Match m = rx.Match(value);
            if (m.Success)
                return m.Groups[1].Value;
            else
                return value;
        }


        private bool IsSingleQuoted(string value)
        {
            value = value.Trim();
            if (value.StartsWith("'") && value.EndsWith("'"))
                return true;
            return false;
        }

        /// <summary>
        /// Check if the DEFAULT clause is valid by SQLite standards
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsValidDefaultValue(string value)
        {
            if (IsSingleQuoted(value))
                return true;

            double testnum;
            if (!double.TryParse(value, out testnum))
                return false;
            return true;
        }

        /// <summary>
        /// Used when creating the CREATE TABLE DDL. Creates a single row
        /// for the specified column.
        /// </summary>
        /// <param name="col">The column schema</param>
        /// <returns>A single column line to be inserted into the general CREATE TABLE DDL statement</returns>
        private string BuildColumnStatement(BaseColumnSchema col)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t[" + col.ColumnName + "]\t\t");
            sb.Append("[" + col.ColumnType + "]");
            if (col.IsNullable)
                sb.Append(" NULL");
            else
                sb.Append(" NOT NULL");
            string defval = StripParens(col.DefaultValue);
            defval = DiscardNational(defval);

            if (defval != string.Empty && defval.ToUpper().Contains("GETDATE"))
            {
                sb.Append(" DEFAULT (CURRENT_TIMESTAMP)");
            }
            else if (defval != string.Empty && IsValidDefaultValue(defval))
                sb.Append(" DEFAULT (" + col.DefaultValue + ")");

            return sb.ToString();
        }


        /// <summary>
        /// Creates a CREATE INDEX DDL for the specified table and index schema.
        /// </summary>
        /// <param name="tableName">The name of the indexed table.</param>
        /// <param name="indexSchema">The schema of the index object</param>
        /// <returns>A CREATE INDEX DDL (SQLite format).</returns>
        private string BuildCreateIndex(string tableName, BaseIndexSchema indexSchema)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE ");
            if (indexSchema.IsUnique)
                sb.Append("UNIQUE ");
            sb.Append("INDEX [" + tableName + "_" + indexSchema.IndexName + "]\n");
            sb.Append("ON [" + tableName + "]\n");
            sb.Append("(");
            sb.Append("[" + indexSchema.ColumnName + "]");
            if (!indexSchema.IsAscending)
                sb.Append(" DESC");

            //for (int i = 0; i < indexSchema.Columns.Count; i++)
            //{
            //    sb.Append("[" + indexSchema.Columns[i].ColumnName + "]");
            //    if (!indexSchema.Columns[i].IsAscending)
            //        sb.Append(" DESC");
            //    if (i < indexSchema.Columns.Count - 1)
            //        sb.Append(", ");
            //} // for
            sb.Append(")");

            return sb.ToString();
        }

        #endregion 

        private CommandTextBase currentCommandTextHandler;
        /// <summary>
        /// 
        /// </summary>
        public override CommandTextBase CurrentCommandTextHandler
        {
            get
            {
                if (null == currentCommandTextHandler)
                {
                    currentCommandTextHandler = new SqliteCommandText();
                }
                return currentCommandTextHandler;
            }
        }

        public sealed override string GetCreateTableString(BaseTableSchema ts)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("CREATE TABLE " + GetMaskedTableName(ts.TableName) + " (\n");

            for (int i = 0; i < ts.Columns.Count; i++)
            {

                BaseColumnSchema col = ts.Columns[i];
                string cline = BuildColumnStatement(col);
                sb.Append(cline);
                if (i < ts.Columns.Count - 1)
                    sb.Append(",\n");
            } // foreach

            if (ts.PrimaryKey != null && ts.PrimaryKey.Count > 0)
            {
                sb.Append(",\n");
                sb.Append("PRIMARY KEY (");
                for (int i = 0; i < ts.PrimaryKey.Count; i++)
                {
                    sb.Append(GetMaskedColumnName(ts.PrimaryKey[i].ColumnName));
                    if (i < ts.PrimaryKey.Count - 1)
                        sb.Append(", ");
                } // for
                sb.Append(")\n");
            }
            else
            {
                sb.Append("\n");
            }

            sb.Append(");\n");

            // Create any relevant indexes
            //if (ts.Indexes != null)
            //{
            //    for (int i = 0; i < ts.Indexes.Count; i++)
            //    {
            //        string stmt = BuildCreateIndex(ts.TableName, ts.Indexes[i]);
            //        sb.Append(stmt + ";\n");
            //    } // for
            //} // if

            return sb.ToString();
        }

        public sealed override string GetMaskedColumnName(string columnName)
        {
            if (columnName.StartsWith("'"))
            {
                if (columnName.EndsWith("'"))
                {
                    return columnName;
                }
                else
                {
                    return string.Format("{0}'", columnName);
                }
            }
            else
            {
                if (columnName.EndsWith("'"))
                {
                    return string.Format("'{0}", columnName);
                }
                else
                {
                    return string.Format("'{0}'", columnName);
                }

            }
        }

        public sealed override BaseTableSchema GetTableSchemaInfoObject(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            BaseTableSchema tableSchema = new BaseTableSchema();
            tableSchema.TableName = tableName;
            DataTable dt = GetColumnInfoFromTable(tableName);

            foreach (DataRow item in dt.Rows)
            {
                BaseColumnSchema schmeaInfo = new BaseColumnSchema();


                schmeaInfo.ColumnName = item["COLUMN_NAME"].ToString();
                schmeaInfo.ColumnType = item["DATA_TYPE"].ToString();
                //Different
                schmeaInfo.IsAutoIncrement = bool.Parse(item["AUTOINCREMENT"].ToString());

                schmeaInfo.CharacterMaxLength = item["CHARACTER_MAXIMUM_LENGTH"].IsDBNull() ? 0 :
                    Int64.Parse(item["CHARACTER_MAXIMUM_LENGTH"].ToString());
                //As well as set this property
                //This property is the common property
                //About property is special property
                //We recommend use this property rather than above one.
                schmeaInfo.ColumnLength = schmeaInfo.CharacterMaxLength;

                schmeaInfo.DefaultValue = item["COLUMN_DEFAULT"].ToString();

                schmeaInfo.IsNullable = (item["IS_NULLABLE"].ToString().ToLower() == "yes" ? true : false);

                schmeaInfo.NumericPrecision = item["NUMERIC_PRECISION"].IsDBNull() ?
                    0 : int.Parse(item["NUMERIC_PRECISION"].ToString());

                schmeaInfo.NumericScale = item["NUMERIC_SCALE"].IsDBNull() ?
                    0 : int.Parse(item["NUMERIC_SCALE"].ToString());

                schmeaInfo.OrdinalPosition = item["ORDINAL_POSITION"].IsDBNull() ?
0 : int.Parse(item["ORDINAL_POSITION"].ToString());

                tableSchema.Columns.Add(schmeaInfo);
            }

            return tableSchema;
        }

        public sealed override List<BasePrimaryKeyInfo> GetPrimaryKeysFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            return new List<BasePrimaryKeyInfo>();
        }

        public sealed override bool HasIdentityColumnInTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            return false;
        }

        /// <summary>
        /// Not Implement
        /// </summary>
        /// <returns></returns>
        public sealed override List<string> GetSystemViewList()
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            return new List<string>();
        }

        /// <summary>
        /// Not Implement
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetProviderInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            return null;
        }

        public sealed override CoreE.UsedDatabaseType HostedType
        {
            get
            {
                return CoreE.UsedDatabaseType.Sqlite;
            }
        }


        public sealed override int MaxTableNameLength
        {
            get
            {
                return 126;
            }
        }

        public sealed override bool CreateDatabase(BaseLoginInfo loginInfo)
        {
            LoginInfo_Sqlite myInfo = loginInfo as LoginInfo_Sqlite;
            if (string.IsNullOrEmpty(myInfo.DbFile))
            {
                throw new ArgumentException("Db name can't be empty or null");
            }

            try
            {
                SQLiteConnection.CreateFile(myInfo.DbFile);
                return true;
            }
            catch (SQLiteException ee)
            {
                Debug.WriteLine(ee.Message);
                return false;
            }
        }

        public sealed override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {

            throw new NotImplementedException();
        }

        public sealed override DbDataAdapter GetDataAdapter(DbCommand dbCmd)
        {
            SQLiteCommand myCmd = dbCmd as SQLiteCommand;
            if (myCmd == null) throw new ArgumentException();

            SQLiteDataAdapter sa = new SQLiteDataAdapter(myCmd);
            return sa;
        }


        public sealed override DbCommand GetNewStringCommand(string sql)
        {
            DbCommand cmd = new SQLiteCommand();
            if (!string.IsNullOrEmpty(sql))
            {
                cmd.CommandText = sql;
            }

            cmd.Connection = baseConn;
            return cmd;
        }

        public sealed override decimal GetColumnLength(string tableName, string columnName)
        {
            return 65535M;
        }

        public sealed override string GetMaskedTableName(string tableName)
        {
            if (tableName.StartsWith("'"))
            {
                if (tableName.EndsWith("'"))
                {
                    return tableName;
                }
                else
                {
                    return string.Format("{0}'", tableName);
                }
            }
            else
            {
                if (tableName.EndsWith("'"))
                {
                    return string.Format("'{0}", tableName);
                }
                else
                {
                    return string.Format("'{0}'", tableName);
                }

            }
        }


        public sealed override List<string> GetDatabaseList()
        {
            throw new NotImplementedException();
        }


        public sealed override void Open(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Invalid connection string");
            }
            try
            {
                if (IsOpened)
                {
                    return;
                }

                baseConn = new SQLiteConnection(connectionString);
                baseConn.Open();

                invalidator = new InvalidatorSqlite();
            }
            catch (SQLiteException ee)
            {
                throw ee;
            }
        }

        public sealed override void Open(BaseLoginInfo pInfo)
        {
            //Record to base class (Vital)
            baseLoginInfo = pInfo;


            LoginInfo_Sqlite myInfo = pInfo as LoginInfo_Sqlite;
            LoginInfo_ForAllDbTypes allInfo = pInfo as LoginInfo_ForAllDbTypes;

            if ((myInfo == null) && (allInfo == null))
            {
                throw new ArgumentException("Only Support Sqlite login info and AllDBTypes Info");
            }
            if (IsOpened)
            {
                return;
            }

            try
            {

                string connectionStr = string.Empty;
                if (allInfo != null)
                {
                    myInfo = new LoginInfo_Sqlite();
                    myInfo.Pwd = allInfo.Pwd;
                    myInfo.IsUnicode = allInfo.IsUnicode;
                    myInfo.IsReadOnly = false;
                    myInfo.DbFile = allInfo.Database;
                }

                connectionStr = DbConnectionString.Sqlite.Standard(
                    myInfo.DbFile, myInfo.IsReadOnly, myInfo.IsUnicode, myInfo.Pwd);

                baseConn = new SQLiteConnection(connectionStr);

                baseConn.Open();

                CurDatabase = myInfo.DbFile;

                invalidator = new InvalidatorSqlite();
            }
            catch (SQLiteException ee)
            {
                throw ee;
            }
            catch (DbException eee)
            {
                throw eee;
            }
        }

        public sealed override List<string> GetTableListInDatabase(string databaseName)
        {
            List<string> tableList = new List<string>();

            string cmdStr = string.Format("SELECT name FROM SQLITE_MASTER where type='table'");
            DbCommand cmd = GetNewStringCommand(cmdStr);
            DataTable ds = ExecuteDataList(cmd).Tables[0];

            foreach (DataRow item in ds.Rows)
            {
                tableList.Add(item["name"].ToString());
            }

            return tableList;
        }
    }
}
