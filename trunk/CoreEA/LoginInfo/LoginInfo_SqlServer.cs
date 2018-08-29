using System;
using System.Collections.Generic;
using System.Text;
using CoreEA.Args;
using CoreEA.InfrastructureInfo;

namespace CoreEA.LoginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInfo_SqlServer:BaseLoginInfo
    {
        private string _attchFile = string.Empty;
        /// <summary>
        /// If attch file use this property
        /// 
        /// The attach file mode need a sql server instance ,
        /// here we make the server name as the instance.
        /// </summary>
        public string AttchFile
        {
            get
            {
                return _attchFile;
            }
            set
            {
                _attchFile = value;
            }
        }



        private CurDbServerConnMode X_CurDbServerConnMode;
        /// <summary>
        /// 
        /// </summary>
        public CurDbServerConnMode X_CurDbConnectionMode
        {
            get
            {
                return X_CurDbServerConnMode;
            }

            set
            {
                X_CurDbServerConnMode = value;
            }
        }


        private bool _isTrustedConn = true;
        /// <summary>
        /// Used in Sqlserver
        /// </summary>
        public bool IsTrustedConn
        {
            get { return _isTrustedConn; }
            set { _isTrustedConn = value; }
        }

        private string _server = string.Empty;
        /// <summary>
        /// Db Server name
        /// </summary>
        public string X_Server
        {
            get { return _server; }
            set { _server = value; }
        }

        private string _port = string.Empty;
        /// <summary>
        /// The db port to connect
        /// </summary>
        public string X_Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private string _userName = string.Empty;
        /// <summary>
        /// login db username
        /// </summary>
        public string X_UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _pwd = string.Empty;
        /// <summary>
        /// login db pwd
        /// </summary>
        public string X_Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }

        private string _database = string.Empty;
        /// <summary>
        /// database name
        /// </summary>
        public string X_Database
        {
            get { return _database; }
            set { _database = value; }
        }

        private string _tableName = string.Empty;
        /// <summary>
        /// table name
        /// </summary>
        public string X_TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// This object is speical for sql server
        /// Becuase the properties in login info didn't has all the necessary data
        /// so add this object ,this will be used when create database
        /// </summary>
        public BaseCreateDbObject CreateDatabaseObject { get; set; }
    }
}
