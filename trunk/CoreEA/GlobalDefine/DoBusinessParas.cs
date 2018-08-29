using System;
using System.Collections.Generic;
using System.Text;

namespace CoreEA.GlobalDefine
{
    /// <summary>
    /// This class is used by AutoCodeMachine
    /// </summary>
    public class DoBusinessParas
    {
        private string _uid;
        /// <summary>
        /// 
        /// </summary>
        public string UID
        {
            get { return _uid; }
            set { _uid = value; }
        }

        private int _port;
        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private string _server;
        /// <summary>
        /// 
        /// </summary>
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        private string _pwd;
        /// <summary>
        /// 
        /// </summary>
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }
        private bool _isEncrypted;
        /// <summary>
        /// 
        /// </summary>
        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set { _isEncrypted = value; }
        }

        private string _databaseName;
        /// <summary>
        /// 
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }
        private string _tableName;
        /// <summary>
        /// 
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }
        private string _namespace;
        /// <summary>
        /// 
        /// </summary>
        public string NamespaceName
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        private string _className;
        /// <summary>
        /// 
        /// </summary>
        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
        }
        private string _outputFolderName;
        /// <summary>
        /// 
        /// </summary>
        public string OutputFolderName
        {
            get { return _outputFolderName; }
            set { _outputFolderName = value; }
        }
        private bool _isAttachFile;
        /// <summary>
        /// 
        /// </summary>
        public bool IsAttachedFile
        {
            get { return _isAttachFile; }
            set { _isAttachFile = value; }
        }
        private string _connectionString;
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        public DoBusinessParas(string connectionStr)
        {
            ConnectionString = connectionStr;
        }
        /// <summary>
        /// 
        /// </summary>
        public DoBusinessParas()
        {

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="databaseName"> If Not Sever Base DB , the database is the db file name </param>
        public DoBusinessParas(string tableName, string databaseName)
        {
            TableName = tableName;
            DatabaseName = databaseName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="databaseName">If Not Sever Base DB , the database is the db file name </param>
        /// <param name="server"></param>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="port"></param>
        public DoBusinessParas(string tableName, string databaseName,string server,string uid,string pwd,int port)
        {
            TableName = tableName;
            DatabaseName = databaseName;
            Server = server;
            UID = uid;
            Pwd = pwd;
            Port = port;
        }
    }

}
