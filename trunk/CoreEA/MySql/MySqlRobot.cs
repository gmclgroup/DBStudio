using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Data.OleDb;
using CoreEA.Invalidation;
using CoreEA.ConnSTR;
using System.Data.Common;
using CoreEA.Exceptions;
using CoreEA.LoginInfo;
using CoreEA.SchemaInfo;
using CoreEA.GlobalDefine;
using CoreEA.MySql;
using MySql.Data.MySqlClient;

namespace CoreEA
{
    internal partial class MySqlRobot : BaseRobot
    {
        
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
                    currentCommandTextHandler = new MySqlCommandText();
                }
                return currentCommandTextHandler;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override List<int> GetDbType()
        {
            throw new NotImplement();
           
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
                    {
                        sb.Append(", ");
                    }
                } // for
                sb.Append(")\n");
            }
            else
                sb.Append("\n");

            sb.Append(");\n");
            string end= sb.ToString();

            return end;
        }


        /// <summary>
        /// Used when creating the CREATE TABLE DDL. Creates a single row
        /// for the specified column.
        /// </summary>
        /// <param name="eachColumn">The column schema</param>
        /// <returns>A single column line to be inserted into the general CREATE TABLE DDL statement</returns>
        private string BuildColumnStatement(BaseColumnSchema eachColumn)
        {
            StringBuilder createSchemaScript = new StringBuilder();

            switch (eachColumn.ColumnType.ToLower())
            {
                case "varchar":
                    createSchemaScript.AppendFormat("{0} {1}({2}) {3} {4} {5} ",
                         GetMaskedColumnName(eachColumn.ColumnName),
                         eachColumn.ColumnType,
                         eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "char":
                    createSchemaScript.AppendFormat("{0} {1}({2}) {3} {4} {5} ",
                         GetMaskedColumnName(eachColumn.ColumnName),
                         eachColumn.ColumnType,
                         eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "nchar":
                    createSchemaScript.AppendFormat("{0} {1}({2}) {3} {4} {5} ",
                         GetMaskedColumnName(eachColumn.ColumnName),
                         eachColumn.ColumnType,
                         eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "nvarchar":
                    createSchemaScript.AppendFormat("{0} {1}({2}) {3} {4} {5}",
                         GetMaskedColumnName(eachColumn.ColumnName),
                        eachColumn.ColumnType,
                        eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "numeric":
                    createSchemaScript.AppendFormat("{0} {1}({2},{3}) {4} {5} {6} ",
                          GetMaskedColumnName(eachColumn.ColumnName),
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
                case "image":
                    createSchemaScript.AppendFormat("{0} {1} {2} {3} {4}{5} {6} ",
      GetMaskedColumnName(eachColumn.ColumnName),
     "BLOB",
     eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
     eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
     eachColumn.RowGuidCol ? "ROWGUIDeachColumn" : string.Empty,
     (eachColumn.AutoIncrementBy > 0 ? string.Format("IDENTITY ({0},{1})", eachColumn.AutoIncrementSeed, eachColumn.AutoIncrementBy) : string.Empty),
     System.Environment.NewLine);
                    break;
                case "ntext":
                    createSchemaScript.AppendFormat("{0} {1} {2} {3} {4}{5} {6} ",
      GetMaskedColumnName(eachColumn.ColumnName),
     "text",
     eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
     eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
     eachColumn.RowGuidCol ? "ROWGUIDeachColumn" : string.Empty,
     (eachColumn.AutoIncrementBy > 0 ? string.Format("IDENTITY ({0},{1})", eachColumn.AutoIncrementSeed, eachColumn.AutoIncrementBy) : string.Empty),
     System.Environment.NewLine);
                    break;
                default:
                    createSchemaScript.AppendFormat("{0} {1} {2} {3} {4}{5} {6} ",
                          GetMaskedColumnName(eachColumn.ColumnName),
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


        public sealed override string GetMaskedColumnName(string columnName)
        {
            if (columnName.StartsWith("`"))
            {
                if (columnName.EndsWith("`"))
                {
                    return columnName;
                }
                else
                {
                    return string.Format("{0}`", columnName);
                }
            }
            else
            {
                if (columnName.EndsWith("`"))
                {
                    return string.Format("`{0}", columnName);
                }
                else
                {
                    return string.Format("`{0}`", columnName);
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

                Debug.WriteLine(schmeaInfo.ColumnType);
                //Different
                //schmeaInfo.AutoIncrementSeed = bool.Parse(item["AUTOINCREMENT"].ToString()) == true ? 1 : 0;

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

            tableSchema.Indexes = GetIndexSchemaInfoFromTable(tableName);

            return tableSchema;
        }
        public sealed override bool HasIdentityColumnInTable(string tableName)
        {
            return false;
        }

        /// <summary>
        /// This db type don't has such view
        /// </summary>
        /// <returns></returns>
        public sealed override List<string> GetSystemViewList()
        {
            return new List<string>();
        }
        /// <summary>
        /// Not Implement
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetProviderInfoFromTable(string tableName)
        {
            return null;
        }


        public sealed override CoreE.UsedDatabaseType HostedType
        {
            get
            {
                return CoreE.UsedDatabaseType.MySql;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed override int MaxTableNameLength
        {
            get
            {
                //I am doubt this value ,
                //I think the 64 is the right value
                //return 10;
                return 64;
            }
        }   
        /// <summary>
        /// 
        /// </summary>
        public sealed override int MaxColumnNameLength
        {
            get
            {
                
                return 64;
            }
        }



        public override bool CreateDatabase(BaseLoginInfo loginInfo)
        {
            throw new NotImplementedException();
        }

        public sealed override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {
            throw new NotImplementedException();
        }

        public sealed override DbDataAdapter GetDataAdapter(DbCommand dbCmd)
        {
            MySqlCommand myCmd = dbCmd as MySqlCommand;
            if (myCmd == null) throw new ArgumentException();
            MySqlDataAdapter sa = new MySqlDataAdapter(myCmd);
            return sa;
        }

        public sealed override DbCommand GetNewStringCommand(string sql)
        {
            DbCommand cmd = new MySqlCommand();
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

            if (tableName.StartsWith("`"))
            {
                if (tableName.EndsWith("`"))
                {
                    return tableName;
                }
                else
                {
                    return string.Format("{0}`", tableName);
                }
            }
            else
            {
                if (tableName.EndsWith("`"))
                {
                    return string.Format("`{0}", tableName);
                }
                else
                {
                    return string.Format("`{0}`", tableName);
                }

            }
        }


        public sealed override int GetLastId(DbConnection connection)
        {
            using (DbCommand command = new MySqlCommand())
            {
                command.CommandType = CommandType.Text;

                command.CommandText="SELECT @@IDENTITY";

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
        /// Get Database List 
        /// 
        /// Another method is retrieve from baseConn.GetSchema("Databases");
        /// </summary>
        /// <returns></returns>
        public sealed override List<string> GetDatabaseList()
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            List<string> jieguo = new List<string>();
            MySqlDataReader reader = null;
            MySqlCommand cmd = new MySqlCommand("SHOW DATABASES",(MySqlConnection)baseConn);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jieguo.Add(reader.GetString(0));
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to populate database list: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            return jieguo;
        }

        /// <summary>
        /// Please notice : the second para of GetSchema Method
        /// null,null,tableName,null
        /// Each value is mapping to : Database , UID ,TableName , ColumnName
        /// 
        ///   <Columns>
        ///  <TABLE_SCHEMA>mysql</TABLE_SCHEMA>
        ///  <TABLE_NAME>user</TABLE_NAME>
        ///  <COLUMN_NAME>Host</COLUMN_NAME>
        ///  <ORDINAL_POSITION>1</ORDINAL_POSITION>
        /// <COLUMN_DEFAULT />
        ///  <IS_NULLABLE>NO</IS_NULLABLE>
        ///  <DATA_TYPE>char</DATA_TYPE>
        ///  <CHARACTER_MAXIMUM_LENGTH>60</CHARACTER_MAXIMUM_LENGTH>
        ///  <CHARACTER_SET_NAME>utf8</CHARACTER_SET_NAME>
        ///  <COLLATION_NAME>utf8_bin</COLLATION_NAME>
        ///  <COLUMN_TYPE>char(60)</COLUMN_TYPE>
        ///  <COLUMN_KEY>PRI</COLUMN_KEY>
        ///  <EXTRA />
        ///  <PRIVILEGES>select,insert,update,references</PRIVILEGES>
        ///  <COLUMN_COMMENT />
        ///  </Columns>
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetColumnInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            tableName = GetMaskedTableName(tableName);

            DataTable dt = baseConn.GetSchema("Columns", new string[] { null, null, tableName, null });

            return dt;
        }

        /// <summary>
        /// Please notice : the second para of GetSchema Method
        /// null,null,tableName,null
        /// Each value is mapping to : Database , UID ,TableName , ColumnName
        ///   <Indexes>
        ///<INDEX_SCHEMA>mysql</INDEX_SCHEMA>
        ///<INDEX_NAME>PRIMARY</INDEX_NAME>
        ///<TABLE_NAME>user</TABLE_NAME>
        ///<UNIQUE>true</UNIQUE>
        ///<PRIMARY>true</PRIMARY>
        ///</Indexes>
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override DataTable GetIndexInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            //Due to this way to get index schema info is unuseful,so we need create a 
            //temp datatable to map the schema

            //tableName = GetMaskedTableName(tableName);
            //DataTable dt = baseConn.GetSchema("Indexes", new string[] { null, null, tableName, null });
            DataTable dt = new DataTable("IndexSchemaTempTable");
            dt.Columns.Add("INDEX_NAME", typeof(string));

            List<BaseIndexSchema> result = GetIndexSchemaInfoFromTable(tableName);
            foreach (var item in result)
            {
                dt.Rows.Add(new object[] { item.IndexName });
            }

            return dt;
        }

        //Description of fields (help taken from Mysql website):
        //Table
        //The name of the table.
        //Non_unique
        //0 if the index cannot contain duplicates, 1 if it can.
        //Key_name
        //The name of the index.
        //Seq_in_index
        //The column sequence number in the index, starting with 1.
        //Column_name
        //The column name.
        //Collation
        //How the column is sorted in the index. In MySQL, this can have values “A” (Ascending) or NULL (Not sorted).
        //Cardinality
        //An estimate of the number of unique values in the index. This is updated by running ANALYZE TABLE or myisamchk -a. Cardinality is counted based on statistics stored as integers, so the value is not necessarily exact even for small tables. The higher the cardinality, the greater the chance that MySQL uses the index when doing joins.
        //Sub_part
        //The number of indexed characters if the column is only partly indexed, NULL if the entire column is indexed.
        //Packed
        //Indicates how the key is packed. NULL if it is not.
        //Null
        //Contains YES if the column may contain NULL values and ” if not.
        //Index_type
        //The index method used (BTREE, FULLTEXT, HASH, RTREE).
        //Comment
        //Various remarks.
        private List<BaseIndexSchema> GetIndexSchemaInfoFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            tableName = GetMaskedTableName(tableName);

            List<BaseIndexSchema> jieguo = new List<BaseIndexSchema>();

            MySqlDataReader reader = null;

            MySqlCommand cmd = new MySqlCommand("SHOW INDEX FROM " + tableName, (MySqlConnection)baseConn);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jieguo.Add(new BaseIndexSchema()
                    {
                        TableName=reader.GetString(0),
                        IsUnique=reader.GetBoolean(1),
                        IndexName=reader.GetString(2),
                        OrdinalPosition=reader.GetInt32(3),
                        ColumnName=reader.GetString(4),
                        IsAscending=reader.IsDBNull(5)?false:true,
                        
                    });
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to populate index list: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return jieguo;
        }

        public sealed override List<string> GetIndexNameListFromTable(string tableName)
        {
            return base.GetIndexNameListFromTable(tableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override List<BaseStoredProcedureInfo> GetStoredProceduresList()
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            List<BaseStoredProcedureInfo> info = new List<BaseStoredProcedureInfo>();
            DataTable dt = this.GetConnection().GetSchema("Procedures");
            foreach (DataRow item in dt.Rows)
            {
                info.Add(new BaseStoredProcedureInfo()
                {
                    ProcedureName = item["name"].ToString()
                });
            }

            return info;
        }

        /// <summary>
        /// Get Table list in which database
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>

        public sealed override List<string> GetTableListInDatabase(string dbName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            List<string> jieguo = new List<string>();

            MySqlDataReader reader = null;
            if (!string.IsNullOrEmpty(dbName))
            {
                baseConn.ChangeDatabase(dbName);
            }
            MySqlCommand cmd = new MySqlCommand("SHOW TABLES", (MySqlConnection)baseConn);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    jieguo.Add(reader.GetString(0));
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Failed to populate table list: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return jieguo;

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

                baseConn = new MySqlConnection(connectionString);
                baseConn.Open();
                invalidator = new InvalidatorForMySql();
            }
            catch (MySqlException ee)
            {
                throw ee;
            }
        }



        public sealed override void Open(BaseLoginInfo pInfo)
        {
            //Record to base class (Vital)
            baseLoginInfo = pInfo;

            LoginInfo_MySql myInfo = pInfo as LoginInfo_MySql;
            LoginInfo_ForAllDbTypes allInfo = pInfo as LoginInfo_ForAllDbTypes;

            if ((myInfo == null) && (allInfo == null))
            {
                throw new ArgumentException("Only Support MySql login info and AllDBTypes Info");
            }

            if (IsOpened)
            {
                return;
            }

            try
            {
                string myConnString = string.Empty;
                if (allInfo != null)
                {
                    myInfo = new LoginInfo_MySql();
                    myInfo.Database = allInfo.Database;
                    myInfo.Pwd = allInfo.Pwd;
                    myInfo.Server = allInfo.Server;
                    myInfo.Username = allInfo.Username;
                    myInfo.Port = allInfo.Port;
                    
                }

                myConnString = DbConnectionString.MySql.GetMySqlConnectionString(
                    myInfo.Server,
                    myInfo.Username,
                    myInfo.Pwd,
                    myInfo.Port,
                    myInfo.Database,
                    myInfo.ConnectionTimeOut,
                    50,
                    myInfo.IsPolling
                    );
                
                baseConn = new MySqlConnection(myConnString);
                baseConn.Open();

                Debug.WriteLine("Connection Timeout is " + baseConn.ConnectionTimeout);

                invalidator = new InvalidatorForMySql();
                //Set Current Opened Database
                base.CurDatabase = myInfo.Database;
                base.CurPwd = myInfo.Pwd;

                if (string.IsNullOrEmpty(myInfo.Database))
                {
                    DoExecuteNonQuery("use mysql;");
                }
                else
                {
                    DoExecuteNonQuery("use " + myInfo.Database + ";");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

    }
}
