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

namespace CoreEA.Excel
{
    internal class ExcelRobot : BaseRobot
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
                    currentCommandTextHandler = new ExcelCommandText();
                }
                return currentCommandTextHandler;
            }
        }

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


        /// <summary>
        /// <TABLE_NAME>Sheet1$</TABLE_NAME>
        //<COLUMN_NAME>F1</COLUMN_NAME>
        //<ORDINAL_POSITION>1</ORDINAL_POSITION>
        //<COLUMN_HASDEFAULT>false</COLUMN_HASDEFAULT>
        //<COLUMN_FLAGS>106</COLUMN_FLAGS>
        //<IS_NULLABLE>true</IS_NULLABLE>
        //<DATA_TYPE>130</DATA_TYPE>
        //<CHARACTER_MAXIMUM_LENGTH>255</CHARACTER_MAXIMUM_LENGTH>
        //<CHARACTER_OCTET_LENGTH>510</CHARACTER_OCTET_LENGTH>
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
            tableSchema.TableName = tableName;
            //DataTable dt = GetAllDataFromTable(tableName);
            DataTable dt = baseConn.GetSchema("Columns", new string[] { null, null, tableName, null });
            //dt.TableName = "afsdfasdf";
            //dt.WriteXml("C:\\testdfsd.xml");

            foreach (DataRow item in dt.Rows)
            {
                try
                {
                    BaseColumnSchema schmeaInfo = new BaseColumnSchema();
                    schmeaInfo.ColumnName = item["COLUMN_NAME"].ToString();
                    //
                    schmeaInfo.ColumnType = GetExcelColumnType(item["DATA_TYPE"].ToString());
                    Debug.WriteLine(schmeaInfo.ColumnType);

                    schmeaInfo.CharacterMaxLength = long.Parse(item["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    //As well as set this property
                    //This property is the common property
                    //About property is special property
                    //We recommend use this property rather than above one.
                    schmeaInfo.ColumnLength = schmeaInfo.CharacterMaxLength;

                    schmeaInfo.IsNullable = (item["IS_NULLABLE"].ToString().ToLower() == "true" ? true : false);

                    schmeaInfo.OrdinalPosition = item["ORDINAL_POSITION"].IsDBNull() ?
                                0 : int.Parse(item["ORDINAL_POSITION"].ToString());

                    tableSchema.Columns.Add(schmeaInfo);

                }
                catch(Exception ee)
                {
                    
                }
            }
#if DEBUG
            dt.TableName = "dfd";
            dt.WriteXml("ExcelColumnInfo.xml");
#endif


            return tableSchema;
        }

        /// <summary>
        /// Return string type oledby object 
        /// As to the p value ,please refer to the OleDBType enum
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string GetExcelColumnType(string p)
        {
            switch (p)
            {
                case "72":
                    return "guid";
                case "128":
                    return "binary";
                case "129":
                    return "nvarchar";
                case "130":
                    return "nvarchar";
                case "203":
                    return "nvarchar";
                case "202":
                    return "nvarchar";
                case "201":
                    return "nvarchar";
                case "200":
                    return "nvarchar";
                case "133":
                    return "DateTime";
                case "64":
                    return "DateTime";
                case "139":
                    return "decimal";
                case "131":
                    return "decimal";
                case "205":
                    return "binary";
                case "204":
                    return "binary";
                case "21":
                    return "bigint";
                case "20":
                    return "int";
                case "16":
                    return "int";
                case "17":
                    return "int";
                case "18":
                    return "int";
                case "19":
                    return "int";
                case "2":
                    return "int";
                case "3":
                    return "int";
                case "4":
                    return "int";
                case "5":
                    return "double";
                case "6":
                    return "currency";
                case "7":
                    return "Date";
                case "11":
                    return "bool";

                case "8":
                    return "nvarchar";
                case "14":
                    return "decimal";
                default:
                    throw new NotSupportedException();
            }


        }



        /// <summary>
        /// No Index Info
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
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

        public sealed override CoreE.UsedDatabaseType HostedType
        {
            get
            {
                return CoreE.UsedDatabaseType.Excel;
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

            LoginInfo_Excel myInfo = pInfo as LoginInfo_Excel;
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
                    myInfo = new LoginInfo_Excel();

                    myInfo.Database = allInfo.Database;
                    myInfo.Pwd = allInfo.Pwd;
                    myInfo.Username = allInfo.Username;
                    myInfo.CurrentOleDBVersion = allInfo.CurrentOleDBVersion;
                }
                switch (myInfo.CurrentOleDBVersion)
                {
                    case OleDBVersion.Is2003:
                        myConnString = DbConnectionString.Excel.GetOleDbConnectionString(myInfo.Database, myInfo.IsFirstRowIsColumnName);

                        break;
                    case OleDBVersion.Is2007:
                        myConnString = DbConnectionString.Excel2007.GetOleDBString(myInfo.Database, myInfo.IsFirstRowIsColumnName);

                        break;
                    default:
                        myConnString = DbConnectionString.Excel.GetOleDbConnectionString(myInfo.Database, myInfo.IsFirstRowIsColumnName);

                        break;
                }

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
