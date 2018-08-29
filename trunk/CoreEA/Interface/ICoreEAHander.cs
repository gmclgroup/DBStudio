using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;
using CoreEA.Args;
using System.ComponentModel;
using CoreEA.LoginInfo;
using CoreEA.SchemaInfo;

namespace CoreEA
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICoreEAHander : IDisposable
    {
        /// <summary>
        /// For Command text collections 
        /// </summary>
        CommandTextBase CurrentCommandTextHandler { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<int> GetDbType();

        /// <summary>
        /// Change username 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool ChangePassword(UserTokenInfo info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnSchema"></param>
        /// <returns></returns>
        bool AddColumnToTable(string tableName,BaseColumnSchema columnSchema);

        /// <summary>
        /// Get the login info 
        /// like get the connection string 
        /// This is useful when reuse it or get the connection info
        /// Such as :Detect which oledbtype current connected (is CSV ,asscess or others)
        /// </summary>
        /// <returns></returns>
        BaseLoginInfo GetLoginInfo();

        /// <summary>
        /// Get the Create primary key sql query string 
        /// </summary>
        /// <param name="pKeyList"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string GetCreatePrimaryKeySqlString(List<string> pKeyList, string tableName);

        /// <summary>
        /// This Method Called By CreateTable
        /// This Method also can be called external if you just need the sql query string
        /// This method will get primarykey , column ,indexes, 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        string GetCreateTableString(BaseTableSchema ts);

        /// <summary>
        /// Create Table
        /// This method create primarykey , column ,indexes, 
        /// Base method
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        bool CreateTable(BaseTableSchema tableSchema);

        /// <summary>
        /// Get The Masked Table Name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        string GetMaskedTableName(string tableName);

        /// <summary>
        /// Get the mask char when query with column name 
        /// </summary>
        /// <returns></returns>
        string GetMaskedColumnName(string columnName);

        /// <summary>
        /// This method will rely on GetTableSchemaInfoObject
        /// so ,please notice :
        /// Some db type such as :SSCE,SqlServer has already implement this method
        /// but others are not . It will process the result from GetTableSchemaInfoObject
        /// so ,if have time, we should implement this method for all db type
        /// 
        /// Get table schema info 
        /// Include :
        /// Column Info
        /// Index Info
        /// Constraint Info
        /// Primrary key info
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        BaseTableSchema GetTableSchemaInfoObject(string tableName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool HasIdentityColumnInTable(string tableName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<BasePrimaryKeyInfo> GetPrimaryKeysFromTable(string tableName);

        /// <summary>
        /// For Reflect using 
        /// Detect the Current CoreEA Type
        /// </summary>
        CoreE.UsedDatabaseType HostedType { get; }

        /// <summary>
        /// Table name max length
        /// Diffient value according to different db type
        /// </summary>
        int MaxTableNameLength { get; }

        /// <summary>
        /// Current Opened Database ,Only useful when connect to Server based Db
        /// </summary>
        string CurDatabase { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string CurPwd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<string> GetSystemViewList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        List<string> GetSystemViewColumnsNameByViewName(string viewName);

        /// <summary>
        /// Remove data in table 
        /// </summary>
        /// <param name="tableName"></param>
        void RemoveAllData(string tableName);

        /// <summary>
        /// Select data from N to N by PKey
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="FromN"></param>
        /// <param name="ToN"></param>
        DataTable SelectDataFromNtoNByPrimaryKey(string tableName, string primaryKeyName, int FromN, int ToN);

        /// <summary>
        /// Delete Duplicate Data
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="matchColumnName"></param>
        void DeleteDuplicateData(string tableName, string primaryKeyName, string matchColumnName);


        /// <summary>
        /// Get the SP from server based DB
        /// </summary>
        /// <returns></returns>
        List<BaseStoredProcedureInfo> GetStoredProceduresList();

        /// <summary>
        /// Get Views
        /// </summary>
        /// <returns></returns>
        List<BaseViewInfo> GetViews();

        /// <summary>
        /// This method currenmly return ONly Text Type Column Data length
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columName"></param>
        /// <returns></returns>
        Decimal GetColumnLength(string tableName, string columName);

        /// <summary>
        /// Get the default value of DbType
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        Object GetDefaultByType(DbType dataType);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DataTable GetSupportedDbType();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetProviderInfoFromTable(string tableName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetKeyColumnInfoFromTable(string tableName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetIndexInfoFromTable(string tableName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<string> GetIndexNameListFromTable(string tableName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="columnValue"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        bool InsertData(string tableName, List<string> columnName, List<object> columnValue, List<DbType> types);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<string> GetColumnNameListFromTable(string tableName);

        /// <summary>
        /// Create table with schema info 
        /// But the schema info is old format , so it is obsolete 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [Obsolete("This method is old, please use CreateTableWithBaseSchemaInfo instead", false)]
        bool CreateTableWithSchemaInfo(List<CreateTableArgs> args, string tableName);

        /// <summary>
        /// Get the column info from the table schema
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetColumnInfoFromTable(string tableName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="paraName"></param>
        /// <param name="paraType"></param>
        /// <param name="paraLength"></param>
        /// <param name="paraValue"></param>
        /// <returns></returns>
        DbCommand GetNewCommandWithGivenParameters(string cmdText, List<string> paraName, List<DbType> paraType, List<int> paraLength, List<object> paraValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        void AddParameters(DbCommand command, params DbParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        void AddParameters(DbCommand command, List<DbParameter> parameters);

        /// <summary>
        /// Get lastest id
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        int GetLastId(DbConnection connection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        object DoExecuteScalar(DbCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cmdBehavior"></param>
        /// <returns></returns>
        IDataReader DoExecuteReader(DbCommand command, CommandBehavior cmdBehavior);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>If -1 indicate failure</returns>
        int DoExecuteNonQuery(DbCommand command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter CreateParameter(string name, DbType type, int size, object value);

        #region Old Method
        /// <summary>
        /// Execute db command and return a result to dataset
        /// </summary>
        /// <param name="dbCmd"></param>
        /// <returns></returns>
        DataSet ExecuteDataList(DbCommand dbCmd);

        /// <summary>
        /// Get Created new Command
        /// </summary>
        /// <returns>DbCommand</returns>
        DbCommand GetNewCommand();

        /// <summary>
        /// Get Created new command with sql text
        /// </summary>
        /// <returns></returns>
        DbCommand GetNewStringCommand(string sql);

        /// <summary>
        /// Get Current Open Connection
        /// </summary>
        /// <returns></returns>
        DbConnection GetConnection();

        /// <summary>
        /// Set the Connection 
        /// This method may be will interrupt the current model.
        /// Please check it ,and pay necessary attention on it . 
        /// </summary>
        /// <returns></returns>
        /// 
        [Description("Need Pay Attention on this method")]
        void SetConnection(DbConnection dbConn);

        /// <summary>
        /// Get Triggers
        /// </summary>
        /// <returns></returns>
        List<BaseTriggerInfo> GetTriggersInfo();

        /// <summary>
        /// Get the Schema Info of current table binding to the connection
        /// ,If you need the sql command or object , 
        /// please use GetTableSchemaInfoObject method
        /// </summary>
        /// <returns></returns>
        DataTable GetSchemaTable();

        /// <summary>
        /// Create Database
        /// This method no need to open connection first
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        bool CreateDatabase(BaseLoginInfo loginInfo);

        /// <summary>
        /// Last Error Msg From Interface 
        /// </summary>
        string LastErrorMsg
        {
            get;
        }
        /// <summary>
        /// Indicate the db is opened or not
        /// Common used when check db connection status 
        /// </summary>
        bool IsOpened
        {
            get;
        }
        /// <summary>
        /// open a db connection
        /// 如果当前的HANDLE 已经打开或者拥有CONNECTION则不会再次创建联接
        /// </summary>
        /// <param name="info">Login info to connect</param>
        void Open(BaseLoginInfo info);

        /// <summary>
        /// Open a db connection using connection string
        /// This is used by website in common.
        /// 如果当前的HANDLE 已经打开或者拥有CONNECTION则不会再次创建联接
        /// </summary>
        /// <param name="connectingString">Connection String</param>
        void Open(string connectingString);

        /// <summary>
        /// Close current db connection
        /// </summary>
        void Close();

        /// <summary>
        /// Get the data as DataSet from the specified table 
        /// If the recordCount set 0 ,it will retrive all the records in the table
        /// </summary>
        /// <param name="tableName">The table info was in info argument</param>
        /// <param name="IsDesc">Orderby desc or not,it used when the orderByOwner is set with non-empty value</param>
        /// <param name="orderbyOwner">which column need orderby,if no need order by ,this should be set with empty string</param>
        /// <param name="recordCount">Count of Retrived Record ,it used when this value is not 0 </param>
        DataTable GetAllDataFromTable(string tableName, string orderbyOwner, bool IsDesc, int recordCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable GetAllDataFromTable(string tableName);

        /// <summary>
        /// Get Current Opened database tableList
        /// Maybe only support in server side database with mutli database host
        /// </summary>
        /// <returns></returns>
        List<string> GetTableListInDatabase();

        /// <summary>
        /// Get the table list in one database
        /// 
        /// Please pay attention to this method 
        /// We have another same name method which no need parameters
        /// When using ServerSide hosted db ,please use this method
        /// otherwise use no parameter method 
        /// </summary>
        /// <param name="databaseName">database name</param>
        /// <returns>data list</returns>
        List<string> GetTableListInDatabase(string databaseName);

        /// <summary>
        /// Create table with input sql command string
        /// </summary>
        /// <param name="sqlCmd">the sql command string</param>
        /// <returns>create successful or not</returns>
        bool CreateTableWithSql(string sqlCmd);

        /// <summary>
        /// Get the Database list in one db server
        /// </summary>
        /// <returns>databases list</returns>
        List<string> GetDatabaseList();

        /// <summary>
        /// Exe Sql Cmd With no result return 
        /// </summary>
        /// <param name="sqlCmd">Sql Command </param>
        /// <returns>indicate the command is successful or failure</returns>
        int DoExecuteNonQuery(string sqlCmd);

        /// <summary>
        /// Exe Sql Cmd with result return
        /// Return The first row and the first column value of the result object
        /// according to the sql command
        /// </summary>
        /// <param name="sqlCmd">sql command string</param>
        /// <returns>The first row and the first column value of the result object</returns>
        object DoExecuteScalar(string sqlCmd);

        /// <summary>
        /// Exe Sql Command with result return
        /// Return the all records in the result
        /// according to the sql command
        /// </summary>
        /// <param name="sqlCmd">sql command string </param>
        /// <returns>All record in the result</returns>
        DataSet ExecuteDataList(string sqlCmd);

        /// <summary>
        /// Get one data from table which column equal the column value 
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="columnName">the specfied column name</param>
        /// <param name="columnValue">ths specfied column name's value</param>
        /// <returns>dataset</returns>
        DataTable GetDataByColumnNameAndValue(string tableName, string columnName, string columnValue);

        /// <summary>
        /// Get Max ID Value in Table 
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="IdName">Id Column Name ,In general is "ID" or "id"</param>
        /// <returns>the max id in the data</returns>
        int GetMaxIDFromTable(string tableName, string IdName);

        /// <summary>
        /// Execture sql procedure 
        /// </summary>
        /// <param name="procedureName">stored procedure name</param>
        /// <param name="varList">list of vars</param>
        /// <param name="dbTypeList">list of vars' type</param>
        /// <param name="objectLengthList">lenght list of objects</param>
        /// <param name="objectList">list of objects</param>
        /// <param name="objectValueList">list of objects' value</param>
        /// <returns>successful or not</returns>
        bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList);


        /// <summary>
        /// Delete one record in table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">column name(or id) </param>
        /// <param name="recordValue">column value</param>
        /// <returns></returns>
        bool DeleteOneRecordInTable(string tableName, string recordId, string recordValue);

        /// <summary>
        /// Execute the command sql via Transaction
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        bool ExecuteTransactionList(List<string> sqlList);

        /// <summary>
        /// Execute the command via Transaction
        /// </summary>
        /// <param name="sqlCmdList"></param>
        /// <returns></returns>
        bool ExecuteTransactionList(List<DbCommand> sqlCmdList);

        /// <summary>
        /// Get DbType from sqlsever datacolumn type string name    
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        DbType GetDbTypeFromTypeName(string typeName);

        #endregion Endof OldMethod

        /// <summary>
        /// Drop a table 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool DeleteTable(string tableName);

        /// <summary>
        /// Some database need call this submitchanges to complete the data actons.
        /// Otherwise the data which operated will be not saved.
        /// </summary>
        void SubmitChanges();
    }
}
