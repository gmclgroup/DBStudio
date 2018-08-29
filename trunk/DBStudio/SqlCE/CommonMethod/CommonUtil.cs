//=============================================================================
//    DBStudio
//    Copyright (C) 2006  ms44

//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation; either
//    version 2 of the License, or (at your option) any later version.

//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.

//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

//    If you have any questions ,please contact me via 54715112@qq.com
//===============================================================================

//We know currently we have not supported image type ,but we still want to proceed it then use this macro.
//otherwise comment it.
#define IGNORE_IMAGE_ERROR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using XLCS.Common;
using System.Windows.Forms;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;
using DBStudio.GlobalDefine;
using ETL;

using System.Data.Common;
using CoreEA;
using CoreEA.LoginInfo;
using CoreEA.Args;
using System.Data.SqlClient;
using CoreEA.SchemaInfo;



namespace DBStudio.CommonMethod
{
    /// <summary>
    /// utility method to Sync data between some databases
    /// </summary>
    class CommonUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool SwitchToBool(object str)
        {
            if (str == null)
            {
                return false;
            }

            return str.ToString() == "Yes" ? true : false;
        }

        /// <summary>
        /// Accesss to sqlce
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sdfFile"></param>
        public static bool SyncMdbToSdf(string mdbFile, string sdfFile, bool NeedCopyData, string targetDbPwd)
        {
            bool result = false;

            if (!File.Exists(mdbFile))
            {
                "ImportData_FileNotFound".GetFromResourece().Notify();
                return false;
            }

            ICoreEAHander srcEngine = new CoreEA.CoreE(CoreE.UsedDatabaseType.OleDb).X_Handler;

            srcEngine.Open(new LoginInfo_Oledb() { Database = mdbFile });

            if (!srcEngine.IsOpened)
            {
                "ImportData_ReadError".GetFromResourece().Notify();
                return false;
            }
            //Filter system table 
            List<string> tableList = new List<string>();
            foreach (string item in srcEngine.GetTableListInDatabase())
            {
                if (!item.StartsWith("MSys"))
                {
                    tableList.Add(item);
                }
            }

            if (tableList == null)
            {
                "ImportData_NoTable".GetFromResourece().Notify();
                return false;
            }
            ICoreEAHander destEngine = new CoreEA.CoreE(CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            if (!File.Exists(sdfFile))
            {
                if (!destEngine.CreateDatabase(new LoginInfo_SSCE() { DbName = sdfFile, IsEncrypted = false, IsCaseSensitive = false }))
                {
                    "ImportData_CreateSSCEFileFailure".GetFromResourece().Notify();
                    return false;
                }
            }

            destEngine.Open(new LoginInfo_SSCE() { DbName = sdfFile, Pwd = targetDbPwd });
            List<string> targetDBList = destEngine.GetTableListInDatabase();

            try
            {
                foreach (string tableName in tableList)
                {
                    //Don't import table which name has existed.
                    if (targetDBList.Contains(tableName))
                    {
                        continue;
                    }
                    string sqlCeTableName = tableName;
                    //if (Properties.Settings.Default.IsAllowAutoParseInvalidCharsInTableName)
                    //{
                    //   sqlCeTableName= sqlCeTableName.Replace(" ", "");
                    //}

                    string strconnection = string.Format("provider=microsoft.jet.oledb.4.0;data source={0}", mdbFile);
                    ADODB.Connection conn = new ADODB.ConnectionClass();
                    //conn.ConnectionString = strconnection;
                    conn.Open(strconnection, "Admin", "", 0);
                    //Prepare to retrive schema info from access via COM
                    ADOX.Catalog catelog = new ADOX.CatalogClass();
                    catelog.let_ActiveConnection(conn);
                    ADOX.Table tempTable = catelog.Tables[tableName];

                    //Start Generate the Create Sdf table command
                    string tempCreateTableCmd = string.Empty;
                    tempCreateTableCmd = String.Format("CREATE TABLE [{0}] ", sqlCeTableName);
                    string tempSechma = string.Empty;

                    for (int i = 0; i < tempTable.Columns.Count; i++)
                    {
                        Debug.WriteLine("Source Field Name ------>" + tempTable.Columns[i].Name);
                        tempSechma += String.Format("[{0}] {1},",
                            tempTable.Columns[i].Name,
                            CoreEA.Utility.TypeConvertor.ParseADODbTypeToSqlCeDbType(tempTable.Columns[i].Type.ToString(),
                            tempTable.Columns[i].DefinedSize)
                            );
                    }

                    tempSechma = tempSechma.Substring(0, tempSechma.Length - 1);
                    tempCreateTableCmd = String.Format("{0} ({1})", tempCreateTableCmd, tempSechma);

                    if (destEngine.DoExecuteNonQuery(tempCreateTableCmd) != -1)
                    {
                        return false;
                    }
                    if (NeedCopyData)
                    {
                        CopyTable(srcEngine.GetConnection(), (SqlCeConnection)destEngine.GetConnection(),
                            string.Format("Select * from [{0}]", tableName), sqlCeTableName);
                    }
                }
                result = true;
            }
            catch (Exception ee)
            {
                ee.HandleMyException();

                //((SqlCeDatabase)destEngine.DbHandler).CloseSharedConnection();
            }


            return result;
        }


        /// <summary>
        /// Insert data from source to target handler
        /// If columns has image type ,we should pay attention and we will use another way to process is 
        /// </summary>
        /// <param name="srcHandle"></param>
        /// <param name="targetHandle"></param>
        /// <param name="tableName"></param>
        public static bool ExchangeDataBetweenAnyDbs(ICoreEAHander srcHandle, ICoreEAHander targetHandle, string tableName)
        {
            bool ret = false;
            try
            {
                bool isContainImageColumn = false;

                //read data form source table
                DbCommand srcCommand = srcHandle.GetConnection().CreateCommand();
                srcCommand.CommandText = "select * from " +targetHandle.GetMaskedTableName(tableName);

                IDataReader srcReader = srcCommand.ExecuteReader();
                object[] values;

                while (srcReader.Read())
                {
                    //This map is help manage the column name and column value 
                    //for string builder to make it
                    Dictionary<string, object> valueMap = new Dictionary<string, object>();

                    BaseTableSchema schema = srcHandle.GetTableSchemaInfoObject(tableName);



                    StringBuilder st = new StringBuilder();
                    st.Append(string.Format("insert into {0} (",targetHandle.GetMaskedTableName(tableName)));
                    //These two vars to allow do not add , in last column and values
                    int allColumnCount = schema.Columns.Count;
                    int tempColumnCursor = 0;
                    foreach (var column in schema.Columns)
                    {
                        tempColumnCursor++;
                        st.Append(targetHandle.GetMaskedColumnName(column.ColumnName));
                        if (tempColumnCursor != allColumnCount)
                        {
                            st.Append(",");
                        }

                        valueMap.Add(column.ColumnName, null);

                    }
                    st.Append(") Values (");

                    #region Get Maps

                    values = new object[srcReader.FieldCount];
                    srcReader.GetValues(values);

                    for (int i = 0; i < srcReader.FieldCount; i++)
                    {
                        valueMap[srcReader.GetName(i)] = srcReader.GetValue(i);
                    }

                    #endregion
                    //These two vars to allow do not add , in last column and values
                    int last = valueMap.Count;
                    int tempCursor = 0;
                    foreach (var item in valueMap)
                    {
                        tempCursor++;
                        Type curObjectType = item.Value.GetType();
                        Debug.WriteLine("Current object type is " + curObjectType);
                        //If empty column append the '' 
                        if ((item.Value == null) || (item.Value.ToString() == string.Empty))
                        {
                            st.Append(@"''");
                        }
                        else
                        {
                            #region All Kinds Filter (Vital)

                            string filtedValue = item.Value.ToString().Replace("'", "");
                            //filtedValue = filtedValue.Replace("\r", "char(10)");
                            //filtedValue = filtedValue.Replace("\n", "char(13)");

                            //filtedValue = filtedValue.Replace("\r\n", @"\r\n");
                            //if string type , 
                            //filter '
                            //append '' in header and footer
                            if ((curObjectType == typeof(string))
                              || (curObjectType == typeof(char))
                              )
                            {
                                st.Append(string.Format("'{0}'", filtedValue));
                            }
                            else if (curObjectType == typeof(bool))
                            {
                                if (item.Value.ToString().ToLower() == "true")
                                {
                                    st.Append(string.Format("1"));
                                }
                                else
                                {
                                    st.Append(string.Format("0"));
                                }
                            }
                            else if (curObjectType == typeof(Byte[]))
                            {
                                //Please pay attention on Image type.
                                //Looks like there is no way to process image (byte) data type directly in sql
                                //We recommend to use Sqlparameter to insert Image Data.
                                isContainImageColumn = true;
                                break;
                                //string tempstr = XLCS.Common.CharExchange.GetString((Byte[])item.Value);
                                
                                //st.Append(string.Format("'{0}'", tempstr));
                            }
                            else if (curObjectType == typeof(DateTime))
                            {
                                st.Append(string.Format("'{0}'", filtedValue));
                            }
                            else if (curObjectType == typeof(System.Guid))
                            {
                                st.Append(string.Format("'{0}'", filtedValue));
                            }
                            else
                            {

                                st.Append(string.Format("{0}", filtedValue));
                            }
                            #endregion
                        }

                        if (tempCursor != last)
                        {
                            st.Append(",");
                        }

                    }
                    try
                    {
#if IGNORE_IMAGE_ERROR
                        st.Append(")");
                        string insertCmdText = st.ToString();
                        DbCommand targetCommand = targetHandle.GetConnection().CreateCommand();
                        targetCommand.CommandText = insertCmdText;
                        targetCommand.ExecuteNonQuery();
#else
                        if (isContainImageColumn)
                        {
                            throw new NotImplementedException();

                            DbCommand cmd = targetHandle.GetNewCommand();
                            //cmd.Parameters.Add("@images", SqlDbType.Image).Value = imageb;
                        }
                        else
                        {
                            st.Append(")");
                            string insertCmdText = st.ToString();
                            DbCommand targetCommand = targetHandle.GetConnection().CreateCommand();
                            targetCommand.CommandText = insertCmdText;
                            Debug.WriteLine(insertCmdText);
                            targetCommand.ExecuteNonQuery();
                            Debug.WriteLine("Execute ok --------->" + insertCmdText);
                        }
#endif
                    }
                    catch (Exception myEx)
                    {
                        throw myEx;
                    }
                }

                ret = true;
            }
            catch (Exception eee)
            {
                throw eee;
            }
            return ret;
        }


        /// <summary>
        /// mysql to sqlce
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="sdfFile"></param>
        /// <param name="isCopyData"></param>
        /// <returns></returns>
        public static bool SyncDataFromMysqlToSqlCE(string server, string database, string username, string pwd, string sdfFile, bool isCopyData)
        {
            bool result = false;

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(sdfFile))
            {
                throw new Exception("Not valid parameters");
            }

            ICoreEAHander targetEnginer = new CoreEA.CoreE(CoreE.UsedDatabaseType.SqlCE35).X_Handler;
            if (!File.Exists(sdfFile))
            {
                if (!targetEnginer.CreateDatabase(new LoginInfo_SSCE() { DbName = sdfFile }))
                {
                    "ImportData_CreateSSCEFileFailure".GetFromResourece().Notify();
                    return false;
                }
            }

            targetEnginer.Open(new LoginInfo_SSCE() { DbName = sdfFile, Pwd = "", IsEncrypted = false, CurOpenMode = OpenMode.ReadWrite });

            ICoreEAHander srcEngineer = new CoreEA.CoreE(CoreE.UsedDatabaseType.MySql).X_Handler;

            try
            {
                srcEngineer.Open(new LoginInfo_MySql() { Server = server, Database = database, Username = username, Pwd = pwd });

                List<string> tableList = srcEngineer.GetTableListInDatabase(database);

                if (tableList.Count <= 0)
                {
                    "ImportData_NoTable".GetFromResourece().Notify();
                    return false;
                }
                List<string> targetDbList = targetEnginer.GetTableListInDatabase();

                foreach (string tableName in tableList)
                {
                    //Don't import table which name has existed.
                    if (targetDbList.Contains(tableName))
                    {
                        continue;
                    }
                    string sqlCeTableName = tableName;


                    DataTable tempTable = srcEngineer.GetColumnInfoFromTable(tableName);

                    //Start Generate the Create Sdf table command
                    string tempCreateTableCmd = string.Empty;
                    tempCreateTableCmd = String.Format("CREATE TABLE [{0}] ", sqlCeTableName);
                    string tempSechma = string.Empty;

                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        Debug.WriteLine("Source Field Name ------>" + tempTable.Rows[i]["COLUMN_NAME"].ToString());
                        tempSechma += String.Format("{0} {1},", tempTable.Rows[i]["COLUMN_NAME"].ToString(),
                            CoreEA.Utility.TypeConvertor.ParseMySqlDbTypeToSqlCeDbType(tempTable.Rows[i]["COLUMN_TYPE"].ToString()));
                    }

                    tempSechma = tempSechma.Substring(0, tempSechma.Length - 1);
                    tempCreateTableCmd = String.Format("{0} ({1})", tempCreateTableCmd, tempSechma);

                    if (targetEnginer.DoExecuteNonQuery(tempCreateTableCmd) != -1)
                    {
                        throw new Exception(string.Format("Create table {0} error", tableName));
                    }

                    if (isCopyData)
                    {
                        CommonUtil.CopyTable(srcEngineer.GetConnection(),
                            (SqlCeConnection)targetEnginer.GetConnection(),
                            String.Format("select * from `{0}`", tableName), sqlCeTableName);
                    }

                    result = true;
                }

            }
            catch (Exception ee)
            {
                ee.HandleMyException();
                //((SqlCeDatabase)targetEnginer.DbHandler).CloseSharedConnection();
                if (File.Exists(sdfFile))
                {
                    File.Delete(sdfFile);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="core"></param>
        /// <param name="targetCeDBFile"></param>
        /// <param name="prcessTableList"></param>
        /// <param name="isNeedCopyData"></param>
        /// <returns></returns>
        public static List<SyncResultArgs> SyncDataFromSqlServerToSSCE(CoreEA.ICoreEAHander srcEngine, string targetCeDBFile,
            List<string> prcessTableList, bool isNeedCopyData)
        {
            List<SyncResultArgs> resultInfo = null;

            if (!srcEngine.IsOpened)
            {
                throw new ArgumentException("Need opened core object");
            }

            ICoreEAHander destEngine = new CoreEA.CoreE(CoreE.UsedDatabaseType.SqlCE35).X_Handler;

            //IF the ce database not existed ,then create it . 
            if (!File.Exists(targetCeDBFile))
            {
                if (!destEngine.CreateDatabase(new LoginInfo_SSCE() { DbName = targetCeDBFile, Pwd = "" }))
                {
                    "ImportData_CreateSSCEFileFailure".GetFromResourece().Notify();
                    return null;
                }
            }

            destEngine.Open(new LoginInfo_SSCE() { DbName = targetCeDBFile, Pwd = "", IsEncrypted = false, CurOpenMode = OpenMode.ReadWrite });

            try
            {
                List<string> tableList = srcEngine.GetTableListInDatabase();
                if (tableList.Count <= 0)
                {
                    "ImportData_NoTable".GetFromResourece().Notify();
                    return null;
                }

                resultInfo = new List<SyncResultArgs>();

                foreach (string srcSqlServerTableName in tableList)
                {
                    if (prcessTableList.Count > 0)
                    {
                        //If not in the need process table list ,then do not process it . 
                        if (!prcessTableList.Contains(srcSqlServerTableName))
                        {
                            continue;
                        }
                    }

                    string sqlCeTableName = srcSqlServerTableName;
                    DataTable tempDs = srcEngine.GetColumnInfoFromTable(srcSqlServerTableName);

                    DataTable tempTable = tempDs;

#if DEBUG
                    tempDs.WriteXml(GlobalDefine.MyGlobal.GlobalDebugFolder+"SourceSchema.xml");
#else
#endif

                    SyncResultArgs args = new SyncResultArgs();
                    args.TableName = srcSqlServerTableName;

                    //Start Generate the Create Sdf table command
                    string tempCreateTableCmd = string.Empty;
                    tempCreateTableCmd = String.Format("CREATE TABLE [{0}] ", sqlCeTableName);
                    string tempSechma = string.Empty;

                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        Debug.WriteLine("Source Field Name ------>" + tempTable.Rows[i]["COLUMN_NAME"].ToString());

                        //获取每个字段的类型和长度,If null then Each Type Define the size themself 
                        //in ParseSqlServerDbTypeToSqlCeDbType method
                        int? length = null;

                        string lenNode =
                            CoreEA.Utility.TypeConvertor.ParseSqlServerLengthNodeNameFromTypeName(
                            tempTable.Rows[i]["DATA_TYPE"].ToString()
                            );

                        if ((!string.IsNullOrEmpty(lenNode)) && ((tempTable.Rows[i][lenNode] != DBNull.Value)))
                        {
                            length = int.Parse(tempTable.Rows[i][lenNode].ToString());
                        }

                        //建上述结果转换成SSCE 类型和语法
                        string appendix = CoreEA.Utility.TypeConvertor.ParseSqlServerDbTypeToSqlCeDbType(tempTable.Rows[i]["DATA_TYPE"].ToString(), length);

                        tempSechma += String.Format("{0} {1},", tempTable.Rows[i]["COLUMN_NAME"].ToString(), appendix);
                    }

                    tempSechma = tempSechma.Substring(0, tempSechma.Length - 1);
                    tempCreateTableCmd = String.Format("{0} ({1})", tempCreateTableCmd, tempSechma);

                    if (destEngine.DoExecuteNonQuery(tempCreateTableCmd) != -1)
                    {
                        args.LastErrorMsg = "Can't Create Target Table";
                        args.ProcessStatus = false;

                        resultInfo.Add(args);
                        //如果出错，继续执行下一次转换
                        continue;
                    }

                    if (isNeedCopyData)
                    {
                        CommonUtil.CopyTable(srcEngine.GetConnection(), (SqlCeConnection)destEngine.GetConnection(),
                            String.Format("select * from {0}", srcSqlServerTableName), sqlCeTableName);
                    }

                    args.ProcessStatus = true;

                    resultInfo.Add(args);
                }

            }
            catch (Exception ee)
            {
                ee.HandleMyException();

                //((SqlCeDatabase)destEngine.DbHandler).CloseSharedConnection();

                if (File.Exists(targetCeDBFile))
                {
                    File.Delete(targetCeDBFile);
                }
            }

            return resultInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly string MySqlServiceName = "MySQL";
        /// <summary>
        /// 
        /// </summary>
        public class ResultArgs
        {
            public ServiceControllerStatus status { get; set; }
            public bool HasThisService { get; set; }
        }

        /// <summary>
        /// Start windows services
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool StartServices(string serviceName)
        {
            bool result = false;
            ServiceController[] f = ServiceController.GetServices();

            foreach (ServiceController c in f)
            {
                if (c.ServiceName == MySqlServiceName)
                {
                    if (c.Status != ServiceControllerStatus.Running)
                    {
                        try
                        {
                            c.Start();
                            result = true;
                            break;
                        }
                        catch (Exception ee)
                        {
                            result = false;
                            MessageBox.Show(ee.Message);
                        }
                    }

                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static ResultArgs DetectMySqlService()
        {
            ServiceControllerStatus Cstatus = ServiceControllerStatus.StopPending;
            bool findMySql = false;
            string lab_ServiceStatus = string.Empty;
            ServiceController[] f = ServiceController.GetServices();
            foreach (ServiceController c in f)
            {
                if (c.ServiceName == MySqlServiceName)
                {
                    Cstatus = c.Status;
                    findMySql = true;
                    break;
                }
            }

            return new ResultArgs() { HasThisService = findMySql, status = Cstatus };

        }

        /// <summary>
        /// Copy Other Connection Table to SQLCE 
        /// So The DestConn is should be SqlCeConnection
        /// </summary>
        /// <param name="srcConn"></param>
        /// <param name="destConn">Must Sqlce Connection Due to SqlCeResultSet Object</param>
        /// <param name="queryString"></param>
        /// <param name="destTableName"></param>
        public static void CopyTable(
            DbConnection srcConn,
            SqlCeConnection destConn,
            string queryString,
            string destTableName)
        {
            IDbCommand srcCommand = srcConn.CreateCommand();
            srcCommand.CommandText = queryString;

            if (destConn.State == ConnectionState.Closed)
            {
                destConn.Open();
            }
            SqlCeCommand destCommand = destConn.CreateCommand();
            destCommand.CommandType = CommandType.TableDirect; //基于表的访问，性能更好
            destCommand.CommandText = destTableName;

            IDataReader srcReader = srcCommand.ExecuteReader();

            SqlCeResultSet resultSet = destCommand.ExecuteResultSet(
                ResultSetOptions.Sensitive |   //检测对数据源所做的更改
                ResultSetOptions.Scrollable |  //可以向前或向后滚动
                ResultSetOptions.Updatable); //允许更新数据

            object[] values;
            SqlCeUpdatableRecord record;
            //这个方法由于前面ADO读取的列信息已经被排序，所以和数据库中真实的RECORD排序冲突。
            //所以使用下面的新的方法，使用列名寻找
            //while (srcReader.Read())
            //{
            //    // 从源数据库表读取记录
            //    values = new object[srcReader.FieldCount];
            //    srcReader.GetValues(values);

            //    // 把记录写入到目标数据库表
            //    record = resultSet.CreateRecord() ;

            //    record.SetValues(values);
            //    resultSet.Insert(record);
            //}
            while (srcReader.Read())
            {
                values = new object[srcReader.FieldCount];
                srcReader.GetValues(values);
                record = resultSet.CreateRecord();
                for (int i = 0; i < srcReader.FieldCount; i++)
                {
                    record[srcReader.GetName(i)] = srcReader.GetValue(i);
                }
                resultSet.Insert(record);
            }

            srcReader.Close();
            resultSet.Close();

        }


        internal static bool ConvertEXCELToSdf(string excelFile, string sdfFile, bool NeedCopyData, string targetDbPwd
            , bool isFirstRowIsColumnName)
        {
            bool result = false;

            if (!File.Exists(excelFile))
            {
                "ImportData_FileNotFound".GetFromResourece().Notify();
                return false;
            }

            ICoreEAHander srcEngineer = new CoreEA.CoreE(CoreE.UsedDatabaseType.Excel).X_Handler;
            srcEngineer.Open(new LoginInfo_Excel() { Database = excelFile, IsFirstRowIsColumnName = isFirstRowIsColumnName });

            if (!srcEngineer.IsOpened)
            {
                "ImportData_ReadError".GetFromResourece().Notify();
                return false;
            }
            ICoreEAHander targetEngineer = new CoreEA.CoreE(CoreE.UsedDatabaseType.SqlCE35).X_Handler;

            if (!File.Exists(sdfFile))
            {
                if (!targetEngineer.CreateDatabase(new LoginInfo_SSCE() { DbName = sdfFile }))
                {
                    "ImportData_CreateSSCEFileFailure".GetFromResourece().Notify();
                    return false;
                }
            }

            targetEngineer.Open(new LoginInfo_SSCE() { DbName = sdfFile, Pwd = "", IsEncrypted = false, CurOpenMode = OpenMode.ReadWrite });
            if (!targetEngineer.IsOpened) { throw new Exception("Can't open sdf file"); }

            List<string> tableList = srcEngineer.GetTableListInDatabase();
            List<string> targetTableList = targetEngineer.GetTableListInDatabase();

            try
            {
                foreach (string tableName in tableList)
                {
                    //If the table existed in the target db , then skip this table 
                    if (targetTableList.Contains(tableName))
                    {
                        Debug.WriteLine(string.Format("Skip import table {0} ,because the table with this name was existed", tableName));
                        continue;
                    }

                    string sqlCeTableName = tableName;

                    string strconnection = CoreEA.ConnSTR.DbConnectionString.Excel.GetOleDbConnectionString(excelFile, isFirstRowIsColumnName);

                    ADODB.Connection conn = new ADODB.ConnectionClass();
                    conn.Open(strconnection, "", "", 0);

                    //Prepare to retrive schema info from access via COM
                    ADOX.Catalog catelog = new ADOX.CatalogClass();
                    catelog.let_ActiveConnection(conn);
                    ADOX.Table tempTable = catelog.Tables[tableName];

                    //Start Generate the Create Sdf table command
                    string tempCreateTableCmd = string.Empty;
                    tempCreateTableCmd = String.Format("CREATE TABLE [{0}] ", sqlCeTableName);
                    string tempSechma = string.Empty;

                    for (int i = 0; i < tempTable.Columns.Count; i++)
                    {
                        Debug.WriteLine("Source Field Name ------>" + tempTable.Columns[i].Name);
                        tempSechma += String.Format("{0} {1},",
                            tempTable.Columns[i].Name,
                            CoreEA.Utility.TypeConvertor.ParseADODbTypeToSqlCeDbType(tempTable.Columns[i].Type.ToString(),
                            tempTable.Columns[i].DefinedSize)
                            );
                    }


                    tempSechma = tempSechma.Substring(0, tempSechma.Length - 1);
                    tempCreateTableCmd = String.Format("{0} ({1})", tempCreateTableCmd, tempSechma);

                    if (targetEngineer.DoExecuteNonQuery(tempCreateTableCmd) != -1)
                    {
                        throw new Exception(string.Format("Create table {0} error", tableName));
                    }
                    if (NeedCopyData)
                    {
                        CopyTable(srcEngineer.GetConnection(), (SqlCeConnection)targetEngineer.GetConnection(),
                            string.Format("Select * from [{0}]", tableName), sqlCeTableName);
                    }
                }
                result = true;
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }


            return result;
        }

        /// <summary>
        /// Mose Like Excel 
        /// Just different in Connection String
        /// </summary>
        /// <param name="csvFile"></param>
        /// <param name="sdfFile"></param>
        /// <param name="p"></param>
        /// <param name="p_4"></param>
        /// <returns></returns>
        internal static bool ConvertCSVToSdf(string csvFile, string sdfFile, bool NeedCopyData, string targetDbPwd, bool isFirstRowIsColumnName)
        {
            bool result = false;

            if (!File.Exists(csvFile))
            {
                "ImportData_FileNotFound".GetFromResourece().Notify();
                return false;
            }

            ICoreEAHander srcEngine = new CoreEA.CoreE(CoreE.UsedDatabaseType.CSV).X_Handler;
            srcEngine.Open(new LoginInfo_CSV() { Database = csvFile, IsFirstRowIsColumnName = isFirstRowIsColumnName });

            if (!srcEngine.IsOpened)
            {
                "ImportData_ReadError".GetFromResourece().Notify();
                return false;
            }

            List<string> tableList = srcEngine.GetTableListInDatabase();

            ICoreEAHander destEngine = new CoreEA.CoreE(CoreE.UsedDatabaseType.SqlCE35).X_Handler;

            //IF the ce database not existed ,then create it . 
            if (!File.Exists(sdfFile))
            {
                if (!destEngine.CreateDatabase(new LoginInfo_SSCE() { DbName = sdfFile }))
                {
                    "ImportData_CreateSSCEFileFailure".GetFromResourece().Notify();
                    return false;
                }
            }

            destEngine.Open(new LoginInfo_SSCE() { DbName = sdfFile, Pwd = "", IsEncrypted = false, CurOpenMode = OpenMode.ReadWrite });
            List<string> targetDbList = destEngine.GetTableListInDatabase();
            try
            {
                foreach (string tableName in tableList)
                {
                    //Don't import table which name has existed.
                    if (targetDbList.Contains(tableName))
                    {
                        continue;
                    }

                    string sqlCeTableName = tableName;

                    string strconnection = CoreEA.ConnSTR.DbConnectionString.TxtFile.OleDb_DelimitedColumns(csvFile, true);
                    ADODB.Connection conn = new ADODB.ConnectionClass();
                    //conn.ConnectionString = strconnection;
                    conn.Open(strconnection, "", "", 0);

                    //Prepare to retrive schema info from access via COM
                    ADOX.Catalog catelog = new ADOX.CatalogClass();
                    catelog.let_ActiveConnection(conn);
                    ADOX.Table tempTable = catelog.Tables[tableName];

                    //Start Generate the Create Sdf table command
                    string tempCreateTableCmd = string.Empty;
                    tempCreateTableCmd = String.Format("CREATE TABLE [{0}] ", sqlCeTableName);
                    string tempSechma = string.Empty;

                    for (int i = 0; i < tempTable.Columns.Count; i++)
                    {
                        Debug.WriteLine("Source Field Name ------>" + tempTable.Columns[i].Name);
                        tempSechma += String.Format("{0} {1},",
                            tempTable.Columns[i].Name,
                            CoreEA.Utility.TypeConvertor.ParseADODbTypeToSqlCeDbType(tempTable.Columns[i].Type.ToString(),
                            tempTable.Columns[i].DefinedSize)
                            );
                    }


                    tempSechma = tempSechma.Substring(0, tempSechma.Length - 1);
                    tempCreateTableCmd = String.Format("{0} ({1})", tempCreateTableCmd, tempSechma);

                    if (destEngine.DoExecuteNonQuery(tempCreateTableCmd) != -1)
                    {
                        throw new Exception(string.Format("Create table {0} error", tableName));
                    }
                    if (NeedCopyData)
                    {
                        CopyTable(srcEngine.GetConnection(), (SqlCeConnection)destEngine.GetConnection(),
                            string.Format("Select * from [{0}]", tableName), sqlCeTableName);
                    }
                }
                result = true;
            }
            catch (Exception ee)
            {
                ee.HandleMyException();
            }


            return result;
        }
    }
}
