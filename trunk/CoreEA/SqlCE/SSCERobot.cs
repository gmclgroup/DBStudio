using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using System.Data.Common;
using System.Data;
using CoreEA.ConnSTR;
using CoreEA.Invalidation;
using System.Diagnostics;
using CoreEA.Args;
using CoreEA.Exceptions;
using CoreEA.LoginInfo;
using CoreEA.SchemaInfo;
using CoreEA.GlobalDefine;
using System.Text.RegularExpressions;
using System.Linq;
using CoreEA.SqlCE;

namespace CoreEA
{
    internal class SqlCERobot : BaseRobot
    {

        private readonly string SeperateLine = Environment.NewLine;


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
        ///<param name="indexSchemas">The schema of the index objects</param>
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
                        sb.AppendFormat("[{0}] {1},", col.ColumnName, col.IsAscending?"Asc":"Desc");
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
                    currentCommandTextHandler = new SSCECommandText();
                }
                return currentCommandTextHandler;
            }
        }
        /// <summary>
        /// This db type don't contain such type
        /// </summary>
        /// <returns></returns>
        public override List<BaseStoredProcedureInfo> GetStoredProceduresList()
        {
            return new List<BaseStoredProcedureInfo>();
        }


        /// <summary>
        /// SSCE does not contain triggers 
        /// so just return a null object
        /// </summary>
        /// <returns></returns>
        public sealed override List<BaseTriggerInfo> GetTriggersInfo()
        {
            return new List<BaseTriggerInfo>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [Obsolete("This method has obsoleted , please use CreateTable instead", true)]
        public sealed override bool CreateTableWithSchemaInfo(List<CreateTableArgs> args, string tableName)
        {
            bool result = false;
            if (args == null)
            {
                return false;
            }
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            tableName = GetMaskedTableName(tableName);
            string sqlCmd = string.Empty;
            try
            {

                //Create table at least need one column 
                //so here add a temp column
                sqlCmd = String.Format("CREATE TABLE {0} (", tableName);
                string t1 = string.Empty;
                string t2 = string.Empty;
                string t3 = string.Empty;
                string t4 = string.Empty;
                string t7 = string.Empty;

                string stepCmd = string.Empty;

                foreach (CreateTableArgs item in args)
                {
                    if (item.dataLength > 0)
                    {
                        t4 = string.Format("({0})", item.dataLength);
                    }

                    t1 = item.allowNulls == true ? "NULL" : "NOT NULL";
                    t2 = item.isUnique == true ? "UNIQUE" : "";
                    t3 = item.isPrimaryKey == true ? "PRIMARY KEY" : "";
                    if (item.isPrimaryKey)
                    {
                        t7 = "CONSTRAINT";
                    }

                    stepCmd = String.Format("{0} {1} {2}{3} {7} {4} {5} {6},", stepCmd, item.fieldName, item.dataType, t4,
                       t7, t1, t2, t3);

                }
                stepCmd = stepCmd.Substring(0, stepCmd.Length - 1);
                stepCmd += ")";

                sqlCmd += stepCmd;

                SqlCeCommand dbCmd = (SqlCeCommand)this.GetNewStringCommand(sqlCmd);

                this.DoExecuteNonQuery(dbCmd);

                result = true;
            }
            catch (Exception ee)
            {
                throw ee;
            }

            return result;
        }

        public sealed override bool CreateTable(BaseTableSchema ts)
        {
            bool result = false;
            //The first sql command is to create table 
            bool isCreateTableOk = false;
            int i = 0;
            try
            {
                string[] createTableSchameCommands = GetCreateTableString(ts).Split(';');
       
                foreach (var item in createTableSchameCommands)
                {
                    //Here the last item will be a '\n' 
                    //so add judegement of length 
                    //it not a good idea ,but workaround
                    if (item.Length > 5)
                    {
                        Debug.WriteLine("Execute create table sql ------------>" + item);

                        DoExecuteNonQuery(item);

                        if (i == 0)
                        {
                            isCreateTableOk = true;
                        }
                    }
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
        /// This method will get primarykey , column ,indexes, 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public sealed override string GetCreateTableString(BaseTableSchema ts)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("CREATE TABLE " +GetMaskedTableName( ts.TableName) + " (\n");

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

            //Leon Marked 2009-10-27
            //Old code to proecess index one by one 
            //but some scenario need create index with two or up columns 
            //so replace these code 

            //// Create any relevant indexes
            //if (ts.Indexes != null)
            //{
            //    for (int i = 0; i < ts.Indexes.Count; i++)
            //    {
            //        string stmt = BuildCreateIndex(ts.TableName, ts.Indexes[i]);
            //        sb.Append(stmt + ";\n");
            //    } // for
            //} // if

             string stmt=BuildCreateIndex(ts.TableName,ts.Indexes);
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

        /// <summary>
        /// This method will get primarykey , column ,indexes, 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override BaseTableSchema GetTableSchemaInfoObject(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            BaseTableSchema tableSchema = new BaseTableSchema();

            tableSchema.PrimaryKey = GetPrimaryKeysFromTable(tableName);

            tableSchema.TableName = tableName;

            DataTable dt = GetColumnInfoFromTable(tableName);

            #region Column Info
            foreach (DataRow item in dt.Rows)
            {
                BaseColumnSchema schmeaInfo = new BaseColumnSchema();

                schmeaInfo.ColumnName = item["COLUMN_NAME"].ToString();
                schmeaInfo.ColumnType = item["DATA_TYPE"].ToString();
                schmeaInfo.AutoIncrementSeed = (item["AUTOINC_SEED"].IsDBNull() ? 0 : 1);

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

                schmeaInfo.IsIdentity = item["AUTOINC_SEED"].IsDBNull() ? false : true;

                tableSchema.Columns.Add(schmeaInfo);
            }

            #endregion Index Info

            #region Index Info
            DataTable dtForIndex = GetIndexInfoFromTable(tableName);
            foreach (DataRow row in dtForIndex.Rows)
            {
                BaseIndexSchema indexInfo = new BaseIndexSchema();
                indexInfo.ColumnName = row["COLUMN_NAME"].ToString();
                indexInfo.IndexName = row["INDEX_NAME"].ToString();
                //indexInfo.IsAscending
                indexInfo.IsClustered = bool.Parse(row["CLUSTERED"].ToString());
                indexInfo.IsPrimaryKey = bool.Parse(row["PRIMARY_KEY"].ToString());
                indexInfo.IsUnique = bool.Parse(row["UNIQUE"].ToString());
                indexInfo.TableName = row["TABLE_NAME"].ToString();

                indexInfo.OrdinalPosition = row["ORDINAL_POSITION"].IsDBNull() ?
    0 : int.Parse(row["ORDINAL_POSITION"].ToString());


                tableSchema.Indexes.Add(indexInfo);
            }
            #endregion Endof IndexInfo

            return tableSchema;
        }


        public sealed override bool HasIdentityColumnInTable(string tableName)
        {
            tableName = GetMaskedTableName(tableName);
            return (ExecuteDataList("SELECT COLUMN_NAME FROM information_schema.columns WHERE TABLE_NAME = N'"
                + tableName +
                "' AND AUTOINC_SEED IS NOT NULL") != null);
        }


        public sealed override CoreE.UsedDatabaseType HostedType
        {
            get
            {
                return CoreE.UsedDatabaseType.SqlCE35;
            }
        }


        public sealed override int MaxTableNameLength
        {
            get
            {
                return 128;
            }
        }

        private readonly string _getTableList = "SELECT table_name FROM information_schema.tables WHERE TABLE_TYPE = N'TABLE'";


        /// <summary>
        /// Get all infos from schemalinfo
        /// </summary>
        private readonly string SystemView_ForIndex = "INFORMATION_SCHEMA.INDEXES";


        /// <summary>
        ///         //+		["DATE_MODIFIED"]	{DATE_MODIFIED}	
        ///  //+		["TABLE_TYPE"]	{TABLE_TYPE}	
        ///  //+		["DESCRIPTION"]	{DESCRIPTION}	
        ///  //+		["DATE_CREATED"]	{DATE_CREATED}	
        ///  //+		["TABLE_PROPID"]	{TABLE_PROPID}	
        ///  //+		["TABLE_SCHEMA"]	{TABLE_SCHEMA}	
        ///   //+		["TABLE_GUID"]	{TABLE_GUID}	
        ///      //+		["TABLE_CATALOG"]	{TABLE_CATALOG}	
        ///      //+		["TABLE_NAME"]	{TABLE_NAME}	
        /// </summary>
        private readonly string SystemView_ForTable = "INFORMATION_SCHEMA.TABLES";
        private readonly string SystemView_ForColumns = "INFORMATION_SCHEMA.COLUMNS";
        private readonly string SystemView_ForIndex_ForKeyColumnUsage = "INFORMATION_SCHEMA.KEY_COLUMN_USAGE";
        private readonly string SystemView_ForProviderType = "INFORMATION_SCHEMA.PROVIDER_TYPES";
        private readonly string SystemView_ForTableConstraint = "INFORMATION_SCHEMA.TABLE_CONSTRAINTS";
        private readonly string SystemView_ForRefConstraint = "INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS";
        private readonly string SystemView_ForSysLockInfo = "sys.lock_information";


        private readonly string ColumnNameMask = "COLUMN_NAME";
        private readonly string IndexNameMask = "INDEX_NAME";
        private readonly string keyColumnUsageMask = "INFORMATION_SCHEMA.KEY_COLUMN_USAGE";
        private readonly string providerTypeMask = "INFORMATION_SCHEMA.PROVIDER_TYPES";

        /// <summary>
        /// Get Readonly Value For SSCE System Views Schema Name
        /// </summary>
        /// <returns></returns>
        public override List<string> GetSystemViewList()
        {
            List<string> rt = new List<string>();
            rt.Add(SystemView_ForColumns);
            rt.Add(SystemView_ForTable);
            rt.Add(SystemView_ForIndex);
            rt.Add(SystemView_ForTableConstraint);
            rt.Add(SystemView_ForIndex_ForKeyColumnUsage);
            rt.Add(SystemView_ForProviderType);
            rt.Add(SystemView_ForRefConstraint);
            rt.Add(SystemView_ForSysLockInfo);

            return rt;
        }

        /// <summary>
        /// Get the SystemViews Info
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public override List<string> GetSystemViewColumnsNameByViewName(string viewName)
        {
            List<string> rt = new List<string>();

            DbCommand cmd = GetNewStringCommand("select * from " + viewName);

            DataTable ds = ExecuteDataList(cmd).Tables[0];

            foreach (DataColumn item in ds.Columns)
            {
                rt.Add(item.ColumnName);
            }

            return rt;
        }

        /// <summary>
        /// This method is most common ,so no need to check the connection 
        /// </summary>
        /// <param name="dbCmd"></param>
        /// <returns></returns>
        public sealed override DbDataAdapter GetDataAdapter(DbCommand dbCmd)
        {
            SqlCeCommand myCmd = dbCmd as SqlCeCommand;
            if (myCmd == null) throw new ArgumentException();
            SqlCeDataAdapter sa = new SqlCeDataAdapter(myCmd);
            return sa;
        }

        /// <summary>
        /// This method is most common ,so no need to check the connection 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public sealed override DbCommand GetNewStringCommand(string sql)
        {

            DbCommand cmd = new SqlCeCommand();
            if (!string.IsNullOrEmpty(sql))
            {
                cmd.CommandText = sql;
            }

            cmd.Connection = baseConn;
            return cmd;
        }

        public sealed override List<string> GetDatabaseList()
        {
            throw new NotImplementedException();
        }

        public sealed override decimal GetColumnLength(string tableName, string columnName)
        {
            return 65535M;
        }


        /// <summary>
        /// ''
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 从Type name 获得具体DbType
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public sealed override DbType GetDbTypeFromTypeName(string typeName)
        {
            DbType curType;

            switch (typeName)
            {
                case "tinyint":
                    curType = DbType.UInt32;
                    break;
                case "bigint":
                    curType = DbType.UInt32;
                    break;
                case "integer":
                    curType = DbType.Int32;
                    break;
                case "int":
                    curType = DbType.Int32;
                    break;
                case "smallint":
                    curType = DbType.UInt32;
                    break;
                case "bit":
                    curType = DbType.Boolean;
                    break;
                case "decimal":
                    curType = DbType.Decimal;
                    break;
                case "money":
                    curType = DbType.Currency;
                    break;
                case "float":
                    curType = DbType.Single;
                    break;
                case "real":
                    curType = DbType.Boolean;
                    break;
                case "datetime":
                    curType = DbType.DateTime;
                    break;
                case "nchar":
                    curType = DbType.String;
                    break;
                case "nvarchar":
                    curType = DbType.String;
                    break;
                case "binary":
                    curType = DbType.Binary;
                    break;
                case "varbinary":
                    curType = DbType.Binary;
                    break;
                case "image":
                    curType = DbType.Binary;
                    break;

                case "ntext":
                    curType = DbType.String;
                    break;
                case "uniqueidentifier":
                    curType = DbType.Guid;
                    break;
                default:
                    curType = DbType.String;
                    break;
            }

            return curType;
        }

        //public sealed override List<string> GetPrimaryKeysFromTable(string tableName)
        //{

        /*My Old Method ,but still can work*/
        //List<string> primaryKeys = new List<string>();
        //table = GetMaskedTableName(table);
        //try
        //{
        //    if (!IsOpened)
        //    {
        //        throw new ConnectErrorException();
        //    }

        //    string cmdText =
        //    "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.INDEXES where Table_Name='" + table + "' AND PRIMARY_KEY=1";

        //    SqlCeCommand cmd = GetNewStringCommand(cmdText) as SqlCeCommand;

        //    DataTable dt = new DataTable();
        //    SqlCeDataAdapter adapter = new SqlCeDataAdapter();
        //    adapter.SelectCommand = cmd;
        //    adapter.Fill(dt);

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        primaryKeys.Add((string)row["COLUMN_NAME"]);

        //    }
        //}
        //catch { }

        //return primaryKeys;
        //}

        /// <summary>
        /// Get all Key collumn info 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetKeyColumnInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            tableName = GetMaskedTableName(tableName);

            DataTable tempDs = null;
            try
            {
                DbCommand cmd = this.GetNewStringCommand(
                    string.Format("SELECT * FROM {0} where TABLE_NAME=?", keyColumnUsageMask));
                this.AddParameters(cmd, this.CreateParameter("TABLE_NAME", DbType.String, 65535, tableName));
                tempDs = this.ExecuteDataList(cmd).Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tempDs;
        }


        /// <summary>
        /// Get all  info 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetProviderInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            tableName = GetMaskedTableName(tableName);
            //where TABLE_NAME='{1}'
            DataTable tempDs = null;
            try
            {
                DbCommand cmd = this.GetNewStringCommand(
                    string.Format("SELECT * FROM {0} ", providerTypeMask));
                tempDs = this.ExecuteDataList(cmd).Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tempDs;
        }

        /// <summary>
        /// Get all data type supported by sqlce 
        /// </summary>
        /// <returns></returns>
        public sealed override DataTable GetSupportedDbType()
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            DataTable tempDs = null;
            try
            {
                //where TABLE_NAME='{1}'
                DbCommand cmd = this.GetNewStringCommand(
                string.Format("SELECT TYPE_NAME FROM {0} ", providerTypeMask));
                tempDs = this.ExecuteDataList(cmd).Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tempDs;
        }


        /// <summary>
        /// Get all indexes info 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetIndexInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            //tableName = GetMaskedTableName(tableName);
            DataTable tempDs = null;
            try
            {
                DbCommand cmd = this.GetNewStringCommand(
                    string.Format("SELECT * FROM {0} where TABLE_NAME=?", SystemView_ForIndex));

                this.AddParameters(cmd, this.CreateParameter("TABLE_NAME", DbType.String, 65535, tableName));

                tempDs = this.ExecuteDataList(cmd).Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tempDs;
        }

        public sealed override List<string> GetIndexNameListFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            tableName = GetMaskedTableName(tableName);
            DataTable tempDs = GetColumnInfoFromTable(tableName);
            List<string> columnList = new List<string>();
            if (tempDs != null)
            {
                foreach (DataRow item in tempDs.Rows)
                {
                    columnList.Add(item[IndexNameMask].ToString());
                }
            }

            return columnList;
        }



        public sealed override bool InsertData(string tableName, List<string> columnName, List<object> columnValue, List<DbType> types)
        {
            bool result = false;
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            tableName = GetMaskedTableName(tableName);

            try
            {
                StringBuilder st = new StringBuilder();
                StringBuilder st2 = new StringBuilder();

                int count = columnName.Count;
                int j = 0;
                foreach (string item in columnName)
                {
                    st.Append(item);

                    st2.Append("?");

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
                        this.AddParameters(cmd, this.CreateParameter(columnName[i], types[i], 65535, columnValue[i]));
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
        /// Get all columns info
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
                DbCommand cmd = this.GetNewStringCommand(
                string.Format("SELECT * FROM {0} where TABLE_NAME=?", SystemView_ForColumns));

                this.AddParameters(cmd, this.CreateParameter("TABLE_NAME", DbType.String, 65535, tableName));

                tempDs = this.ExecuteDataList(cmd).Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return tempDs;
        }


        public sealed override List<string> GetColumnNameListFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            //tableName = GetMaskedTableName(tableName);
            DataTable tempDs = GetColumnInfoFromTable(tableName);
            List<string> columnList = new List<string>();
            if (tempDs != null)
            {
                foreach (DataRow item in tempDs.Rows)
                {
                    columnList.Add(item[ColumnNameMask].ToString());
                }
            }

            return columnList;
        }


        /// <summary>
        ///		Returns the ID of the most recently added row.
        /// </summary>
        /// <returns>
        ///		The ID of the row added, or -1 if no row added, or the table doesn't have an identity column.
        ///	</returns>
        public override int GetLastId(DbConnection connection)
        {
            using (DbCommand command = new SqlCeCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText = "SELECT @@IDENTITY";

                command.Connection = connection;

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return -1;
                    else if (reader[0] is DBNull)
                        return -1;
                    else return Convert.ToInt32(reader[0]);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public sealed override DbParameter CreateParameter(string name, DbType type, int size, object value)
        {

            SqlCeParameter param = new SqlCeParameter();
            param.ParameterName = name;
            param.DbType = type;
            param.Size = size;
            param.Value = (value == null) ? DBNull.Value : value;
            return param;
        }


        public sealed override bool CreateDatabase(BaseLoginInfo loginInfo)
        {
            LoginInfo_SSCE myInfo = loginInfo as LoginInfo_SSCE;
            bool result = false;

            string createTabelCommand = string.Empty;
            //Use "" for filter Special Chars
            createTabelCommand = string.Format("DataSource=\"{0}\"", myInfo.DbName);

            if (!string.IsNullOrEmpty(myInfo.Pwd))
            {
                createTabelCommand = string.Format("{0} ;Password={1}", createTabelCommand, myInfo.Pwd);
            }
            else
            {
                createTabelCommand = string.Format("{0} ;Password=''", createTabelCommand, myInfo.Pwd);
            }

            createTabelCommand = string.Format("{0} ;Encrypt={1}", createTabelCommand, myInfo.IsEncrypted);

            //Add Case Sensitive Support
            //This feautre only support after SqlCE3.5 Sp1
            if (myInfo.IsCaseSensitive)
            {
                createTabelCommand = createTabelCommand + ";Case Sensitive=true";
            }
            try
            {
                SqlCeEngine eg = new SqlCeEngine(createTabelCommand);
                eg.CreateDatabase();

                result = true;
            }
            catch (Exception ce)
            {
                throw ce;
            }

            return result;
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

                baseConn = new SqlCeConnection(connectionString);
                baseConn.Open();


                invalidator = new InvalidatorForSqlCe();
            }
            catch (SqlCeException ee)
            {
                throw ee;
            }
        }

        public sealed override void Open(BaseLoginInfo pInfo)
        {

            //Record to base class (Vital)
            baseLoginInfo = pInfo;


            LoginInfo_SSCE myInfo = pInfo as LoginInfo_SSCE;
            LoginInfo_ForAllDbTypes allInfo = pInfo as LoginInfo_ForAllDbTypes;

            if ((myInfo == null) && (allInfo == null))
            {
                throw new ArgumentException("Only Support SSCE login info and AllDBTypes Info");
            }

            if (IsOpened)
            {
                return;
            }

            string myConnString = string.Empty;
            try
            {
                if (allInfo != null)
                {
                    myInfo = new LoginInfo_SSCE();
                    myInfo.DbName = allInfo.Database;
                    myInfo.Pwd = allInfo.Pwd;
                    myInfo.IsEncrypted = allInfo.IsEncrypt;

                }

                OpenModeClass op = new OpenModeClass();
                op.mode = myInfo.CurOpenMode;
                switch (myInfo.CurOpenMode)
                {
                    case OpenMode.ReadWrite:
                        op.modeDisplayName = "Read Write";
                        break;
                    case OpenMode.ReadOnly:
                        op.modeDisplayName = "Read Only";
                        break;
                    case OpenMode.Exclusive:
                        op.modeDisplayName = "Exclusive";
                        break;
                    case OpenMode.SharedRead:
                        op.modeDisplayName = "Shared Read";
                        break;
                    default:
                        op.modeDisplayName = "Read Write";
                        break;

                }

                if (myInfo.MaxBufferSize == 0)
                {
                    myInfo.MaxBufferSize = 1024;
                }
                if (myInfo.MaxDbSize == 0)
                {
                    myInfo.MaxDbSize = 4000;
                }

                myConnString = DbConnectionString.SSCE.GetSSCEConnectionString(
                    myInfo.DbName, myInfo.Pwd, myInfo.IsEncrypted, op,myInfo.MaxDbSize,myInfo.MaxBufferSize);


                baseConn = new SqlCeConnection(myConnString);
                baseConn.Open();

                CurDatabase = myInfo.DbName;
                CurPwd = myInfo.Pwd;

                invalidator = new InvalidatorForSqlCe();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName">
        /// This parameter is not used in sqlce 
        /// Recommend use this base method (with no parameter)</param>
        /// <returns></returns>
        public sealed override List<string> GetTableListInDatabase(string databaseName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            List<string> result = new List<string>();

            SqlCeCommand cmd = (SqlCeCommand)GetNewStringCommand(_getTableList);

            DataTable tempDs = ExecuteDataList(cmd).Tables[0];

            if (tempDs != null)
            {
                result = new List<string>();
                foreach (DataRow dr in tempDs.Rows)
                {
                    result.Add(dr[0].ToString());
                }
            }

            return result;
        }


        /// <summary>
        /// I am curious about the SSCE support the stored procedure ?
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="varList"></param>
        /// <param name="dbTypeList"></param>
        /// <param name="objectLengthList"></param>
        /// <param name="objectList"></param>
        /// <param name="objectValueList"></param>
        /// <returns></returns>
        public sealed override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, System.Data.OleDb.OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {
            bool jieguo = false;
            if (invalidator.IsInvalidArguments(procedureName))
            {
                return false;
            }
            if (!IsOpened) { throw new ConnectErrorException(); }
            try
            {
                using (SqlCeCommand myCmd = new SqlCeCommand())
                {
                    myCmd.Connection = (SqlCeConnection)baseConn;
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

        public sealed override DataTable GetSchemaTable()
        {
            return baseConn.GetSchema();
        }

    }
}
