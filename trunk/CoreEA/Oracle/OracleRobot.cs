using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEA.SchemaInfo;
using CoreEA.LoginInfo;
using System.Diagnostics;
using System.Data.Common;
using System.Data;
using CoreEA.Exceptions;
using System.Data.OleDb;
using CoreEA.ConnSTR;
using System.Data.OracleClient;

namespace CoreEA.Oracle
{
    internal class OracleRobot : BaseRobot
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
                    currentCommandTextHandler = new OracleCommandText();
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

        public sealed override BaseTableSchema GetTableSchemaInfoObject(string tableName)
        {
            throw new NotImplementedException();
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
                return CoreE.UsedDatabaseType.Oracle;
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
            throw new NotImplement();
            //LoginInfo_Oracle myInfo = loginInfo as LoginInfo_Oracle;
            //Debug.Assert(myInfo != null);

            //bool result = false;

            //return result;
        }

        public sealed override DbDataAdapter GetDataAdapter(DbCommand dbCmd)
        {
            OracleCommand myCmd = dbCmd as OracleCommand;
            if (myCmd == null) throw new ArgumentException();
            OracleDataAdapter sa = new OracleDataAdapter(myCmd);
            return sa;
        }

        public sealed override DbCommand GetNewStringCommand(string sql)
        {
            DbCommand cmd = new OracleCommand();
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

        /// <summary>
        /// "" is the mask
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
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


        public override List<string> GetColumnNameListFromTable(string tableName)
        {
            List<string> jieguo = new List<string>();

            DataTable dt = ExecuteDataList(string.Format("select column_name from all_tab_columns where table_name='{0}'"
                ,tableName)).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                jieguo.Add(item["column_name"].ToString());
            }

            return jieguo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override DataTable GetColumnInfoFromTable(string tableName)
        {
            DbCommand cmd = GetNewStringCommand("select * from all_tab_columns where table_name='" +tableName+"'");
            DataTable dt = ExecuteDataList(cmd).Tables[0];
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public override List<string> GetTableListInDatabase(string databaseName)
        {
            List<string> jieguo = new List<string>();

            DbCommand cmd = GetNewStringCommand("select * from tab");

            DataTable dt = ExecuteDataList(cmd).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                jieguo.Add(item["TName"].ToString());
            }
            
            return jieguo;

        }

        public override DataTable GetIndexInfoFromTable(string tableName)
        {
            DbCommand cmd = GetNewStringCommand("select * from all_indexes where table_name='" + tableName + "'");
            DataTable dt = ExecuteDataList(cmd).Tables[0];
            return dt;
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
                baseConn = new OracleConnection(connectionString);
                baseConn.Open();

                //invalidor = new InvalidForOledb();
            }
            catch (OracleException ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// Notice: 
        /// Because OleDB Suppor many db types . 
        /// So the UsingOleDbType property in LoginInfo should be specifed. otherwise will use default db type -->Here is Oracle
        /// </summary>
        /// <param name="pInfo"></param>
        public override void Open(BaseLoginInfo pInfo)
        {
            //Record to base class (Vital)
            baseLoginInfo = pInfo;

            LoginInfo_Oracle myInfo = pInfo as LoginInfo_Oracle;
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
                    myInfo = new LoginInfo_Oracle();

                    //Acctually the SID is the global db name in Oracle
                    //It is different from other db types
                    myInfo.SID = allInfo.Database;

                    myInfo.Username = allInfo.Username;
                    myInfo.Password = allInfo.Pwd;
                    myInfo.Port = allInfo.Port;
                    
                    //The hostname can be ip address of server or machine name
                    myInfo.HostName = allInfo.Server;
                }

                myConnString = DbConnectionString.Oracle.ConnectionStringWithoutClient(myInfo.HostName, myInfo.Port.ToString(),
                    myInfo.SID, myInfo.Username, myInfo.Password);

                //Here just use the dll in odp.net 
                //but we need confirm the right version of current client 
                baseConn = new OracleConnection(myConnString);



                baseConn.Open();

                invalidator = new Invalidation.InvalidationBase();
                CurDatabase = myInfo.SID;
                CurPwd = myInfo.Password;
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {
            throw new NotImplementedException();
        }

    }
}
