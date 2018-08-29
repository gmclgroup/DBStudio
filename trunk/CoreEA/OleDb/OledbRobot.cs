using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
using CoreEA.Invalidation;
using CoreEA.ConnSTR;
using System.Data.Common;
using CoreEA.Exceptions;
using System.Data.SqlClient;
using CoreEA.LoginInfo;
using CoreEA.SchemaInfo;
using CoreEA.GlobalDefine;
using System.IO;
using CoreEA.OleDb;
namespace CoreEA
{
    internal class OledbRobot : BaseRobot
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
                    currentCommandTextHandler = new OledbCommandText();

                }
                return currentCommandTextHandler;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public string MatchAdoDbType(int dbType)
        {
            string ret = string.Empty;

            switch (dbType.ToString())
            {
                case "20":
                    ret= "BitInt";
                    break;
                case "128":
                    ret= "Binary";
                    break;
                case "11":
                    ret= "bool";
                    break;
                case "8":
                    ret= "BSTR";
                    break;
                case "129":

                    ret= "Char";
                    break;
                case "6":
                    ret= "Currency";
                    break;
                case "7":
                    //Maybe short time
                    ret= "DateTime";
                    break;
                case "133":
                    ret= "DateTime";
                    break;
                case "137":
                    ret= "DateTime";
                    break;
                case "134":
                    ret= "DateTime";
                    break;
                case "135":
                    ret= "Short DateTime";
                    break;
                case "14":
                    ret= "Decimal";
                    break;

                case "5":
                    ret= "Double";
                    break;
                case "72":
                    ret= "GUID";
                    break;
                case "4":
                    ret= "Single";
                    break;
                case "3":
                    ret= "int";
                    break;
                case "205":
                    ret= "Image";
                    break;
                case "201":
                    ret= "varchar";
                    break;
                case "203":
                    ret= "NText";
                    break;
                case "131":
                    ret= "Decimal";
                    break;
                case "2":
                    ret= "SmallInt";
                    break;
                case "16":
                    ret= "TinyInt";
                    break;
                case "21":
                    ret= "UInt64";
                    break;
                case "19":
                    ret= "UInt32";
                    break;
                case "18":
                    ret= "UInt16";
                    break;
                case "17":
                    ret= "tinyInt";
                    break;
                case "204":
                    ret= "varBinary";
                    break;
                case "200":
                    ret= "varchar";
                    break;
                case "12":
                    ret= "variant";
                    break;
                case "139":
                    ret= "varNumeric";
                    break;
                case "202":
                    ret= "nvarchar";
                    break;
                case "130":
                    ret= "nchar";
                    break;
            }

            return ret;
        }

        //[Obsolete("Not Completed")]
        //public sealed override DataTable GetColumnInfoFromTable(string tableName)
        //{
        //    DataTable dt = baseConn.GetSchema("Columns", new string[] { null, null, tableName, null });
        //    DataTable tempDt = dt.Copy();
            
        //    //Clear Data
        //    foreach (DataRow row in tempDt.Rows)
        //    {
        //        row["DATA_TYPE"]=DBNull.Value;
        //    }
        //    //Reset Type
        //    tempDt.Columns["DATA_TYPE"].DataType = typeof(string);

        //    //Copy Data
        //    #region Match ADO Dbtype to .net framework dbtype
        //    for(int i=0;i<dt.Rows.Count;i++)
        //    {
        //        switch (dt.Rows[i]["DATA_TYPE"].ToString())
        //        {
        //            case "20":
        //                tempDt.Rows[i]["DATA_TYPE"] = "BitInt";
        //                break;
        //            case "128":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Binary";
        //                break;
        //            case "11":
        //                tempDt.Rows[i]["DATA_TYPE"] = "bool";
        //                break;
        //            case "8":
        //                tempDt.Rows[i]["DATA_TYPE"] = "BSTR";
        //                break;
        //            case "129":

        //                tempDt.Rows[i]["DATA_TYPE"] = "Char";
        //                break;
        //            case "6":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Currency";
        //                break;
        //            case "7":
        //                //Maybe short time
        //                tempDt.Rows[i]["DATA_TYPE"] = "DateTime";
        //                break;
        //            case "133":
        //                tempDt.Rows[i]["DATA_TYPE"] = "DateTime";
        //                break;
        //            case "137":
        //                tempDt.Rows[i]["DATA_TYPE"] = "DateTime";
        //                break;
        //            case "134":
        //                tempDt.Rows[i]["DATA_TYPE"] = "DateTime";
        //                break;
        //            case "135":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Short DateTime";
        //                break;
        //            case "14":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Decimal";
        //                break;

        //            case "5":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Double";
        //                break;
        //            case "72":
        //                tempDt.Rows[i]["DATA_TYPE"] = "GUID";
        //                break;
        //            case "4":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Single";
        //                break;
        //            case "3":
        //                tempDt.Rows[i]["DATA_TYPE"] = "int";
        //                break;
        //            case "205":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Image";
        //                break;
        //            case "201":
        //                tempDt.Rows[i]["DATA_TYPE"] = "varchar";
        //                break;
        //            case "203":
        //                tempDt.Rows[i]["DATA_TYPE"] = "NText";
        //                break;
        //            case "131":
        //                tempDt.Rows[i]["DATA_TYPE"] = "Decimal";
        //                break;
        //            case "2":
        //                tempDt.Rows[i]["DATA_TYPE"] = "SmallInt";
        //                break;
        //            case "16":
        //                tempDt.Rows[i]["DATA_TYPE"] = "TinyInt";
        //                break;
        //            case "21":
        //                tempDt.Rows[i]["DATA_TYPE"] = "UInt64";
        //                break;
        //            case "19":
        //                tempDt.Rows[i]["DATA_TYPE"] = "UInt32";
        //                break;
        //            case "18":
        //                tempDt.Rows[i]["DATA_TYPE"] = "UInt16";
        //                break;
        //            case "17":
        //                tempDt.Rows[i]["DATA_TYPE"] = "tinyInt";
        //                break;
        //            case "204":
        //                tempDt.Rows[i]["DATA_TYPE"] = "varBinary";
        //                break;
        //            case "200":
        //                tempDt.Rows[i]["DATA_TYPE"] = "varchar";
        //                break;
        //            case "12":
        //                tempDt.Rows[i]["DATA_TYPE"] = "variant";
        //                break;
        //            case "139":
        //                tempDt.Rows[i]["DATA_TYPE"] = "varNumeric";
        //                break;
        //            case "202":
        //                tempDt.Rows[i]["DATA_TYPE"] = "nvarchar";
        //                break;
        //            case "130":
        //                tempDt.Rows[i]["DATA_TYPE"] = "nchar";
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    #endregion 

        //    return dt;
        //}

        public sealed override string GetCreateTableString(BaseTableSchema ts)
        {
            throw new NotImplementedException();
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
            tableSchema.TableName = tableName;

            #region Oldway
            //DataTable dt = GetAllDataFromTable(tableName);

            //foreach (DataColumn item in dt.Columns)
            //{
            //    BaseColumnSchema schmeaInfo = new BaseColumnSchema();

            //    schmeaInfo.ColumnName = item.ColumnName;
            //    schmeaInfo.ColumnType = item.DataType.ToString();

            //    Debug.WriteLine(schmeaInfo.ColumnType);
            //    schmeaInfo.IsAutoIncrement = item.AutoIncrement;
            //    schmeaInfo.AutoIncrementBy = (int)item.AutoIncrementStep;
            //    schmeaInfo.AutoIncrementSeed = (int)item.AutoIncrementSeed;

            //    schmeaInfo.CharacterMaxLength = item.MaxLength;
            //    schmeaInfo.DefaultValue = item.DefaultValue.ToString();

            //    schmeaInfo.IsNullable = item.AllowDBNull;

            //    tableSchema.Columns.Add(schmeaInfo);
            //}
            #endregion 

            DataTable dt = GetColumnInfoFromTable(tableName);
            foreach (DataRow item in dt.Rows)
            {
                BaseColumnSchema columnInfo = new BaseColumnSchema();

                columnInfo.ColumnName = item["COLUMN_NAME"].ToString();
                string tempColumnType = MatchAdoDbType((int)(item["DATA_TYPE"]));
                
                columnInfo.ColumnType = tempColumnType;

                Debug.WriteLine(columnInfo.ColumnType);
                //schmeaInfo.AutoIncrementSeed = (item["AUTOINC_SEED"].IsDBNull() ? 0 : 1);

                columnInfo.CharacterMaxLength = item["CHARACTER_MAXIMUM_LENGTH"].IsDBNull() ? 0 :
                    Int64.Parse(item["CHARACTER_MAXIMUM_LENGTH"].ToString());
                //As well as set this property
                //This property is the common property
                //About property is special property
                //We recommend use this property rather than above one.
                columnInfo.ColumnLength = columnInfo.CharacterMaxLength;
                columnInfo.DefaultValue = item["COLUMN_DEFAULT"].ToString();
                //Different
                //schmeaInfo.IsNullable = (item["IS_NULLABLE"].ToString().ToLower() == "yes" ? true : false);

                columnInfo.NumericPrecision = item["NUMERIC_PRECISION"].IsDBNull() ?
                    0 : int.Parse(item["NUMERIC_PRECISION"].ToString());

                columnInfo.NumericScale = item["NUMERIC_SCALE"].IsDBNull() ?
                    0 : int.Parse(item["NUMERIC_SCALE"].ToString());

                columnInfo.OrdinalPosition = item["ORDINAL_POSITION"].IsDBNull() ?
0 : int.Parse(item["ORDINAL_POSITION"].ToString());

                tableSchema.Columns.Add(columnInfo);
            }

            #region ADO Way

            //string sqlCeTableName = tableName;
            //string strconnection = string.Format("provider=microsoft.jet.oledb.4.0;data source={0}",
            //    CurDatabase);
            //ADODB.Connection conn = new ADODB.ConnectionClass();
            //if (string.IsNullOrEmpty(CurPwd))
            //{
            //    conn.Open(strconnection, "Admin", "", 0);
            //}
            //else
            //{
            //    conn.Open(strconnection, "Admin", CurPwd, 0);
            //}

            //ADOX.Catalog catelog = new ADOX.CatalogClass();
            //catelog.let_ActiveConnection(conn);

            //ADOX.Table tempTable = catelog.Tables[tableName];

            //for (int i = 0; i < tempTable.Columns.Count; i++)
            //{
            //    tableSchema.Columns.Add(new BaseColumnSchema()
            //    {
            //        ColumnName = tempTable.Columns[i].Name,
            //        ColumnType = CoreEA.Utility.TypeConvertor.ParseADODbTypeToSqlCeDbType(tempTable.Columns[i].Type.ToString(),
            //                    tempTable.Columns[i].DefinedSize),

            //        CharacterMaxLength = tempTable.Columns[i].DefinedSize,
            //        NumericScale = tempTable.Columns[i].NumericScale,
            //        IsNullable = true,
            //        NumericPrecision = tempTable.Columns[i].Precision,
            //    });
            //}

            #endregion 

            return tableSchema;
        }

        public sealed override List<BasePrimaryKeyInfo> GetPrimaryKeysFromTable(string tableName)
        {
            return new List<BasePrimaryKeyInfo>();
        }

        public sealed override bool HasIdentityColumnInTable(string tableName)
        {
            return false;
        }

        /// <summary>
        /// This db type don't has such views
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

        public OledbRobot()
        {
        }

        public sealed override CoreE.UsedDatabaseType HostedType
        {
            get
            {
                return CoreE.UsedDatabaseType.OleDb;
            }
        }

        public sealed override int MaxTableNameLength
        {
            get
            {
                return 64;
            }
        }


        public override bool CreateDatabase(BaseLoginInfo loginInfo)
        {
            LoginInfo_Oledb myInfo = loginInfo as LoginInfo_Oledb;
            Debug.Assert(myInfo != null);

            bool result = false;

            try
            {
                ADOX.CatalogClass cat = new ADOX.CatalogClass();

                cat.Create(String.Format("Provider=Microsoft.Jet.OLEDB.4.0;" +
                       "Data Source={0};" +
                       "Jet OLEDB:Engine Type=5", myInfo.Database));

                result = true;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            return result;
        }

        public sealed override DbDataAdapter GetDataAdapter(DbCommand dbCmd)
        {
            OleDbCommand myCmd = dbCmd as OleDbCommand;
            if (myCmd == null) throw new ArgumentException();
            OleDbDataAdapter sa = new OleDbDataAdapter(myCmd);
            return sa;
        }

        public sealed override DbCommand GetNewStringCommand(string sql)
        {
            DbCommand cmd = new OleDbCommand();
            if (!string.IsNullOrEmpty(sql))
            {
                cmd.CommandText = sql;
            }

            cmd.Connection = baseConn;
            return cmd;
        }

        public override List<string> GetDatabaseList()
        {
            throw new NotImplementedException();
        }

        public override decimal GetColumnLength(string tableName, string columnName)
        {
            return 65535M;
        }

        public override string GetMaskedTableName(string tableName)
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
        /// 
        /// </summary>
        /// <param name="databaseName">
        /// In access ,this para is not used ever
        /// Like SSCE ,Recommend use this base method with( no parameter)</param>
        /// <returns></returns>
        public override List<string> GetTableListInDatabase(string databaseName)
        {
            List<string> jieguo = new List<string>();
            OleDbDataReader reader = null;

            //string cmdStr = "SELECT MSysObjects.Name FROM MSysObjects WHERE (((MSysObjects.Flags)=0) AND ((MSysObjects.Type)=1))ORDER BY MSysObjects.Name";
            string cmdStr = string.Empty;
            using (OleDbCommand cmd = new OleDbCommand(cmdStr, (OleDbConnection)baseConn))
            {
                cmd.CommandTimeout = 10;
                //If you wanna Update DataAdapter You must specfie the UpdataCommand/InsertCommand/DeleteCommand etc.. 
                try
                {
                    // We only want user tables, not system tables
                    string[] restrictions = new string[4];
                    restrictions[3] = "Table";

                    DataTable userTables = baseConn.GetSchema("Tables", restrictions);

                    for (int i = 0; i < userTables.Rows.Count; i++)
                        jieguo.Add(userTables.Rows[i][2].ToString());

                    //jieguo = new List<string>();
                    //reader = cmd.ExecuteReader();
                    //while (reader.NextResult())
                    //{
                    //    jieguo.Add(reader[0].ToString());
                    //}
                }
                catch (Exception ee)
                {
                    GlobalDefine.SP.LastErrorMsg = ee.Message;
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


        public override void Open(string connectionString)
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
                baseConn = new OleDbConnection(connectionString);
                baseConn.Open();

                invalidator = new InvalidatorForOledb();
            }
            catch (OleDbException ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// Notice: 
        /// Because OleDB Suppor many db types . 
        /// So the UsingOleDbType property in LoginInfo should be specifed. otherwise will use default db type -->Here is Access
        /// </summary>
        /// <param name="pInfo"></param>
        public override void Open(BaseLoginInfo pInfo)
        {
            //Record to base class (Vital)
            baseLoginInfo = pInfo;


            LoginInfo_Oledb myInfo = pInfo as LoginInfo_Oledb;
            LoginInfo_ForAllDbTypes allInfo = pInfo as LoginInfo_ForAllDbTypes;

            if ((myInfo == null) && (allInfo == null))
            {
                throw new ArgumentException("Only Support Oledb login info and AllDBTypes Info");
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
                    myInfo = new LoginInfo_Oledb();

                    myInfo.Database = allInfo.Database;
                    myInfo.Pwd = allInfo.Pwd;
                    myInfo.Username = allInfo.Username;
                    myInfo.CurrentOleDBVersion = allInfo.CurrentOleDBVersion;
                    //myInfo.CurOleDBType=allInfo.
                }
                #region Get Connection String
                switch (myInfo.CurrentOleDBVersion)
                {
                    case OleDBVersion.Is2003:
                        if (string.IsNullOrEmpty(myInfo.Pwd))
                        {
                            myConnString = DbConnectionString.Access.GetOledbAccess(myInfo.Database, myInfo.Username, myInfo.Pwd);
                        }
                        else
                        {
                            myConnString = DbConnectionString.Access.GetOleDbAccessWithPassword(myInfo.Database, myInfo.Pwd);
                        }
                        break;
                    case OleDBVersion.Is2007:
                        if (string.IsNullOrEmpty(myInfo.Pwd))
                        {
                            myConnString = DbConnectionString.Access2007.GetOleDBString(myInfo.Database,"");
                        }
                        else
                        {
                            myConnString = DbConnectionString.Access2007.GetOleDBString(myInfo.Database, myInfo.Pwd);
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(myInfo.Pwd))
                        {
                            myConnString = DbConnectionString.Access.GetOledbAccess(myInfo.Database, myInfo.Username, myInfo.Pwd);
                        }
                        else
                        {
                            myConnString = DbConnectionString.Access.GetOleDbAccessWithPassword(myInfo.Database, myInfo.Pwd);
                        }
                        break;
                }
                #endregion

                baseConn = new OleDbConnection(myConnString);
                baseConn.Open();

                invalidator = new InvalidatorForOledb();
                CurDatabase = myInfo.Database;
                CurPwd = myInfo.Pwd;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DataTable GetSchemaTable()
        {
            if (!IsOpened)
            {
                return null;
            }
            try
            {
                DataTable table = ((OleDbConnection)baseConn).GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                ////
                //DataTable tempDs = ExecuteSqlCmdAndReturnAllResult("Select * from MSysObjects");
                //tempDs.WriteXml("C:\\SysObject.xml");

                return table;
            }
            catch (OleDbException ee)
            {
                GlobalDefine.SP.LastErrorMsg = ee.Message;
#if DEBUG
                throw new Exception(ee.Message);
#else
                
                return null;
#endif
            }

        }


        public override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {
            throw new NotImplementedException();
        }

    }

}
