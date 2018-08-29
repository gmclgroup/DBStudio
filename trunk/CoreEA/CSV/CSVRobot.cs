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

namespace CoreEA.CSV
{
    internal class CSVRobot : BaseRobot
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
                    currentCommandTextHandler = new CSVCommandText();
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

        public sealed override BaseTableSchema GetTableSchemaInfoObject(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }
            BaseTableSchema tableSchema = new BaseTableSchema();
            tableSchema.TableName = tableName;
            Dictionary<string, DbType> tempValue = GetColumnNameAndTypeFromTable(tableName);
            foreach (var item in tempValue)
            {
                tableSchema.Columns.Add(new BaseColumnSchema()
                {
                    ColumnName = item.Key,
                    ColumnType = item.Value.ToString(),
                });
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
                return CoreE.UsedDatabaseType.CSV;
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
            LoginInfo_CSV myInfo = loginInfo as LoginInfo_CSV;
            Debug.Assert(myInfo != null);

            bool result = false;

            try
            {
                File.Create(myInfo.Database);

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
        /// Due to the CSV has no detail scehma info . 
        /// It just has column name and value 
        /// So This method is very special for CSV
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private Dictionary<string, DbType> GetColumnNameAndTypeFromTable(string tableName)
        {
            if (!IsOpened)
            {
                throw new ConnectErrorException();
            }

            tableName = GetMaskedTableName(tableName);
            Dictionary<string, DbType> result = new Dictionary<string, DbType>();

            using (DataTable ds = new DataTable())
            using (OleDbDataAdapter da = new OleDbDataAdapter(String.Format("select * from {0}",

tableName), (OleDbConnection)baseConn))
            {
                da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                da.Fill(ds);

                foreach (DataColumn dc in ds.Columns)
                {
                    result.Add(dc.ColumnName, GetDbTypeFromTypeName(dc.DataType.Name));
                }
            }

            return result;

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
        /// Here notice :
        /// The database in the connection is a folder name 
        /// And the database name is the file name
        /// In CSV the filename is just the database name and the table name 
        /// 
        /// </summary>
        /// <param name="pInfo"></param>
        public override void Open(BaseLoginInfo pInfo)
        {
            //Record to base class (Vital)
            baseLoginInfo = pInfo;


            LoginInfo_CSV myInfo = pInfo as LoginInfo_CSV;
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
                    myInfo = new LoginInfo_CSV();

                    myInfo.Database = allInfo.Database;
                    myInfo.Pwd = allInfo.Pwd;
                    myInfo.Username = allInfo.Username;
                }

                //Process Database name 
                myInfo.Database = Directory.GetParent(myInfo.Database).FullName;

                myConnString = DbConnectionString.TxtFile.OleDb_DelimitedColumns(myInfo.Database, myInfo.IsFirstRowIsColumnName);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public sealed override bool CreateTable(BaseTableSchema ts)
        {
            bool result = false;
            try
            {
                StreamWriter sw = File.CreateText(CurDatabase+ ts.TableName+".csv");
                sw.Dispose();
                sw.Close();
                result = true;
            }
            catch (Exception ee)
            {
                throw ee;
            }

            return result;
        }
    }
}
