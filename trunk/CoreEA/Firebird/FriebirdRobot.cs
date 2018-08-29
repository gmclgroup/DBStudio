using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEA.LoginInfo;
using CoreEA.Invalidation;
using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;
using CoreEA.SchemaInfo;

namespace CoreEA.Firebird
{
    internal class FriebirdRobot : BaseRobot
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
                    currentCommandTextHandler = new FirebirdCommandText();
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
            if (columnName.StartsWith("'"))
            {
                if (columnName.EndsWith("'"))
                {
                    return columnName;
                }
                else
                {
                    return string.Format("{0}'", columnName);
                }
            }
            else
            {
                if (columnName.EndsWith("'"))
                {
                    return string.Format("'{0}", columnName);
                }
                else
                {
                    return string.Format("'{0}'", columnName);
                }

            }
        }

        public sealed override int MaxTableNameLength
        {
            get 
            {
                return 50;
            }
        }

        public sealed override CoreEA.SchemaInfo.BaseTableSchema GetTableSchemaInfoObject(string tableName)
        {
            throw new NotImplementedException();
        }


        public sealed override bool HasIdentityColumnInTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public sealed override List<string> GetSystemViewList()
        {
            throw new NotImplementedException();
        }

        public sealed override System.Data.DataTable GetProviderInfoFromTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public sealed override CoreE.UsedDatabaseType HostedType
        {
            get
            {
                return CoreEA.CoreE.UsedDatabaseType.Firebird;
            }
        }

        public sealed override System.Data.Common.DbDataAdapter GetDataAdapter(System.Data.Common.DbCommand dbCmd)
        {
            FbCommand myCmd = dbCmd as FbCommand;
            if (myCmd == null) throw new ArgumentException();
            FbDataAdapter sa = new FbDataAdapter(myCmd);
            return sa;
        }

        public sealed override System.Data.Common.DbCommand GetNewStringCommand(string sql)
        {
            DbCommand cmd = new FbCommand();
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

        public sealed override bool CreateDatabase(BaseLoginInfo loginInfo)
        {
            throw new NotImplementedException();
        }

        public sealed override void Open(CoreEA.LoginInfo.BaseLoginInfo info)
        {
            LoginInfo_Firebird myInfo = info as LoginInfo_Firebird;
            LoginInfo_ForAllDbTypes allInfo = info as LoginInfo_ForAllDbTypes;

            if ((myInfo == null) && (allInfo == null))
            {
                throw new ArgumentException("Only Support Firebird login info and AllDBTypes Info");
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
                    myInfo = new LoginInfo_Firebird();
                    myInfo.DataFile = allInfo.Database;
                    myInfo.Password = allInfo.Pwd;
                    myInfo.DataSource = allInfo.Server;
                    myInfo.Username = allInfo.Username;
                }

                FbConnectionStringBuilder strBuilder = new FbConnectionStringBuilder();
                strBuilder.Charset = myInfo.CharSet;
                strBuilder.ConnectionLifeTime = myInfo.ConnectionLifeTime;
                strBuilder.Database = myInfo.DataFile;
                strBuilder.DataSource = myInfo.DataSource;
                strBuilder.Dialect = myInfo.Dialect;
                strBuilder.MaxPoolSize = myInfo.MaxPollSize;
                strBuilder.MinPoolSize = myInfo.MinPollSize;
                strBuilder.PacketSize = myInfo.PacketSize;
                strBuilder.Password = myInfo.Password;
                strBuilder.Pooling = myInfo.IsPolling;
                strBuilder.Port = myInfo.Port;

                baseConn = new FbConnection(strBuilder.ConnectionString);
                baseConn.Open();

                CurDatabase = myInfo.DataFile;
                CurPwd = myInfo.Password;

                invalidator = new InvalidatorForFirebird();
            }
            catch (Exception e)
            {
                throw e;
            }
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
                baseConn = new FbConnection(connectionString);
                baseConn.Open();

                invalidator = new InvalidatorForFirebird();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public sealed override List<string> GetTableListInDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public sealed override bool ExecuteProcedureWithNoQuery(string procedureName, object[] varList, System.Data.OleDb.OleDbType[] dbTypeList, int[] objectLengthList, object[] objectList, object[] objectValueList)
        {
            throw new NotImplementedException();
        }

        public sealed override string GetMaskedTableName(string tableName)
        {
            if (tableName.StartsWith("'"))
            {
                return tableName;
            }
            else
            {

                return string.Format("'{0}'", tableName);
            }
            
        }

        public sealed override decimal GetColumnLength(string tableName, string columName)
        {
            throw new NotImplementedException();
        }
    }
}
