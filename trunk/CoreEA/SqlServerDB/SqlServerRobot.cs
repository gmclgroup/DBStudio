using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using CoreEA.Invalidation;
using System.Text.RegularExpressions;
using CoreEA.ConnSTR;
using System.Data.Common;
using System.Data.SqlClient;
using CoreEA.Exceptions;
using CoreEA.LoginInfo;
using CoreEA.Args;
using CoreEA.SchemaInfo;
using CoreEA.GlobalDefine;
using System.Linq;
using CoreEA.SqlServerDB;
namespace CoreEA
{
    internal class SqlServerRobot : BaseRobot
    {

        //private readonly string SeperateLine = "GO" + Environment.NewLine;
        //Here the seperate is incorrect when execute in sql command or dbcommand
        //But it is fine when execute in sql express management studio
        private readonly string SeperateLine = Environment.NewLine;

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
                    currentCommandTextHandler = new SqlServerCommandText();
                }
                return currentCommandTextHandler;
            }
        }

        public sealed override bool AddColumnToTable(string tableName, BaseColumnSchema columnSchema)
        {
            bool ret = false;
            try
            {
                string sqlCmd = string.Format("Alter table {0} add {1} {2} ",
                    tableName, columnSchema.ColumnName, columnSchema.ColumnType);
                if (columnSchema.CharacterMaxLength != 0)
                {
                    sqlCmd += string.Format("({0})", columnSchema.CharacterMaxLength);
                }
                DoExecuteNonQuery(sqlCmd);

                ret = true;
            }
            catch (Exception ee)
            {
                throw ee;
            }

            return ret;
        }

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
        /// <param name="eachColumn">The column schema</param>
        /// <returns>A single column line to be inserted into the general CREATE TABLE DDL statement</returns>
        private string BuildColumnStatement(BaseColumnSchema eachColumn)
        {
            //This way can be used when create simple schema from SSCE
            //BUT when the basecolumnschema is from sqlserver or other db type
            //This way is not powerful to create .

            #region Not Completed Way
            //StringBuilder sb = new StringBuilder();
            //sb.Append("\t[" + col.ColumnName + "]\t\t");
            //sb.Append("[" + col.ColumnType + "]");
            //if (col.IsNullable)
            //    sb.Append(" NULL");
            //else
            //    sb.Append(" NOT NULL");
            //string defval = StripParens(col.DefaultValue);
            //defval = DiscardNational(defval);

            //if (defval != string.Empty && defval.ToUpper().Contains("GETDATE"))
            //{
            //    sb.Append(" DEFAULT (CURRENT_TIMESTAMP)");
            //}
            //else if (defval != string.Empty && IsValidDefaultValue(defval))
            //    sb.Append(" DEFAULT (" + col.DefaultValue + ")");

            //return sb.ToString();

            //Here should convert to SSCEColumnSchema
            //because here we only support sync data to SSCE currently
            //Maybe will refactor ,but not sure.
            #endregion

            StringBuilder createSchemaScript = new StringBuilder();
            switch (eachColumn.ColumnType.ToLower())
            {
                case "char":
                    eachColumn.ColumnType = "nchar";
                    break;
                case "varchar":
                    eachColumn.ColumnType = "nvarchar";
                    break;
                case "text":
                    eachColumn.ColumnType = "ntext";
                    break;
            }

            switch (eachColumn.ColumnType.ToLower())
            {
                case "nvarchar":
                    createSchemaScript.AppendFormat("[{0}] {1}({2}) {3} {4} {5} ",
                         eachColumn.ColumnName,
                         eachColumn.ColumnType,
                         eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "nchar":
                    createSchemaScript.AppendFormat("[{0}] {1}({2}) {3} {4} {5}",
                        eachColumn.ColumnName,
                        "nchar",
                        eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "numeric":
                    createSchemaScript.AppendFormat("[{0}] {1}({2},{3}) {4} {5} {6} ",
                         eachColumn.ColumnName,
                         eachColumn.ColumnType,
                         eachColumn.NumericPrecision,
                         eachColumn.NumericScale,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                //Not support such type
                case "timestamp":

                    break;
                case "enum":

                    break;
                default:
                    createSchemaScript.AppendFormat("[{0}] {1} {2} {3} {4}{5} {6} ",
                         eachColumn.ColumnName,
                         eachColumn.ColumnType,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         eachColumn.RowGuidCol ? "ROWGUIDeachColumn" : string.Empty,
                         (eachColumn.AutoIncrementBy > 0 ? string.Format("IDENTITY ({0},{1})", eachColumn.AutoIncrementSeed, eachColumn.AutoIncrementBy) : string.Empty),
                         System.Environment.NewLine);
                    break;
            }

            return createSchemaScript.ToString();

        }


        /// <summary>
        /// Creates a CREATE INDEX DDL for the specified table and index schema.
        /// </summary>
        /// <param name="tableName">The name of the indexed table.</param>
        ///<param name="indexSchemas">The schema of the index object</param>
        /// <returns>A CREATE INDEX DDL (SQLite format).</returns>
        private string BuildCreateIndex(string tableName, List<BaseIndexSchema> indexSchemas)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("CREATE ");
            //if (indexSchema.IsUnique)
            //    sb.Append("UNIQUE ");
            //sb.Append("INDEX [" + tableName + "_" + indexSchema.IndexName + "]\n");
            //sb.Append("ON [" + tableName + "]\n");
            //sb.Append("(");
            //sb.Append("[" + indexSchema.ColumnName + "]");
            //if (!indexSchema.IsAscending)
            //    sb.Append(" DESC");

            ////for (int i = 0; i < indexSchema.Columns.Count; i++)
            ////{
            ////    sb.Append("[" + indexSchema.Columns[i].ColumnName + "]");
            ////    if (!indexSchema.Columns[i].IsAscending)
            ////        sb.Append(" DESC");
            ////    if (i < indexSchema.Columns.Count - 1)
            ////        sb.Append(", ");
            ////} // for
            //sb.Append(")");

            //return sb.ToString();


            if (indexSchemas.Count > 0)
            {
                IEnumerable<string> uniqueIndexNameList = indexSchemas.Select(i => i.IndexName).Distinct();

                foreach (string uniqueIndexName in uniqueIndexNameList)
                {
                    string name = uniqueIndexName;
                    IOrderedEnumerable<BaseIndexSchema> indexesByName = from i in indexSchemas
                                                                        where i.IndexName == name
                                                                        orderby i.OrdinalPosition
                                                                        select i;

                    sb.Append("CREATE ");

                    // Just get the first one to decide whether it's unique and/or clustered index
                    var idx = indexesByName.First();
                    if (idx.IsUnique)
                        sb.Append("UNIQUE ");
                    if (idx.IsClustered)
                        sb.Append("CLUSTERED ");

                    sb.AppendFormat("INDEX [{0}] ON [{1}] (", idx.IndexName, idx.TableName);

                    foreach (BaseIndexSchema col in indexesByName)
                    {
                        sb.AppendFormat("[{0}] {1},", col.ColumnName, col.IsAscending ? "Asc" : "Desc");
                    }

                    // Remove the last comma
                    sb.Remove(sb.Length - 1, 1);
                    sb.AppendLine(");");
                    sb.Append(SeperateLine);


                }
            }

            return sb.ToString();
        }

        #endregion

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
                    {
                        sb.Append(", ");
                    }
                } // for
                sb.Append(")\n");
            }
            else
                sb.Append("\n");

            sb.Append(");\n");

            string stmt = BuildCreateIndex(ts.TableName, ts.Indexes);
            sb.Append(stmt);

            return sb.ToString();
        }

        public sealed override string GetMaskedColumnName(string columnName)
        {
            if (columnName.StartsWith("["))
            {
                if (columnName.EndsWith("]"))
                {
                    return columnName;
                }
                else
                {
                    return string.Format("{0}]", columnName);
                }
            }
            else
            {
                if (columnName.EndsWith("]"))
                {
                    return string.Format("[{0}", columnName);
                }
                else
                {
                    return string.Format("[{0}]", columnName);
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
            try
            {
                Dictionary<string, List<string>> identifyColumns = GetIdentifyColumnsFromCurrentDatabase();

                tableSchema.TableName = tableName;
                tableSchema.PrimaryKey = GetPrimaryKeysFromTable(tableName);
                #region Column Info
                DataTable dt = GetColumnInfoFromTable(tableName);

                foreach (DataRow item in dt.Rows)
                {
                    BaseColumnSchema schmeaInfo = new BaseColumnSchema();

                    schmeaInfo.ColumnName = item["COLUMN_NAME"].ToString();
                    schmeaInfo.ColumnType = item["DATA_TYPE"].ToString();
                    Debug.WriteLine(schmeaInfo.ColumnType);
                    //schmeaInfo.AutoIncrementSeed = (item["AUTOINC_SEED"].IsDBNull() ? 0 : 1);

                    schmeaInfo.CharacterMaxLength = item["CHARACTER_MAXIMUM_LENGTH"].IsDBNull() ? 0 :
                        Int64.Parse(item["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    //As well as set this property
                    //This property is the common property
                    //About property is special property
                    //We recommend use this property rather than above one.
                    schmeaInfo.ColumnLength = schmeaInfo.CharacterMaxLength;

                    schmeaInfo.DefaultValue = item["COLUMN_DEFAULT"].ToString();
                    //Different
                    //schmeaInfo.IsNullable = (item["IS_NULLABLE"].ToString().ToLower() == "yes" ? true : false);
                    schmeaInfo.IsNullable = (item["IS_NULLABLE"].ToString().ToLower() == "yes" ? true : false);

                    schmeaInfo.NumericPrecision = item["NUMERIC_PRECISION"].IsDBNull() ?
                        0 : int.Parse(item["NUMERIC_PRECISION"].ToString());

                    schmeaInfo.NumericScale = item["NUMERIC_SCALE"].IsDBNull() ?
                        0 : int.Parse(item["NUMERIC_SCALE"].ToString());

                    schmeaInfo.OrdinalPosition = item["ORDINAL_POSITION"].IsDBNull() ?
                                0 : int.Parse(item["ORDINAL_POSITION"].ToString());

                    if (identifyColumns.ContainsKey(tableName))
                    {
                        schmeaInfo.IsIdentity = identifyColumns[tableName].Contains(schmeaInfo.ColumnName);
                    }

                    tableSchema.Columns.Add(schmeaInfo);
                }
                #endregion

                #region Index Info
                DataTable dtForIndex = GetIndexInfoFromTable(tableName);

                //#if DEBUG 
                //                dtForIndex.WriteXml("C:\\test111.xml");
                //#endif

                foreach (DataRow row in dtForIndex.Rows)
                {
                    BaseIndexSchema indexInfo = new BaseIndexSchema();
                    indexInfo.IndexName = row["INDEX_NAME"].ToString();
                    indexInfo.TableName = row["TABLE_NAME"].ToString();
                    tableSchema.Indexes.Add(indexInfo);
                }
                #endregion Endof IndexInfo
            }

            catch (Exception ee)
            {
                throw ee;
            }

            return tableSchema;
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
                return CoreE.UsedDatabaseType.SqlServer;
            }
        }


        public sealed override int MaxTableNameLength
        {
            get
            {
                return 126;
            }
        }


        public override bool CreateDatabase(BaseLoginInfo loginInfo)
        {
            bool ret = false;
            LoginInfo_SqlServer info=loginInfo as LoginInfo_SqlServer;

            try
            {
                string cmdStr = string.Format("Create Database {0} on (NAME={1},FILENAME='{2}',SIZE={3}MB,FILEGROWTH={4}MB) log on (NAME={0},FILENAME='{5}',SIZE={3}MB,FILEGROWTH={4}MB)",
                    info.CreateDatabaseObject.DbName,
                    info.CreateDatabaseObject.DbName+"xxssyy",
                    info.CreateDatabaseObject.DbLocation,
                    info.CreateDatabaseObject.InitSize,
                    info.CreateDatabaseObject.FileGrowth,
                    info.CreateDatabaseObject.DbLogFileLocation);

                DbCommand cmd = GetNewStringCommand(cmdStr);

                Debug.WriteLine("Create database sql \r\n" + cmdStr);
                cmd.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ee)
            {

                throw ee;
            }
            return ret;
        }

        public sealed override DbDataAdapter GetDataAdapter(DbCommand dbCmd)
        {
            SqlDataAdapter sa = new SqlDataAdapter((SqlCommand)dbCmd);
            return sa;
        }


        public sealed override DbCommand GetNewStringCommand(string sql)
        {
            DbCommand cmd = new SqlCommand();
            if (!string.IsNullOrEmpty(sql))
            {
                cmd.CommandText = sql;
            }

            cmd.Connection = baseConn;
            return cmd;
        }


        public sealed override string GetMaskedTableName(string tableName)
        {
            if (tableName.StartsWith("["))
            {
                if (tableName.EndsWith("]"))
                {
                    return tableName;
                }
                else
                {
                    return string.Format("{0}]", tableName);
                }
            }
            else
            {
                if (tableName.EndsWith("]"))
                {
                    return string.Format("[{0}", tableName);
                }
                else
                {
                    return string.Format("[{0}]", tableName);
                }

            }
        }

        #region Basic Method

        /// <summary>
        /// When use SqlServerExpress need some notices about ServerName .
        /// LIke
        /// "Data Source=DEVUSER-PC\\SQLEXPRESS;Initial Catalog=Infuture_JonnaCloneSite;User Id=DEVUSER-PC\\Administrator;Password=111111;Trusted_Connection=True");
        /// </summary>
        /// <param name="connectionString"></param>
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
                    throw new ConnectErrorException();
                }
              
               // baseConn = new SqlConnection(connectionString);
                //baseConn = new OleDbConnection(connectionString);
                baseConn.Open();

                invalidator = new Invalidation.InvalidatorForSqlServer();

            }
            catch (Exception ee)
            {
                try
                {
                    baseConn = null;

                    baseConn = new OleDbConnection(connectionString);
                    baseConn.Open();
                    invalidator = new Invalidation.InvalidatorForSqlServer();
                }
                catch(Exception me2)
                {
                    throw me2;
                }
                
            }
        }



        /// <summary>
        /// 
        /// When to use trust connection string (Connect to  online )
        /// When to ues not trust connection string (Connect to remote or local)
        /// 
        /// </summary>
        /// <param name="pInfo"></param>
        public sealed override void Open(BaseLoginInfo pInfo)
        {

            //Record to base class (Vital)
            baseLoginInfo = pInfo;

            LoginInfo_SqlServer myInfo = pInfo as LoginInfo_SqlServer;
            LoginInfo_ForAllDbTypes allInfo = pInfo as LoginInfo_ForAllDbTypes;

            if ((myInfo == null) && (allInfo == null))
            {
                throw new ArgumentException("Only Support Sqlserver login info and AllDBTypes Info");
            }

            if (IsOpened)
            {
                return;
            }

            string myConnString = string.Empty;

            if (allInfo != null)
            {
                myInfo = new LoginInfo_SqlServer();
                myInfo.IsTrustedConn = allInfo.IsTrustedConn;
                myInfo.X_Database = allInfo.Database;
                myInfo.X_Pwd = allInfo.Pwd;
                myInfo.X_Server = allInfo.Server;
                myInfo.X_TableName = allInfo.TableName;
                myInfo.X_UserName = allInfo.Username;
                myInfo.X_CurDbConnectionMode = allInfo.CurConnMode;
                myInfo.AttchFile = allInfo.AttachedFileName;
            }

            if (myInfo.IsTrustedConn)
            {
                switch (myInfo.X_CurDbConnectionMode)
                {
                    case CurDbServerConnMode.Local:
                        myConnString = DbConnectionString.Sqlserver.GetConnectionString(myInfo.X_Server, myInfo.X_UserName, myInfo.X_Pwd, myInfo.X_Database);
                        break;

                    case CurDbServerConnMode.Standard:
                        myConnString = DbConnectionString.Sqlserver.Standard_WithTrustOrNot(myInfo.X_Server, myInfo.X_Database, myInfo.X_UserName, myInfo.X_Pwd, true);
                        break;
                    case CurDbServerConnMode.SqlServer2005Express:
                        myConnString = DbConnectionString.Sqlserver.GetSqlServerExpressCS(myInfo.X_Server, myInfo.X_Database, myInfo.X_UserName, myInfo.X_Pwd, myInfo.IsTrustedConn);
                        break;
                    case CurDbServerConnMode.SqlServer2000:
                        myConnString = DbConnectionString.Sqlserver.SqlServerConnectionString(myInfo.X_Server, myInfo.X_UserName, myInfo.X_Pwd, myInfo.X_Database);
                        break;
                    case CurDbServerConnMode.OleDb:
                        myConnString = DbConnectionString.Sqlserver.GetOledbConnectionString_Trust(myInfo.X_Server, myInfo.X_UserName, myInfo.X_Pwd, myInfo.X_Database);
                        break;
                    case CurDbServerConnMode.AttachFile:
                       // myConnString = DbConnectionString.Sqlserver.AttachFile(myInfo.X_Server, myInfo.AttchFile);
                        myConnString = DbConnectionString.Sqlserver.AttachFileEx(myInfo.X_Server, myInfo.AttchFile);
                        break;
                    case CurDbServerConnMode.SqlServer2008Express:
                        myConnString = DbConnectionString.Sqlserver.GetSqlServer2008_Trust(myInfo.X_Server, myInfo.X_Database);
                        break;
                    case CurDbServerConnMode.SqlServer2005:
                        myConnString = DbConnectionString.Sqlserver.Connection_Mars(myInfo.X_Server, myInfo.X_Database);
                        break;
                }
            }
            else
            {
                switch (myInfo.X_CurDbConnectionMode)
                {
                    case CurDbServerConnMode.OleDb:
                        myConnString = DbConnectionString.Sqlserver.GetOledbConnectionString_NoTrust(myInfo.X_Server, myInfo.X_UserName, myInfo.X_Pwd, myInfo.X_Database);
                        break;
                    case CurDbServerConnMode.Local:
                        myConnString = DbConnectionString.Sqlserver.GetConnectionString(myInfo.X_Server, myInfo.X_UserName, myInfo.X_Pwd, myInfo.X_Database);
                        break;
                    case CurDbServerConnMode.Standard:
                        myConnString = DbConnectionString.Sqlserver.Standard_WithTrustOrNot(myInfo.X_Server, myInfo.X_Database, myInfo.X_UserName, myInfo.X_Pwd, false);
                        break;
                    case CurDbServerConnMode.SqlServer2000:
                        myConnString = DbConnectionString.Sqlserver.SqlServerConnectionString(myInfo.X_Server, myInfo.X_UserName, myInfo.X_Pwd, myInfo.X_Database);
                        break;
                    case CurDbServerConnMode.SqlServer2005Express:
                        myConnString = DbConnectionString.Sqlserver.GetSqlServerExpressCS(myInfo.X_Server, myInfo.X_Database, myInfo.X_UserName, myInfo.X_Pwd, myInfo.IsTrustedConn);
                        break;
                    case CurDbServerConnMode.AttachFile:
                        throw new Exception("Attach File mode need TrustConnection");

                    case CurDbServerConnMode.SqlServer2008Express:
                        myConnString = DbConnectionString.Sqlserver.GetSqlServer2008_StandardSecurity(
                            myInfo.X_Server, myInfo.X_Database, myInfo.X_UserName, myInfo.X_Pwd
                            );
                        break;
                    case CurDbServerConnMode.SqlServer2005:
                        myConnString = DbConnectionString.Sqlserver.Connection_Mars_NoTrust(
                            myInfo.X_Server, myInfo.X_Database, myInfo.X_UserName, myInfo.X_Pwd);
                        break;
                }

            }

            try
            {
                baseConn = new SqlConnection(myConnString);
                baseConn.Open();

                CurDatabase = myInfo.X_Database;
                CurPwd = myInfo.X_Pwd;

                invalidator = new InvalidatorForSqlServer();
            }
            catch (DataException ee)
            {
                throw ee;
            }
        }

        #endregion Endof Basic Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public override bool CreateTable(BaseTableSchema tableSchema)
        {
            bool ret = false;

            string cmdText = GetCreateTableString(tableSchema);
            SqlCommand cmd = (SqlCommand)GetNewStringCommand(cmdText);

            try
            {
                cmd.ExecuteNonQuery();
                ret = true;
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message);
#if DEBUG
                throw ee;
#else
                 return false;
#endif

            }
            return ret;
        }

        public sealed override List<string> GetColumnNameListFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            List<string> columnList = new List<string>();
            DataTable tempDs = this.GetColumnInfoFromTable(tableName);
            try
            {
                foreach (DataRow row in tempDs.Rows)
                {
                    //I'd recommend using row["name"] for the column name row . 
                    columnList.Add(row[0].ToString());
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return columnList;
        }

        public sealed override List<string> GetIndexNameListFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            List<string> columnList = new List<string>();
            DataTable tempDs = this.GetIndexInfoFromTable(tableName);
            try
            {
                foreach (DataRow row in tempDs.Rows)
                {
                    columnList.Add(row[0].ToString());
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return columnList;
        }

        /// <summary>
        /// Get all indexes info 
        /// 
        ///<id>91147370</id>
        ///<status>18450</status>
        ///<first>pwAAAAEA</first>
        ///<indid>1</indid>
        ///<root>2QAAAAEA</root>
        ///<minlen>208</minlen>
        ///<keycnt>1</keycnt>
        ///<groupid>1</groupid>
        ///<dpages>1</dpages>
        ///<reserved>3</reserved>
        ///<used>3</used>
        ///<rowcnt>9</rowcnt>
        ///<rowmodctr>0</rowmodctr>
        ///<reserved3>0</reserved3>
        ///<reserved4>0</reserved4>
        ///<xmaxlen>1227</xmaxlen>
        ///<maxirow>28</maxirow>
        ///<OrigFillFactor>90</OrigFillFactor>
        ///<StatVersion>0</StatVersion>
        ///<reserved2>0</reserved2>
        ///<FirstIAM>wQAAAAEA</FirstIAM>
        ///<impid>0</impid>
        ///<lockflags>0</lockflags>
        ///<pgmodctr>0</pgmodctr>
        ///<keys>OAE4AAQACgAAAAAAAAAAAAEAAQAAAAAABAABAAAAAAA=</keys>
        ///<name>PK_InnerMsgContent</name>
        ///<statblob>BAAAAG5+EACHmwAACQAAAAAAAAAJAA
        ///    AAAAAAADmO4z05juM9AAAAAAAAAAAAAAAAAAA
        ///    AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        ///        AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        ///        AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        ///            AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        ///            ACAAAACQAAAAEAAAAUAAAAAACAQAAAEEEAAAAAAACAQAAA
        ///                AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        ///               AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        ///                   AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        ///                    AAAAAAAAAAAAAXAAAAAAAAAAAAAAAAAAAAAAAQABQAAACAPwAAAAAAAAAAAA
        ///                        AAAAQAABAAFAAAAIA/AADgQAAAgD8IAAAABAAA</statblob>
        ///<maxlen>8000</maxlen>
        ///<rows>9</rows>
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetIndexInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            DataTable tempDs = new DataTable();
            try
            {
                #region Method1
                //DbCommand cmd = this.GetStringCommand(
                //string.Format("select * from sysindexes where id=object_id('{0}')", tableName));
                #endregion


                //<constraint_catalog>Cyma_Sap</constraint_catalog> 
                //<constraint_schema>Cyma_Sap_f</constraint_schema> 
                //<constraint_name>PK_InnerMsgContent</constraint_name> 
                //<table_catalog>Cyma_Sap</table_catalog> 
                //<table_schema>Cyma_Sap_f</table_schema> 
                //<table_name>InnerMsgContent</table_name> 
                //<index_name>PK_InnerMsgContent</index_name> 
                //</Indexes>

                DataTable dt = baseConn.GetSchema("Indexes", new string[] { null, null, tableName, null });
                tempDs = dt;

                #region Method3
                //string cmdStr = string.Format("SELECT * FROM INFORMATION_SCHEMA.INDEXES where Table_Name='{0}'", tableName);
                //DbCommand cmd = this.GetStringCommand(cmdStr);
                //tempDs = this.ExecuteDataList(cmd);
                #endregion
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tempDs;
        }


        /// <summary>
        /// Get all columns info
        /// Columns has : 
        /// 
        ///     <name>InnerMsgID</name>
        ///    <id>91147370</id>
        ///    <xtype>56</xtype>
        ///   <typestat>1</typestat>
        ///   <xusertype>56</xusertype>
        ///   <length>4</length>
        ///  <xprec>10</xprec>
        ///   <xscale>0</xscale>
        ///   <colid>1</colid>
        ///   <xoffset>4</xoffset>
        ///   <bitpos>0</bitpos>
        ///   <reserved>0</reserved>
        ///   <colstat>0</colstat>
        ///   <cdefault>0</cdefault>
        ///  <domain>0</domain>
        /// <number>0</number>
        /// <colorder>1</colorder>
        /// <offset>2</offset>
        ///   <language>0</language>
        ///   <status>0</status>
        ///   <type>56</type>
        ///   <usertype>7</usertype>
        ///   <prec>10</prec>
        ///  <scale>0</scale>
        ///  <iscomputed>0</iscomputed>
        ///  <isoutparam>0</isoutparam>
        ///  <isnullable>0</isnullable>
        ///  
        /// The xml will be like following :
        /// Please notice : the table name in it does not need mask 
        /// Just here
        /// <TABLE_CATALOG>ECCDatabase</TABLE_CATALOG>
        /// <TABLE_SCHEMA>dbo</TABLE_SCHEMA>
        ///  <TABLE_NAME>My TA</TABLE_NAME>
        ///  <COLUMN_NAME>id</COLUMN_NAME>
        ///  <ORDINAL_POSITION>1</ORDINAL_POSITION>
        ///  <IS_NULLABLE>YES</IS_NULLABLE>
        ///  <DATA_TYPE>int</DATA_TYPE>
        ///  <NUMERIC_PRECISION>10</NUMERIC_PRECISION>
        ///  <NUMERIC_PRECISION_RADIX>10</NUMERIC_PRECISION_RADIX>
        ///  <NUMERIC_SCALE>0</NUMERIC_SCALE>
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetColumnInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            DataTable tempDs = null;
            try
            {
#if DEBUG
                DataTable tempTable = this.ExecuteDataList(this.GetNewStringCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS")).Tables[0];
                tempTable.TableName = "dfadfa";
                tempTable.WriteXml("AllColumnSchemaInfo.xml");
#endif
                //Please pay attention to this . the table name should be without the mask 
                string cmdStr = string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS where Table_Name='{0}'", tableName);

                //string cmdStr = string.Format("SELECT INFORMATION_SCHEMA.COLUMNS FROM INFORMATION_SCHEMA.COLUMNS C,INFORMATION_SCHEMA.TABLES t where t.Table_Name=C.Table_Name and c.Table_Name='{0}'", tableName);
                DbCommand cmd = this.GetNewStringCommand(cmdStr);

                tempDs = this.ExecuteDataList(cmd).Tables[0];

                Debug.WriteLine("Retrieve column info ,count is" + tempDs.Rows.Count);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tempDs;
        }

        /// <summary>
        /// Load all the identify column in each table of current databases
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<string>> GetIdentifyColumnsFromCurrentDatabase()
        {
            Dictionary<string, List<string>> ret = new Dictionary<string, List<string>>();

            string cmdStr = string.Format("select * from INFORMATION_SCHEMA.COLUMNS a where	columnproperty(object_id(a.TABLE_SCHEMA+'.'+a.TABLE_NAME),a.COLUMN_NAME,'IsIdentity') =1");

            DbCommand cmd = this.GetNewStringCommand(cmdStr);

            foreach (DataRow row in this.ExecuteDataList(cmd).Tables[0].Rows)
            {
                string tableName = row["TABLE_NAME"].ToString();

                if (!ret.ContainsKey(tableName))
                {
                    ret[tableName] = new List<string>();
                }

                ret[tableName].Add(row["COLUMN_NAME"].ToString());
            }

            return ret;
        }


        /// <summary>
        /// This method currenmly return ONly Text Type Column Data length
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columName"></param>
        /// <returns></returns>
        public sealed override Decimal GetColumnLength(string tableName, string columName)
        {
            Decimal result = 8;
            DataTable ds = GetColumnInfoFromTable(tableName);
            foreach (DataRow item in ds.Rows)
            {
                if (item["COLUMN_NAME"].ToString() == columName)
                {
                    try
                    {
                        result = Decimal.Parse(item["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    }
                    catch
                    {
                        result = 65535;
                    }
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="columnValue"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public sealed override bool InsertData(string tableName, List<string> columnName, List<object> columnValue, List<DbType> types)
        {
            bool result = false;

            try
            {
                StringBuilder st = new StringBuilder();
                StringBuilder st2 = new StringBuilder();

                int count = columnName.Count;
                int j = 0;
                foreach (string item in columnName)
                {
                    st.Append(item);

                    st2.Append(String.Format("@in{0}", item));

                    j++;
                    if (j < count)
                    {
                        st.Append(",");
                        st2.Append(",");
                    }
                }

                using (DbCommand cmd = GetNewCommand())
                {
                    cmd.CommandText = string.Format("Insert into {0} ({1}) values ({2})", tableName, st.ToString(), st2.ToString());
                    for (int i = 0; i < columnName.Count; i++)
                    {
                        this.AddParameters(cmd, this.CreateParameter(String.Format("@in{0}", columnName[i]), types[i], 65535, columnValue[i]));
                    }

                    cmd.ExecuteNonQuery();
                }
                result = true;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return result;
        }


        /// <summary>
        ///<name>aspnet_Membership_FindUsersByEmail</name>
        ///<id>2099048</id>
        ///<xtype>P </xtype>
        ///<uid>5</uid>
        ///<info>0</info>
        ///<status>536870913</status>
        ///<base_schema_ver>0</base_schema_ver>
        ///<replinfo>0</replinfo>
        ///<parent_obj>0</parent_obj>
        ///<crdate>2008-05-26T13:44:46.333+08:00</crdate>
        ///<ftcatid>0</ftcatid>
        ///<schema_ver>0</schema_ver>
        ///<stats_schema_ver>0</stats_schema_ver>
        ///<type>P </type>
        ///<userstat>0</userstat>
        ///<sysstat>4</sysstat>
        ///<indexdel>0</indexdel>
        ///<refdate>2008-05-26T13:44:46.333+08:00</refdate>
        ///<version>0</version>
        ///<deltrig>0</deltrig>
        ///<instrig>0</instrig>
        ///<updtrig>0</updtrig>
        ///<seltrig>0</seltrig>
        ///<category>0</category>
        ///<cache>0</cache>
        /// </summary>
        /// <returns></returns>
        public sealed override List<BaseStoredProcedureInfo> GetStoredProceduresList()
        {
            DataTable tempDs = null;
            List<BaseStoredProcedureInfo> info = new List<BaseStoredProcedureInfo>();
            try
            {
                DbCommand cmd = this.GetNewStringCommand("select * from sysobjects where xtype = 'P'");

                tempDs = this.ExecuteDataList(cmd).Tables[0];

                info = new List<BaseStoredProcedureInfo>();
                foreach (DataRow item in tempDs.Rows)
                {
                    info.Add(new BaseStoredProcedureInfo()
                    {
                        ProcedureName = item["name"].ToString()
                    });
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return info;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override List<string> GetDatabaseList()
        {
            List<string> jieguo = new List<string>();
            SqlDataReader reader = null;
            if (!IsOpened) { throw new ConnectErrorException(); }

            SqlCommand SqlCom = GetNewCommand() as SqlCommand;
            SqlCom.CommandType = CommandType.StoredProcedure;
            SqlCom.CommandText = "sp_databases";
            reader = SqlCom.ExecuteReader();

            while (reader.Read())
            {
                jieguo.Add(reader.GetString(0));
            }

            return jieguo;
        }


        /// <summary>
        /// Get the table list in given database
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public sealed override List<string> GetTableListInDatabase(string dbName)
        {
            List<string> jieguo = null;
            SqlDataReader reader = null;
            if (invalidator.IsInvalidArguments(dbName))
            {
                return null;
            }
            if (!IsOpened) { throw new ConnectErrorException(); }
            //string cmdStr = "SELECT TableNmae = O.name, ColName = C.name, Type = T.name FROM sysobjects O, syscolumns C, systypes T WHERE O.id = C.id AND C.xusertype = T.xusertype ORDER BY TableName, ColName";

            string cmdStr = @"select sysobjects.* from sysobjects where xtype='u'";
            //string cmdStr = "select * from MyTable";
            using (SqlCommand cmd = (SqlCommand)GetNewStringCommand(cmdStr))
            {
                cmd.CommandTimeout = 30;
                //If you wanna Update DataAdapter You must specfie the UpdataCommand/InsertCommand/DeleteCommand etc.                
                try
                {
                    jieguo = new List<string>();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        jieguo.Add(reader[0].ToString());
                    }
                }
                catch (Exception ee)
                {
#if DEBUG
                    Debug.WriteLine(ee.Message);
                    Debug.WriteLine(ee.StackTrace);
                    throw ee;
#else
                    GlobalDefine.SP.LastErrorMsg = ee.Message;
#endif
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }

            return jieguo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="varList"></param>
        /// <param name="dbTypeList"></param>
        /// <param name="objectLengthList"></param>
        /// <param name="objectList"></param>
        /// <param name="objectValueList"></param>
        /// <returns></returns>
        public sealed override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {
            bool jieguo = false;
            if (invalidator.IsInvalidArguments(procedureName))
            {
                return false;
            }
            if (!IsOpened) { throw new ConnectErrorException(); }
            try
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    myCmd.Connection = (SqlConnection)baseConn;
                    myCmd.CommandType = CommandType.StoredProcedure;
                    myCmd.CommandText = procedureName;
                    for (int i = 0; i < varList.Length; i++)
                    {
                        myCmd.Parameters.Add("@" + varList[i], (SqlDbType)(dbTypeList[i]), objectLengthList[i], objectList[i].ToString());
                        myCmd.Parameters["@" + varList[i]].Value = objectValueList[i];
                        Debug.WriteLine(i + "  " + varList[i] + "    " + dbTypeList[i] + "   " + objectValueList[i]);
                    }
                    myCmd.ExecuteNonQuery();
                }
                jieguo = true;
            }
            catch (Exception ee)
            {
                GlobalDefine.SP.LastErrorMsg = ee.Message;
#if DEBUG
                throw ee;
#else

                return false;
#endif
            }
            return jieguo;
        }

        /// <summary>
        ///		Creates a new parameter and sets the name of the parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="size">The size of this parameter.</param>
        /// <param name="value">
        ///		The value you want assigned to this parameter. A null value will be converted to
        ///		a <see cref="DBNull"/> value in the parameter.
        /// </param>
        /// <returns>
        ///		A new <see cref="DbParameter"/> instance of the correct type for this database.</returns>
        /// <remarks>
        ///		The database will automatically add the correct prefix, like "@" for SQL Server, to the
        ///		parameter name. In other words, you can just supply the name without a prefix.
        /// </remarks>
        public sealed override DbParameter CreateParameter(string name, DbType type, int size, object value)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            DbParameter param = factory.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Size = size;
            param.Value = (value == null) ? DBNull.Value : value;
            return param;
        }

        public sealed override DataTable GetSchemaTable()
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            DataTable ds = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM INFORMATION_SCHEMA.TABLES", (SqlConnection)baseConn))
            {
                da.Fill(ds);
            }
            return ds;
        }

    }
}
