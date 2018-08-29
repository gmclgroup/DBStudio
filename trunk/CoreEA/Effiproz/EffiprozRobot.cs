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
using System.Data.EffiProz;

namespace CoreEA.Effiproz
{
    internal class EffiprozRobot : BaseRobot
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
                    currentCommandTextHandler = new EffiprozCommandText();
                }
                return currentCommandTextHandler;
            }
        }

        public sealed override void SubmitChanges()
        {
            DbCommand cmd = GetNewStringCommand("COMMIT");
            cmd.ExecuteNonQuery();
        }

        public sealed override List<BaseStoredProcedureInfo> GetStoredProceduresList()
        {
            List<BaseStoredProcedureInfo> info = new List<BaseStoredProcedureInfo>();
            DataTable dt = this.GetConnection().GetSchema("PROCEDURES");
#if DEBUG
            dt.TableName = "sp";
            dt.WriteXml("sp.xml");
#endif
            foreach (DataRow item in dt.Rows)
            {
                info.Add(new BaseStoredProcedureInfo()
                {
                    ProcedureName = item["PROCEDURE_NAME"].ToString(),
                    ProcedureCategory = item["PROCEDURE_CAT"].ToString(),
                    ProcedureSchema= item["PROCEDURE_SCHEM"].ToString(),
                    Remarks= item["REMARKS"].ToString(),
                    SpecificName = item["SPECIFIC_NAME"].ToString(),
                    ProcedureType= (BaseSpType)int.Parse(item["PROCEDURE_TYPE"].ToString())
                });
            }

            #region Fill with the cild column info in each sp
            try
            {
                EfzConnection conn = this.GetConnection() as EfzConnection;
                Debug.Assert(null != conn);

                DataTable childDt = conn.GetSchema("PROCEDURECOLUMNS");
                foreach (DataRow childRow in childDt.Rows)
                {
                    string currentSpName = childRow["PROCEDURE_NAME"].ToString();
                    foreach (BaseStoredProcedureInfo childSpInfo in info)
                    {
                        if (childSpInfo.ProcedureName == currentSpName)
                        {
                            BaseColumnInfoInSP columnInfo = new BaseColumnInfoInSP()
                            {
                                COLUMN_NAME = childRow["COLUMN_NAME"].ToString(),
                                COLUMN_SIZE = int.Parse(childRow["COLUMN_SIZE"].ToString()),
                                DATA_TYPE = int.Parse(childRow["DATA_TYPE"].ToString()),
                                NULLABLE = bool.Parse(childRow["NULLABLE"].ToString()),
                                ORDINAL_POSITION = int.Parse(childRow["ORDINAL_POSITION"].ToString()),
                                PRECISION = int.Parse(childRow["PRECISION"].ToString()),
                                SCALE = int.Parse(childRow["SCALE"].ToString()),
                                TYPE_NAME = childRow["TYPE_NAME"].ToString()
                            };
                            break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message);
            }
            #endregion 

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed override CoreE.UsedDatabaseType HostedType
        {
            get
            {
                return CoreE.UsedDatabaseType.Effiproz;
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
                    {
                        sb.Append(", ");
                    }
                } // for
                sb.Append(")\n");
            }
            else
                sb.Append("\n");

            sb.Append(");\n");

            //string stmt = BuildCreateIndex(ts.TableName, ts.Indexes);
            //sb.Append(stmt);

            return sb.ToString();
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
                    createSchemaScript.AppendFormat("\"{0}\" {1}({2}) {3} {4} {5} ",
                         eachColumn.ColumnName,
                         eachColumn.ColumnType,
                         eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "char":
                    createSchemaScript.AppendFormat("\"{0}\" {1}({2}) {3} {4} {5} ",
                         eachColumn.ColumnName,
                         eachColumn.ColumnType,
                         eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "varchar2":
                    createSchemaScript.AppendFormat("\"{0}\" {1}({2}) {3} {4} {5}",
                        eachColumn.ColumnName,
                        eachColumn.ColumnType,
                        eachColumn.CharacterMaxLength,
                         eachColumn.IsNullable == true ? "NULL" : "NOT NULL",
                         eachColumn.DefaultValue.不是空的() ? "DEFAULT " + eachColumn.DefaultValue.ToString() : string.Empty,
                         System.Environment.NewLine);
                    break;
                case "numeric":
                    createSchemaScript.AppendFormat("\"{0}\" {1}({2},{3}) {4} {5} {6} ",
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
                    createSchemaScript.AppendFormat("\"{0}\" {1} {2} {3} {4}{5} {6} ",
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


        public sealed override string GetMaskedColumnName(string columnName)
        {
            if (columnName.StartsWith("\""))
            {
                if (columnName.EndsWith("\""))
                {
                    return columnName;
                }
                else
                {
                    return string.Format("{0}\"", columnName);
                }
            }
            else
            {
                if (columnName.EndsWith("\""))
                {
                    return string.Format("\"{0}", columnName);
                }
                else
                {
                    return string.Format("\"{0}\"", columnName);
                }

            }
        }

        /// <summary>
        /// Below is the column schema
        /// <TABLE_CAT>PUBLIC</TABLE_CAT>
        /// <TABLE_SCHEM>PUBLIC</TABLE_SCHEM>
        /// <TABLE_NAME>TEST</TABLE_NAME>
        /// <COLUMN_NAME>NAME</COLUMN_NAME>
        /// <DATA_TYPE>16</DATA_TYPE>
        /// <TYPE_NAME>VARCHAR2</TYPE_NAME>
        /// <COLUMN_SIZE>100</COLUMN_SIZE>
        /// <NUM_PREC_RADIX>0</NUM_PREC_RADIX>
        /// <NULLABLE>1</NULLABLE>
        /// <SQL_DATA_TYPE>12</SQL_DATA_TYPE>
        /// <CHAR_OCTET_LENGTH>100</CHAR_OCTET_LENGTH>
        /// <ORDINAL_POSITION>3</ORDINAL_POSITION>
        /// <IS_NULLABLE>true</IS_NULLABLE>
        /// <IS_AUTOINCREMENT>false</IS_AUTOINCREMENT>
        ///<TYPE_SUB>1</TYPE_SUB>
        ///<IS_PRIMARY_KEY>false</IS_PRIMARY_KEY>
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
            try
            {
                tableSchema.TableName = tableName;
                tableSchema.PrimaryKey = GetPrimaryKeysFromTable(tableName);
                
                #region Column Info
                DataTable dt = GetColumnInfoFromTable(tableName);

#if DEBUG
                dt.WriteXml("C:\\column1.xml");
#else

#endif
                foreach (DataRow item in dt.Rows)
                {
                    BaseColumnSchema schmeaInfo = new BaseColumnSchema();

                    schmeaInfo.ColumnName = item["COLUMN_NAME"].ToString();
                    //
                    schmeaInfo.ColumnType = item["TYPE_NAME"].ToString();
                    Debug.WriteLine(schmeaInfo.ColumnType);

                    schmeaInfo.CharacterMaxLength = long.Parse(item["COLUMN_SIZE"].ToString());
                    //As well as set this property
                    //This property is the common property
                    //About property is special property
                    //We recommend use this property rather than above one.
                    schmeaInfo.ColumnLength = schmeaInfo.CharacterMaxLength;

                    schmeaInfo.IsNullable = (item["IS_NULLABLE"].ToString().ToLower() == "true" ? true : false);

                    schmeaInfo.OrdinalPosition = item["ORDINAL_POSITION"].IsDBNull() ?
                                0 : int.Parse(item["ORDINAL_POSITION"].ToString());

                    schmeaInfo.IsAutoIncrement = (item["IS_AUTOINCREMENT"].ToString().ToLower() == "true" ? true : false);
                    schmeaInfo.IsIdentity = (item["IS_PRIMARY_KEY"].ToString().ToLower() == "true" ? true : false);
                    tableSchema.Columns.Add(schmeaInfo);
                }
                #endregion

                #region Index Info
                DataTable dtForIndex = GetIndexInfoFromTable(tableName);

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

        /// <summary>
        /// No Index Info
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public sealed override List<BasePrimaryKeyInfo> GetPrimaryKeysFromTable(string tableName)
        {

            return base.GetPrimaryKeysFromTable(tableName);

//            List<BasePrimaryKeyInfo> info = new List<BasePrimaryKeyInfo>();
//            try
//            {
//                DataTable dt = this.GetConnection().GetSchema("PRIMARYKEYS");
//#if DEBUG
//                dt.TableName = "adfadsfdasf";
//                dt.WriteXml("primarykey_effiproz.xml");
//#endif
//                tableName = GetMaskedTableName(tableName);
//                foreach (DataRow item in dt.Rows)
//                {
//                    if (item["TABLE_NAME"].ToString() == tableName)
//                    {
//                        info.Add(new BasePrimaryKeyInfo()
//                        {
//                            ColumnName = item[""].ToString(),

//                        });
//                    }
//                }
//            }
//            catch (Exception ee)
//            {
//                Debug.WriteLine(ee.Message);
//                throw ee;
//            }
//            return info;
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
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public sealed override bool ChangePassword(UserTokenInfo info)
        {
            bool ret = false;
            try
            {
                EfzConnection conn = (EfzConnection)this.GetConnection();
                conn.ChangePassword(info.NewPassword);
                ret = true;
            }
            catch (Exception ee)
            {
                ret = false;
                Debug.WriteLine(ee.Message);
            }
            return ret;
        }

        public override bool CreateDatabase(BaseLoginInfo loginInfo)
        {
            bool result = false;
            string createCmd = string.Empty;
            LoginInfo_Effiproz myInfo = loginInfo as LoginInfo_Effiproz;
            Debug.Assert(myInfo != null);

            createCmd = ConnSTR.DbConnectionString.EffiprozConnStr.GetConnectionString(myInfo.DBConnectionType, myInfo.InitialCatalog, myInfo.Username, myInfo.Password);

            try
            {
                EfzConnection conn = new EfzConnection(createCmd);
                conn.Open();

                result = true;
            }
            catch (Exception ee)
            {
                Debug.Write(ee.Message);
            }

            return result;
        }

        public sealed override DbDataAdapter GetDataAdapter(DbCommand dbCmd)
        {
            EfzCommand myCmd = dbCmd as EfzCommand;
            if (myCmd == null) throw new ArgumentException();
            EfzDataAdapter sa = new EfzDataAdapter(myCmd);
            return sa;
        }

        public sealed override DbCommand GetNewStringCommand(string sql)
        {
            DbCommand cmd = ((EfzConnection)baseConn).CreateCommand();

            if (!string.IsNullOrEmpty(sql))
            {
                cmd.CommandText = sql;
            }

            cmd.Connection = baseConn;
            return cmd;
        }

        /// <summary>
        /// Effiproz is file based db
        /// so this will return empty
        /// </summary>
        /// <returns></returns>
        public override List<string> GetDatabaseList()
        {
            return new List<string>();
        }

        public override decimal GetColumnLength(string tableName, string columnName)
        {
            return 65535M;
        }

        public override string GetMaskedTableName(string tableName)
        {

            if (tableName.StartsWith("\""))
            {
                if (tableName.EndsWith("\""))
                {
                    return tableName;
                }
                else
                {
                    return string.Format("{0}\"", tableName);
                }
            }
            else
            {
                if (tableName.EndsWith("\""))
                {
                    return string.Format("\"{0}", tableName);
                }
                else
                {
                    return string.Format("\"{0}\"", tableName);
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
            EfzDataAdapter reader = null;

            string cmdStr = string.Empty;
            EfzConnection myConn = (EfzConnection)baseConn;
            using (EfzCommand cmd = new EfzCommand(cmdStr, myConn))
            {
                cmd.CommandTimeout = 10;
                try
                {
                    DataTable userTables;

                    userTables = myConn.GetSchema("Tables", new string[] { null, null, null, "TABLE" });
                    Debug.WriteLine("Table count "+userTables.Rows.Count);
                    for (int i = 0; i < userTables.Rows.Count; i++)
                    {
                        jieguo.Add(userTables.Rows[i]["Table_Name"].ToString());
                        Debug.WriteLine(userTables.Rows[i]["Table_Name"].ToString());
                    }
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
                        reader.Dispose();
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
                baseConn = new EfzConnection(connectionString);
                baseConn.Open();

                invalidator = new InvalidatorEffiproz();
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

            LoginInfo_Effiproz myInfo = pInfo as LoginInfo_Effiproz;
            LoginInfo_ForAllDbTypes allInfo = pInfo as LoginInfo_ForAllDbTypes;

            if ((myInfo == null) && (allInfo == null))
            {
                throw new ArgumentException("Only Support Effiproz login info and AllDBTypes Info");
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
                    myInfo = new LoginInfo_Effiproz();
                    myInfo.InitialCatalog = allInfo.Database;
                    myInfo.Username = allInfo.Username;
                    myInfo.Password = allInfo.Pwd;
                }
                myConnString = ConnSTR.DbConnectionString.EffiprozConnStr.GetConnectionString(myInfo.DBConnectionType, myInfo.InitialCatalog,
            myInfo.Username, myInfo.Password);

                baseConn = new EfzConnection(myConnString);
                baseConn.Open();

                invalidator = new InvalidatorEffiproz();
                CurDatabase = myInfo.InitialCatalog;
                CurPwd = myInfo.Password;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {
            bool jieguo = false;
            if (invalidator.IsInvalidArguments(procedureName))
            {
                return false;
            }
            if (!IsOpened) { throw new ConnectErrorException(); }
            try
            {
                using (EfzCommand myCmd = new EfzCommand())
                {
                    myCmd.Connection = (EfzConnection)baseConn;
                    myCmd.CommandType = CommandType.StoredProcedure;
                    myCmd.CommandText = procedureName;
                    for (int i = 0; i < varList.Length; i++)
                    {
                        myCmd.Parameters.Add("@" + varList[i], (EfzType)(dbTypeList[i]), objectLengthList[i], objectList[i].ToString());
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

    }
}
